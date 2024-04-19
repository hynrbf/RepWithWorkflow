<script lang="ts">
import {defineComponent, inject} from "vue";
import {
  UploadFileInfo,
  UploadOnAddEvent,
  UploadOnRemoveEvent,
  UploadOnProgressEvent,
  UploadOnStatusChangeEvent,
  UploadFileStatus,
  UploadFileRestrictions,
} from "@progress/kendo-vue-upload";
import {container} from "tsyringe";
import {
  IMediaFileService,
  IMediaFileServiceInfo,
} from "@/infra/dependency-services/rest/media-file/IMediaFileService";
import {PropType} from "vue";
import isEmpty from "lodash/isEmpty";
import {AppConstants} from "@/infra/AppConstants";
import {Emitter, EventType} from "mitt";

export default defineComponent({
  name: "KendoUploadInputComponent",
  props: {
    name: {
      type: String,
      default: "",
    },
    id: {
      type: String,
      default: "",
    },
    label: {
      type: String,
      default: "",
    },
    optionalText: {
      type: String,
      default: "(Optional)",
    },
    isRequired: {
      type: Boolean,
      default: true,
    },
    multiple: {
      type: Boolean,
      default: false,
    },
    autoUpload: {
      type: Boolean,
      default: true,
    },
    allowedExtensions: {
      type: Array,
      default: () => [],
    },
    value: {
      type: Object as PropType<UploadFileInfo[]>,
      default: () => [] as UploadFileInfo[],
    },
    stretched: Boolean,
    dropzone: Boolean,
    uploadFor: {
      type: String,
      default: "",
    },
    buttonText: {
      type: String,
      default: "Upload File",
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    asFileModel: Boolean,
  },
  data() {
    return {
      eventBus: inject("$eventBusService") as Emitter<
          Record<EventType, unknown>
      >,
      mediaFileService: container.resolve<IMediaFileService>(
          IMediaFileServiceInfo.name,
      ),
      files: [] as UploadFileInfo[],
    };
  },
  setup() {
    return {
      kendoForm: inject<any>("kendoForm", {default: {}}),
    };
  },
  computed: {
    restrictions() {
      return {
        allowedExtensions: this.allowedExtensions,
      } as UploadFileRestrictions;
    },
  },
  watch: {
    "kendoForm.values": {
      handler(value) {
        if (!value[this.name]) {
          this.files = [];
        }
      },
      deep: true,
    },
  },
  mounted() {
    if (!isEmpty(this.value)) {
      this.files = this.value.map((file) => {
        file.status = UploadFileStatus.Uploaded;
        file.progress = 100;
        return file;
      });
    }

    if (this.dropzone) {
      this.transformToDropzone();
    }
  },
  methods: {
    sendUploadRequest(
        files: UploadFileInfo[],
        options: { formData: FormData; requestOptions: any },
        onProgress: (uid: string, event: ProgressEvent<EventTarget>) => void,
    ): Promise<{ uid: string }> {
      return new Promise((resolve, reject) => {
        const file = files[0];
        const rawFile = file?.getRawFile?.() as Blob;
        const fileExtension = `${file.extension}`.replace(/^\./, "");
        const fileSize = rawFile.size;

        if (AppConstants.ImageExtensions.includes(fileExtension)) {
          const fileName = `image-${Date.now()}`;
          this.mediaFileService
              .uploadImageAsync(
                  rawFile,
                  fileExtension,
                  fileName,
                  this.uploadFor,
                  options.formData,
                  (event: ProgressEvent) => {
                    onProgress(file.uid, event);
                  },
              )
              .then((response) => {
                this.storeToForm(
                    file.uid,
                    fileName,
                    fileExtension,
                    fileSize,
                    response,
                );

                this.$emit("onFinishedUpload", response);
                this.eventBus.emit(AppConstants.formFieldChangedEvent);
                this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
                resolve({uid: file.uid});
              })
              .catch(() => {
                reject(file);
              });

          return;
        }

        if (AppConstants.DocumentExtensions.includes(fileExtension)) {
          const fileName = `document-${Date.now()}`;
          this.mediaFileService
              .uploadDocumentAsync(
                  rawFile,
                  fileExtension,
                  fileName,
                  this.uploadFor,
                  options.formData,
                  (event: ProgressEvent) => {
                    onProgress(file.uid, event);
                  },
              )
              .then((response) => {
                const fileUrl = response.fileUrl ?? response;
                this.storeToForm(
                    file.uid,
                    fileName,
                    fileExtension,
                    fileSize,
                    fileUrl as string,
                );

                this.$emit("onFinishedUpload", fileUrl);
                this.eventBus.emit(AppConstants.formFieldChangedEvent);
                this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
                resolve({uid: file.uid});
              })
              .catch(() => {
                reject(file);
              });
          return;
        }

        reject(file);
      });
    },

    sendRemoveRequest(files: UploadFileInfo[]) {
      this.removeFromForm(files[0].uid);
      // @TODO Request remove file API endpoint
      return Promise.resolve({uid: files[0].uid});
    },

    handleUpload(
        event:
            | UploadOnAddEvent
            | UploadOnRemoveEvent
            | UploadOnProgressEvent
            | UploadOnStatusChangeEvent,
    ) {
      this.files = event.newState;
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
    },

    storeToForm(
        uid: string,
        name: string,
        extension: string,
        size: number,
        uploadedUrl: string,
    ) {
      let value = this.kendoForm.values[this.name];

      const data = this.mapToFileModel({
        uid,
        name,
        extension,
        size,
        uploadedUrl,
      });

      if (this.multiple) {
        value = Array.isArray(value) ? [...value, data] : [data];
      } else {
        value = data;
      }

      this.kendoForm.onChange(this.name, {
        value,
      });
      this.$emit("updated", value);
    },

    removeFromForm(uid: string) {
      let value = this.kendoForm.values[this.name];

      if (this.multiple) {
        value = (Array.isArray(value) ? value : []).filter(
            (item) => item.uid !== uid,
        );
      } else {
        value = null;
      }

      this.kendoForm.onChange(this.name, {
        value,
      });
      this.$emit("updated", value);
    },

    transformToDropzone() {
      const element = (this.$refs["upload"] as any).$el as HTMLDivElement;
      const icon = this.$refs["icon"] as HTMLDivElement;

      if (!element || !icon) {
        return;
      }

      const dropzone = element.querySelector(
          ".k-upload-dropzone",
      ) as HTMLDivElement;
      const button = element.querySelector(
          ".k-upload-button",
      ) as HTMLButtonElement;
      const hint = element.querySelector(".k-dropzone-hint") as HTMLDivElement;

      if (!dropzone || !button || !hint) {
        return;
      }

      dropzone.prepend(icon);
      button.innerHTML = this.buttonText;
      const linkTextStyle =
          "text-decoration: underline; color: var(--brand-color-brand-primary);font-size: 14px;font-style: normal;font-weight: 700;";
      hint.innerHTML = `Drag and Drop or <span style='${linkTextStyle}'>Browse File</span>`;

      dropzone.addEventListener("click", () => {
        button.click();
      });
    },

    mapToFileModel({uid, name, extension, size, uploadedUrl}: any) {
      if (this.asFileModel) {
        return {
          id: uid,
          name,
          extension,
          size,
          url: uploadedUrl,
        };
      }
      return {
        uid,
        name,
        extension,
        size,
        uploadedUrl,
      };
    },
  },
});
</script>

<template>
  <Field :name="name" :id="id" component="field">
    <template v-slot:field="{ props }">
      <StackLayout
          orientation="vertical"
          :gap="8"
          :align="{ horizontal: 'start' }"
          :class="props.class"
      >
        <Label class="control-label">
          {{ label }}
          <span v-if="!isRequired" class="fineprint ms-1">
            {{ optionalText }}
          </span>
        </Label>

        <div v-if="dropzone" ref="icon" class="Upload-icon">
          <IconComponent symbol="upload-tray-9" size="20"/>
        </div>

        <Upload
            ref="upload"
            list="list"
            :auto-upload="autoUpload"
            :batch="false"
            :multiple="multiple"
            :with-credentials="false"
            :files="files"
            :restrictions="restrictions"
            :class="[
            'Upload',
            dropzone && 'Upload--dropzone',
            stretched && 'w-100',
          ]"
            :save-url="sendUploadRequest"
            :remove-url="sendRemoveRequest"
            :disabled="disabled"
            @add="handleUpload($event)"
            @remove="handleUpload($event)"
            @progress="handleUpload($event)"
            @statuschange="handleUpload($event)"
        >
          <template v-slot:list="{ props }">
            <UploadListSingleItem
                :files="props.files"
                :async="props.async"
                @cancel="props.onCancel"
                @remove="props.onRemove"
                @retry="props.onRetry"
            />
          </template>
        </Upload>
      </StackLayout>
    </template>
  </Field>
</template>

<style scoped lang="scss">
.Upload {
  border: none;

  &-icon {
    padding: 10px;
    border-radius: 50%;
    background: #cce0ff;
    margin-bottom: 5px;

    svg {
      display: block;
    }
  }

  &--dropzone {
    :deep(.k-upload-dropzone) {
      border-radius: 8px;
      display: flex;
      flex-direction: column;
      padding: 24.5px;
      background: #f4faff;
      border: 1px dashed var(--color-primary);
      border-radius: 8px;
    }

    :deep(.k-upload-button) {
      pointer-events: none;
      box-shadow: none;
      border: none;
      background: none;
      font-size: var(--font-size-default);
      font-weight: var(--font-weight-bold);
      padding: 5px;
    }

    :deep(.k-dropzone-hint),
    :deep(.k-upload-status) {
      font-size: var(--font-size-sm);
    }

    :deep(.k-upload-files) {
      border-color: transparent;
    }
  }
}
</style>
