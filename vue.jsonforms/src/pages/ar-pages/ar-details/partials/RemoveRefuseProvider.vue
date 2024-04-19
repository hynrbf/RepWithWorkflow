<script setup lang="ts">
import { toRefs, ref } from "vue";
import { container } from "tsyringe";
import { AppConstants } from "@/infra/AppConstants";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

const props = withDefaults(
  defineProps<{
    items: {
      status: string;
      providerName: string;
      partyName: string;
      refusedDate: number;
      refusedInfo: string;
      refusedSupportingDocumentsUrls: Array<string>;
    }[];
    refuseStatuses: {
      label: string;
      value: string;
    }[];
    placeholder: string;
  }>(),
  {
    items: () => [],
  },
);

const { items } = toRefs(props);

const helperService = container.resolve<IHelperService>(
  IHelperServiceInfo.name,
);

const deleteItem = (index: number) => {
  if (items?.value && items.value.length > 1) {
    items.value.splice(index, 1);
  }
};

const newItem = ref({
  status: "",
  providerName: "",
  partyName: "",
  refusedDate: 0,
  refusedInfo: "",
  refusedSupportingDocumentsUrls: [],
});

const addNewItem = () => {
  items.value.push(newItem.value);
  newItem.value = {
    status: "",
    providerName: "",
    partyName: "",
    refusedDate: 0,
    refusedInfo: "",
    refusedSupportingDocumentsUrls: [],
  };
};

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.arFirmDetailsRoute}${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};

const convertEpochValueToDate = (
  input: number | undefined,
): Date | undefined => {
  if (!input) {
    return undefined;
  }

  if (input === -1) {
    return undefined;
  }

  return helperService.convertEpochToDateTime(input);
};
</script>
<template>
  <div class="RemoveRefusePanel">
    <div class="RemoveRefusePanel-head">
      <div class="RemoveRefusePanel-cell pl-43px">Removed/Refused</div>
      <div class="RemoveRefusePanel-cell">Provider Name</div>
      <div class="RemoveRefusePanel-cell">Name of Party Refused/Removed</div>
      <div class="RemoveRefusePanel-cell">Date Removed/Refused</div>
    </div>
    <div v-if="items.length > 0">
      <PanelComponent
        v-for="(item, index) in items"
        :key="index"
        class="position-relative"
      >
        <KendoButton
          type="button"
          size="small"
          rounded="full"
          shape="square"
          theme-color="light"
          title="View"
          class="pannel-close"
          fill-mode="outline"
          @click="deleteItem(index)"
          v-if="items.length > 1 && index > 0"
        >
          <IconComponent symbol="close" size="10" />
        </KendoButton>

        <div class="RemoveRefusePanel-row">
          <div class="RemoveRefusePanel-cell">
            <KendoRadioGroupComponent
              :id="setUniqueIdentifier(`-${index}-additionalInformation.hasBeenRemovedOrRefusedItems.status`)"
              :name="'Status'"
              :value="item.status"
              :dataItems="refuseStatuses"
              :isRequired="item.providerName != ''"
              @onValueChange="(value: string) => (item.status = value)"
            />
          </div>
          <div class="RemoveRefusePanel-cell">
            <KendoGenericInputComponent
              :name="`${index}-additionalInformation.hasBeenRemovedOrRefusedItems.providerName`"
              :id="setUniqueIdentifier(`-${index}-additionalInformation.hasBeenRemovedOrRefusedItems.providerName`)"
              class="col"
              @onValueChange="(value: string) => (item.providerName = value)"
              :value="item.providerName"
              :isCapitalizeFirstLetter="true"
              :isCheckForProfanity="true"
              :isValueReactive="true"
            />
          </div>
          <div class="RemoveRefusePanel-cell">
            <KendoGenericInputComponent
              :name="`${index}-additionalInformation.hasBeenRemovedOrRefusedItems.partyName`"
              :id="setUniqueIdentifier(`-${index}-additionalInformation.hasBeenRemovedOrRefusedItems.partyName`)"
              class="col"
              @onValueChange="(value: string) => (item.partyName = value)"
              :value="item.partyName"
              :isCapitalizeFirstLetter="true"
              :isCheckForProfanity="true"
              :isValueReactive="true"
            />
          </div>
          <div class="RemoveRefusePanel-cell">
            <KendoDatePickerInputComponent
              :id="setUniqueIdentifier(`-${index}-additionalInformation.hasBeenRemovedOrRefusedItems.refusedDate`)"
              class="refused-date"
              :name="
                setUniqueIdentifier(
                  `-${index}-additionalInformation.hasBeenRemovedOrRefusedItems.refusedDate`,
                )
              "
              @onValueChange="
                (value: Date) =>
                  (item.refusedDate = helperService.dateStringToEpoch(
                    value.toDateString(),
                  ))
              "
              :value="convertEpochValueToDate(item.refusedDate ?? undefined)"
              :isValueReactive="true"
            />
          </div>
        </div>
        <div class="RemoveRefusePanel-row mt-20px">
          <KendoTextAreaInputComponent
            :id="setUniqueIdentifier(`-${index}-additionalInformation.hasBeenRemovedOrRefusedItems.refusedInfo`)"
            :name="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasBeenRemovedOrRefusedItems.refusedInfo`,
              )
            "
            class="flex-grow-1 mb-15px"
            label="Additional Information"
            :corner-radius="8"
            :placeholder="placeholder"
            @onValueChange="(value: string) => (item.refusedInfo = value)"
            :value="item.refusedInfo"
            :isValueReactive="true"
            :isRequired="false"
          />
        </div>
        <div class="RemoveRefusePanel-row">
          <KendoUploadInputComponent
            label="Supporting documents"
            class="w-100"
            optionalText="(Optional) (Accepted file types: pdf, jpg, doc, Maximum 25 MB file size)"
            stretched
            multiple
            dropzone
            buttonText="Upload file"
            :isRequired="false"
            :allowed-extensions="['.pdf', 'doc', 'docx']"
            :uploadFor="`ARDetailsInsurerEverDeclined-additionalInformation-refusedSupportingDocumentsUrls`"
            @onFinishedUpload="
              (url: string) => item.refusedSupportingDocumentsUrls.push(url)
            "
          />
        </div>
      </PanelComponent>
    </div>

    <KendoButton
      fill-mode="outline"
      theme-color="primary"
      class="ml-10px mt-8px"
      @click="addNewItem"
    >
      <IconComponent symbol="add-1-28" width="15" class="me-1" />
      {{
        $t("arDetailsPage-additional-hasBeenRemovedOrRefused-addAnotherButton")
      }}
    </KendoButton>
  </div>
</template>

<style scoped lang="scss">
.RemoveRefusePanel {
  width: 100%;
  border: 1px solid var(--infotable-accent-color);
  border-radius: 8px;
  overflow: hidden;
  padding-bottom: 15px;

  &-head,
  &-row,
  &-foot {
    display: flex;
  }

  &-head {
    background-color: var(--infotable-accent-color);
    font-size: var(--font-size-xs);
    font-weight: var(--font-weight-bold);
  }

  &-row {
    align-items: center;

    .RemoveRefusePanel-cell {
      padding: 0 10px;
    }
  }

  &-foot {
    font-size: var(--font-size-sm);
    align-items: left;
  }

  &-cell {
    flex: 1;
    padding: 10px;

    &:first-child {
    }
  }

  &-amount {
    font-size: var(--font-size-lg);
    font-weight: var(--font-weight-bold);
    color: rgba(151, 161, 175, 1);
  }

  .Panel {
    margin: 10px !important;
  }
}

.CurrencyInput {
  :deep(.k-input-prefix) {
    background-color: var(--content-content-10);
    padding: 5px 12px;
    color: rgba(151, 161, 175, 1);
    border-right: 1px solid var(--content-content-07);
  }
}

.ActionButton {
  background: none;
  border: none;
  padding: 0;
  margin: 0;
}

.refused-date > .k-align-items-stretch {
  gap: 0 !important;
}

.pannel-close {
  width: 20px;
  height: 20px;
  color: var(--color-primary);
  position: absolute;
  right: 5px;
  top: 5px;
  z-index: 1000;
}
</style>
