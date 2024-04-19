<script lang="ts">
import { defineComponent } from "vue";
import { v4 as uuidv4 } from "uuid";
import { container } from "tsyringe";
import { Input } from "@progress/kendo-vue-inputs";
import { Loader } from "@progress/kendo-vue-indicators";
import { Label } from "@progress/kendo-vue-labels";
import { DropDownList } from "@progress/kendo-vue-dropdowns";
import { UploadFileInfo, UploadOnAddEvent } from "@progress/kendo-vue-upload";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { PermissionEdit } from "@/entities/PermissionEdit";
import { AppConstants } from "@/infra/AppConstants";
import KendoDialog, {
  KendoDialogComponent,
} from "../../../../components/KendoDialog.vue";

export default defineComponent({
  name: "EditPermission",
  components: {
    KendoDialog,
    kendoInput: Input,
    Loader,
    Label,
    DropDownList,
  },
  data() {
    return {
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      permissionEditList: [] as PermissionEdit[],
      permissionGroupNames: [] as string[],
      categoryNames: [] as string[],
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
    // category names and permission group names will come from look-up table soon.
    this.categoryNames = this.getCategoryNameList();
    this.permissionGroupNames = this.getPermissionGroupNameList();
    let definedGroupedPermissions = await this.fcaService
      .getDefinedPermissionsAsync()
      .finally(() => (this.isLoading = false));

    if (!definedGroupedPermissions) {
      return;
    }

    this.permissionEditList = definedGroupedPermissions;
  },
  methods: {
    async savePermissionsAsync() {
      if (!this.permissionEditList || this.permissionEditList.length < 1) {
        return;
      }

      this.isLoading = true;
      let result = await this.fcaService
        .savePermissionsAsync(this.permissionEditList)
        .finally(() => {
          this.isLoading = false;
        });

      return result;
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
          this.permissionEditList = this.generateFcaPermissionsFromContent(
            this.content,
          );
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

    generateFcaPermissionsFromContent(
      content: string | ArrayBuffer | null,
    ): PermissionEdit[] {
      let splits = content?.toString().split("\r\n") ?? [];

      if (splits.length < 1) {
        return [] as PermissionEdit[];
      }

      let rows = splits.splice(1);
      // Remove empty row
      rows = rows.filter((r) => r.length > 0);
      const colNoOfCategoryInCsv = 1;
      const colNoOfGroupNameInCsv = 2;
      const colNoOfPermissionInCsv = 3;
      const colNoOfDisplayTextInCsv = 4;

      let permissionEditModels = [] as PermissionEdit[];

      for (let row of rows) {
        let splits = row.split("\t");
        let permission: PermissionEdit = {
          id: uuidv4(),
          permissionGroupName: splits[colNoOfGroupNameInCsv].trim(),
          categoryName: splits[colNoOfCategoryInCsv].trim(),
          subPermissionName: splits[colNoOfPermissionInCsv].trim(),
          subPermissionDisplayText: splits[colNoOfDisplayTextInCsv].trim(),
        };

        permissionEditModels.push(permission);
      }

      return permissionEditModels;
    },

    getCategoryNameList(): string[] {
      // This values will come from a look-up table soon.
      return [
        AppConstants.PermissionCategoryAdditional,
        AppConstants.PermissionCategoryInsuranceBroker,
        AppConstants.PermissionCategoryMortgateBroker,
      ].sort();
    },

    getPermissionGroupNameList(): string[] {
      // This values will come from a look-up table soon.
      return [
        "Owner Occupier Mortgages",
        "Consumer Buy to Let Mortgages",
        "Home Reversion Plans",
        "Home Purchase Plans",
        "Debt Consolidation",
        "Advising deals in Investments",
        "Arranging deals in Investments",
        "Making Arrangements…",
        "Incepting a policy on behalf of the Insurer or Insured",
        "Providing ongoing support during the term of the policy",
        "Credit Broking",
        "Hold and Control Client Money",
      ].sort();
    },
  },
});
</script>

<template src="./edit_permission.html" />

<style src="./edit_permission.css" scoped />
