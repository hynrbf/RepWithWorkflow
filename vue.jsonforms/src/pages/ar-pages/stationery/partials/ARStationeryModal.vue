<script setup lang="ts">
import { ref, toRefs, computed, onMounted, onBeforeMount } from "vue";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import { FileModel } from "@/pages/firm-profile-pages/stationery/model/FileModel";
import { StationeryEntity } from "@/entities/stationery/StationeryEntity";
import isEmpty from "lodash/isEmpty";
import { AppConstants } from "@/infra/AppConstants";

const props = withDefaults(
  defineProps<{ initialValues?: Partial<StationeryEntity>; saving: boolean }>(),
  {}
);
const emits = defineEmits<{
  (event: "submit", value: Partial<StationeryEntity>): void;
  (event: "update", value: Partial<StationeryEntity>): void;
}>();

const { initialValues } = toRefs(props);

const modalElement = ref();
const formElement = ref();

const pageLifeCycleStore = usePageLifeCycleStore();
const { changeLifeCycleName } = pageLifeCycleStore;

const isEdit = computed(() => !isEmpty(initialValues?.value));

const handleSubmit = (values: StationeryEntity) => {
  if (values.files) {
    values.files = values.files.map(
      ({
        uid = "",
        id = "",
        name = "",
        extension = "",
        size = 0,
        uploadedUrl = "",
        url = "",
      }) => ({
        id: id || uid || "",
        name,
        extension,
        size,
        url: uploadedUrl || url,
      })
    ) as FileModel[];
  }

  if (isEdit.value) {
    emits("update", {
      ...values,
    });
  } else {
    emits("submit", {
      ...values,
    });
  }

  formElement.value.reset();
};

onBeforeMount(() => {
  changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
});

onMounted(() => {
  changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
});

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.arStationeryRoute}-stationaryModal${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};
</script>

<template>
  <ModalComponent
    ref="modalElement"
    :title="`${isEdit ? 'Edit' : 'Add'} Stationery`"
  >
    <KendoForm :initialValues="initialValues" @submit="handleSubmit">
      <KendoFormElementComponent ref="formElement" v-slot="{ form }">
        <OverlayLoader :loading="saving">
          <KendoGenericInputComponent
            :id="setUniqueIdentifier( '-name')"
            name="name"
            label="Stationery Name"
            class="mb-3"
            :value="form.values.name"
          />

          <KendoUploadInputComponent
            label="Upload File"
            name="files"
            optionalText="(Accepted file types: pdf, jpg, png, mp4, mov,  Maximum 25 MB file size)"
            stretched
            multiple
            dropzone
            :value="form.values.files"
            :isRequired="false"
            :allowed-extensions="[
              '.pdf',
              '.jpg',
              '.png',
              '.gif',
              '.webp',
              '.jpeg',
              '.mov',
              '.mp4',
            ]"
          />
        </OverlayLoader>
      </KendoFormElementComponent>
    </KendoForm>

    <template #footer>
      <div class="text-right">
        <KendoButton
          type="button"
          theme-color="primary"
          :disabled="saving"
          @click="formElement.submit()"
        >
          {{ isEdit ? "Save Changes" : "Save & Add" }}
        </KendoButton>
      </div>
    </template>
  </ModalComponent>
</template>
