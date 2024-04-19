<script lang="ts">
import { defineComponent } from "vue";
import { container } from "tsyringe";
import {
  Input,
  Checkbox,
  CheckboxChangeEvent,
} from "@progress/kendo-vue-inputs";
import { Loader } from "@progress/kendo-vue-indicators";
import { DropDownList } from "@progress/kendo-vue-dropdowns";
import { FcaAuthStatus } from "@/entities/FcaAuthStatus";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { UploadFileInfo, UploadOnAddEvent } from "@progress/kendo-vue-upload";
import { v4 as uuidv4 } from "uuid";
import { AppConstants } from "@/infra/AppConstants";
import KendoDialog, {
  KendoDialogComponent,
} from "../../../../components/KendoDialog.vue";

export default defineComponent({
  name: "EditStatus",
  components: {
    KendoDialog,
    kendoInput: Input,
    Loader,
    DropDownList,
    kendoCheckbox: Checkbox,
  },
  data() {
    return {
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      fcaStatuses: [] as FcaAuthStatus[],
      // Temp only
      colorCodingValues: ["Red", "Green", "Blue"],
      files: [] as UploadFileInfo[],
      file: null as unknown as Blob,
      content: null as string | ArrayBuffer | null,
      isLoading: false,
      kendoDialogInstance: null as KendoDialogComponent | null,
    };
  },
  mounted() {
    if (this.$refs.kendoDialog) {
      this.kendoDialogInstance = this.$refs.kendoDialog as KendoDialogComponent;
    }
  },
  async created() {
    this.isLoading = true;
    this.fcaStatuses = await this.fcaService.getFcaDefinedStatusesAsync();
    this.fcaStatuses.forEach((status) => {
      let found =
        this.fcaStatuses.find((i) => i.actualStatus === status.actualStatus)
          ?.generalStatus === "Authorised";

      if (!found) {
        return false;
      }

      status.isAuthorised = found;
    });

    this.isLoading = false;
  },

  methods: {
    async saveStatusesAsync() {
      if (!this.fcaStatuses || this.fcaStatuses.length < 1) {
        return;
      }

      this.isLoading = true;
      let result = await this.fcaService
        .saveFcaStatusesAsync(this.fcaStatuses)
        .finally(() => {
          this.isLoading = false;
        });

      return result;
    },

    onUpdateGeneralStatus(
      event: CheckboxChangeEvent,
      selectedStatus: FcaAuthStatus,
    ) {
      let found = this.fcaStatuses.find((s) => s.id === selectedStatus.id);

      if (!found) {
        return;
      }

      found.generalStatus = event.value
        ? "Authorised"
        : AppConstants.notAuthorised;
      found.isAuthorised = event.value;
    },

    onUploadFile(event: UploadOnAddEvent) {
      this.files = event.newState;
      this.readFile();
    },

    readFile() {
      let input = this.$refs.doc as HTMLInputElement;

      if (!input.files) {
        return;
      }

      this.file = input.files[0];
      const reader = new FileReader();
      let currentFile = this.file as File;

      if (currentFile.name.includes(".txt")) {
        reader.onload = async (res) => {
          if (!res.target) {
            return;
          }

          this.content = res.target.result;
          this.fcaStatuses = this.generateFcaStatusesFromContent(this.content);
          // Reset upload input
          input.value = "";
        };
        reader.onerror = (err) => err;
        reader.readAsText(this.file as Blob);
      } else {
        this.kendoDialogInstance?.setDialogMessage(
          "Invalid file type uploaded! \nMust be tab separated text file.",
        );
        input.value = ""; // Reset upload input
      }
    },

    generateFcaStatusesFromContent(
      content: string | ArrayBuffer | null,
    ): FcaAuthStatus[] {
      let splits = content?.toString().split("\r\n") ?? [];

      if (splits.length < 1) {
        return [] as FcaAuthStatus[];
      }

      let rows = splits.splice(1);
      // Remove empty row
      rows = rows.filter((r) => r.length > 0);
      const colNoOfCurrentActualStatusInCsv = 0;
      const colNoOfColorCodingInCsv = 1;
      const colNoOfGeneralStatusInCsv = 2;

      let fcaAuthStatuses = [] as FcaAuthStatus[];

      for (let row of rows) {
        let splits = row.split("\t");
        let permission: FcaAuthStatus = {
          id: uuidv4(),
          actualStatus: splits[colNoOfCurrentActualStatusInCsv].trim(),
          isAuthorised:
            splits[colNoOfGeneralStatusInCsv].trim() === "Authorised",
          generalStatus: splits[colNoOfGeneralStatusInCsv].trim(),
          colorCoding: splits[colNoOfColorCodingInCsv], // Temp for now
        };

        fcaAuthStatuses.push(permission);
      }

      return fcaAuthStatuses;
    },
  },
});
</script>

<template src="./edit_status.html" />

<style src="./edit_status.css" scoped />
