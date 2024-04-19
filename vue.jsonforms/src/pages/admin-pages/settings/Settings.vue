<script lang="ts">
import { defineComponent } from "vue";
import { container } from "tsyringe";
import { Button } from "@progress/kendo-vue-buttons";
import { TextArea, TextAreaChangeEvent } from "@progress/kendo-vue-inputs";
import { Label } from "@progress/kendo-vue-labels";
import { Loader } from "@progress/kendo-vue-indicators";
import { SettingEntity } from "@/entities/SettingEntity";
import {
  ISettingServiceInfo,
  ISettingService,
} from "@/infra/dependency-services/rest/settings/ISettingService";
import { UploadFileInfo, UploadOnAddEvent } from "@progress/kendo-vue-upload";
import { v4 as uuidv4 } from "uuid";
import KendoDialog, {
  KendoDialogComponent,
} from "../../../components/KendoDialog.vue";

export default defineComponent({
  name: "Settings",
  components: {
    KendoDialog,
    Button,
    TextArea,
    Loader,
    Label,
  },
  data() {
    return {
      settingService: container.resolve<ISettingService>(
        ISettingServiceInfo.name,
      ),
      settings: [] as SettingEntity[],
      isLoadingSettings: false,
      files: [] as UploadFileInfo[],
      file: null as unknown as Blob,
      content: null as string | ArrayBuffer | null,
      kendoDialogInstance: null as KendoDialogComponent | null,
    };
  },
  mounted() {
    if (this.$refs.kendoDialog) {
      this.kendoDialogInstance = this.$refs.kendoDialog as KendoDialogComponent;
    }
  },
  async created() {
    this.isLoadingSettings = true;
    this.settings = await this.settingService
      .getAllSettingsAsync()
      .finally(() => {
        this.isLoadingSettings = false;
      });
  },
  methods: {
    onValueChanged(event: TextAreaChangeEvent, setting: SettingEntity) {
      setting.value = event.value;
    },

    onDeleteSetting(_event: Event, setting: SettingEntity) {
      let foundIndex = this.settings.indexOf(setting);

      if (foundIndex !== -1) {
        this.settings.splice(foundIndex, 1);
      }
    },

    async saveSettings() {
      this.isLoadingSettings = true;
      await this.settingService
        .saveAllSettingsAsync(this.settings)
        .finally(() => {
          this.isLoadingSettings = false;
        });

      this.kendoDialogInstance?.setDialogMessage("Settings saved!", "Success");
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

          if (this.settings.length > 0) {
            this.settings.splice(0);
          }

          this.settings = this.generateSettingsFromContent(this.content);
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

    generateSettingsFromContent(
      content: string | ArrayBuffer | null,
    ): SettingEntity[] {
      let splits = content?.toString().split("\r\n") ?? [];

      if (splits.length < 1) {
        return [] as SettingEntity[];
      }

      let rows = splits.splice(1);
      // Remove empty row
      rows = rows.filter((r) => r.length > 0);
      const colNoOfkeyInCsv = 0;
      const colNoOfValueInCsv = 1;
      const colNoOfUsedForInCsv = 2;

      let settings = [] as SettingEntity[];

      for (let row of rows) {
        let splits = row.split("\t");
        let setting: SettingEntity = {
          id: uuidv4(),
          key: splits[colNoOfkeyInCsv].trim(),
          value: splits[colNoOfValueInCsv].trim(),
          usedFor: splits[colNoOfUsedForInCsv].trim(),
        };

        settings.push(setting);
      }

      return settings;
    },
  },
});
</script>

<template src="./settings.html" />

<style scoped />