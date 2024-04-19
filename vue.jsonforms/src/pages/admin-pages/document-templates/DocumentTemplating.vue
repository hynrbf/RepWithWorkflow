<script lang="ts">
import { defineComponent } from "vue";
import Editor from "@tinymce/tinymce-vue";
import {
  DropDownList,
  DropDownListChangeEvent,
} from "@progress/kendo-vue-dropdowns";
import { TINY_MCE_API_KEY } from "@/config";
import { container } from "tsyringe";
import {
  IDocumentService,
  IDocumentServiceInfo,
} from "@/infra/dependency-services/rest/document-html/IDocumentService";
import { UploadFileInfo, UploadOnAddEvent } from "@progress/kendo-vue-upload";
import { v4 as uuidv4 } from "uuid";
import { HtmlSource } from "@/entities/HtmlSource";
import { DocumentNames } from "@/entities/enums/DocumentNames";
import KendoDialog, {
  KendoDialogComponent,
} from "../../../components/KendoDialog.vue";

export default defineComponent({
  name: "DocumentTemplating",
  components: { KendoDialog, Editor, DropDownList },
  mounted() {
    if (this.$refs.kendoDialog) {
      this.kendoDialogInstance = this.$refs.kendoDialog as KendoDialogComponent;
    }
  },
  data() {
    return {
      htmlSourceDocumentService: container.resolve<IDocumentService>(
        IDocumentServiceInfo.name,
      ),
      apiKey: TINY_MCE_API_KEY,
      name: "",
      content: "",
      files: [] as UploadFileInfo[],
      selectedDocument: null as unknown as HtmlSource,
      file: null as unknown as Blob,
      documentNames: Object.keys(DocumentNames),
      kendoDialogInstance: null as KendoDialogComponent | null,
    };
  },
  methods: {
    async onConvertWordToHtmlAsync(event: UploadOnAddEvent) {
      const file = event.target.files[0];

      let importedFile = file as UploadFileInfo;
      let fileExtension = importedFile.name.substring(
        importedFile.name.lastIndexOf(".") + 1,
      );
      let htmlSourceModel =
        await this.htmlSourceDocumentService.uploadWordToConvertToHtmlAsync(
          file as Blob,
          fileExtension,
          this.name,
        );

      if (htmlSourceModel) {
        this.kendoDialogInstance?.setDialogMessage(
          "Upload word document successful!",
          "Success",
        );
        this.content = htmlSourceModel.content;
        this.selectedDocument = htmlSourceModel;
        this.resetUploadFile();
        return;
      }

      this.kendoDialogInstance?.setDialogMessage(
        "Upload word document failed!",
        "Error",
      );
      this.resetUploadFile();
    },

    async saveContent() {
      // remove all -aw-import: ignore; by aspose
      let cleanedContent = this.content.replaceAll("-aw-import: ignore;", "");
      this.content = cleanedContent;

      if (!this.selectedDocument) {
        let htmlSourceModel: HtmlSource = {
          content: this.content,
          id: uuidv4(),
          name: this.name,
        };

        this.selectedDocument = htmlSourceModel;
      } else {
        this.selectedDocument.content = this.content;
      }

      let documentJson = JSON.stringify(this.selectedDocument);
      let updatedDocument: HtmlSource =
        await this.htmlSourceDocumentService.updateDocumentAsync(documentJson);
      this.content = updatedDocument.content;
      this.kendoDialogInstance?.setDialogMessage("Document saved!", "Success");
    },

    resetUploadFile() {
      let input = this.$refs.doc as HTMLInputElement;
      input.value = "";
    },

    selectName(event: DropDownListChangeEvent) {
      this.name = event.target.value;
    },
  },
});
</script>

<template src="./document_templating.html" />

<style scoped></style>