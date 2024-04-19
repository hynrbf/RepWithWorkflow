<script lang="ts">
import { defineComponent } from "vue";
import { v4 as uuidv4 } from "uuid";
import { TINY_MCE_API_KEY } from "@/config";
import { container } from "tsyringe";
import { HtmlSource } from "@/entities/HtmlSource";
import Editor from "@tinymce/tinymce-vue";
import { UploadFileInfo, UploadOnAddEvent } from "@progress/kendo-vue-upload";
import {
  ISigningService,
  ISigningServiceInfo,
} from "@/infra/dependency-services/rest/sign-now/ISigningService";
import {
  IDocumentService,
  IDocumentServiceInfo,
} from "@/infra/dependency-services/rest/document-html/IDocumentService";
import {
  DropDownList,
  DropDownListChangeEvent,
} from "@progress/kendo-vue-dropdowns";
import { DocumentNames } from "@/entities/enums/DocumentNames";
import KendoDialog, {
  KendoDialogComponent,
} from "../../../components/KendoDialog.vue";

export default defineComponent({
  name: "DocumentEdit",
  components: {
    KendoDialog,
    editor: Editor,
    DropDownList,
  },
  mounted() {
    if (this.$refs.kendoDialog) {
      this.kendoDialogInstance = this.$refs.kendoDialog as KendoDialogComponent;
    }
  },
  async created() {
    let foundDocument =
      await this.htmlSourceDocumentService.getDocumentsAsync();

    if (!foundDocument || foundDocument.length < 1) {
      return;
    }

    this.selectedDocument = foundDocument[0];
    this.content = this.selectedDocument.content;
  },
  data() {
    return {
      htmlSourceDocumentService: container.resolve<IDocumentService>(
        IDocumentServiceInfo.name,
      ),
      signNowService: container.resolve<ISigningService>(
        ISigningServiceInfo.name,
      ),
      selectedDocument: null as unknown as HtmlSource,
      content: "",
      recipientEmail: "",
      name: "",
      signButtonClass: "btn btn-success col-2 disabled",
      apiKey: TINY_MCE_API_KEY,
      files: [] as UploadFileInfo[],
      file: null as unknown as Blob,
      documentNames: Object.keys(DocumentNames),
      kendoDialogInstance: null as KendoDialogComponent | null,
    };
  },
  watch: {
    recipientEmail: function (val) {
      let cleanedRecipientEmail = val.trim();
      let defaultClass = "btn btn-success col-2";

      if (!this.isEmailValid(cleanedRecipientEmail)) {
        defaultClass += " disabled";
      }

      this.signButtonClass = defaultClass;
    },
  },
  methods: {
    async saveContent() {
      // remove all -aw-import: ignore; by aspose
      let cleanedContent = this.content.replaceAll("-aw-import: ignore;", "");
      this.content = cleanedContent;

      if (!this.selectedDocument) {
        const id = uuidv4();

        let htmlSourceModel: HtmlSource = {
          content: this.content,
          id: id,
          name: this.name,
        };

        this.selectedDocument = htmlSourceModel;
      } else {
        this.selectedDocument.content = this.content;
      }

      let documentJson = JSON.stringify(this.selectedDocument);
      let updatedDocument =
        await this.htmlSourceDocumentService.updateDocumentAsync(documentJson);
      this.content = updatedDocument.content;
      this.kendoDialogInstance?.setDialogMessage("Document saved!", "Success");
    },

    async signAsync() {
      let cleanedRecipientEmail = this.recipientEmail.trim();
      let isSuccess = await this.signNowService.sendInviteToSignDocument(
        cleanedRecipientEmail,
      );

      if (isSuccess) {
        this.kendoDialogInstance?.setDialogMessage(
          "Document sign invite success!",
          "Success",
        );
      }
    },

    isEmailValid(email: string): boolean {
      const emailRegex: RegExp = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i;
      const result: boolean = emailRegex.test(email);
      return result;
    },

    getApiHeartBeat() {
      this.kendoDialogInstance?.setDialogMessage("Success", "Success");
    },

    async onConvertWordToHtmlAsync(event: UploadOnAddEvent) {
      const file = event.target.files[0];

      let importedFile = file as UploadFileInfo;
      let fileExtension = importedFile.name.substring(
        importedFile.name.lastIndexOf(".") + 1,
      );
      let htmlContentModel =
        await this.htmlSourceDocumentService.uploadWordToConvertToHtmlAsync(
          file as Blob,
          fileExtension,
          this.name,
        );

      if (htmlContentModel) {
        this.kendoDialogInstance?.setDialogMessage(
          "Upload word document is successful!",
          "Success",
        );
        return;
      }

      this.kendoDialogInstance?.setDialogMessage(
        "Upload word document is failed!",
      );
    },

    async onConvertHtmlToDocAsync() {
      let documentUrl =
        await this.htmlSourceDocumentService.convertHtmlToWordAsync();
      window.open(documentUrl, "_blank", "noreferrer");
    },

    selectName(event: DropDownListChangeEvent) {
      this.name = event.target.value;
    },
  },
});
</script>

<template src="./document_edit.html" />

<style scoped></style>
