<script setup lang="ts">
import { computed, toRefs, ref } from "vue";
import { container } from "tsyringe";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { AppConstants } from "@/infra/AppConstants";
import StaticList from "@/infra/StaticListService";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

const props = withDefaults(
  defineProps<{
    items: {
      principalFirmName: string;
      firmReferenceNumber: string;
      startDate: number;
      endDate: number;
      reasonForTermination: string;
    }[];
  }>(),
  {
    items: () => [],
  }
);

const { items } = toRefs(props);

const helperService = container.resolve<IHelperService>(
  IHelperServiceInfo.name
);

const terminationOptions = computed(() =>
  StaticList.getTerminationReasons().map((item) => ({
    label: item,
    value: item,
  }))
);

const newItem = ref({
  principalFirmName: " ",
  firmReferenceNumber: "",
  startDate: 0,
  endDate: 0,
  reasonForTermination: "",
});

const addNewItem = () => {
  items.value.push(newItem.value);
  newItem.value = {
    principalFirmName: " ",
    firmReferenceNumber: "",
    startDate: 0,
    endDate: 0,
    reasonForTermination: "",
  };
};

const deleteItem = (index: number) => {
  items.value.splice(index, 1);

  if (items?.value && items.value.length >= 1) {
    newItem.value = {
      principalFirmName: "",
      firmReferenceNumber: "",
      startDate: 0,
      endDate: 0,
      reasonForTermination: "",
    };
  }
};

const unableToAdd = computed(() => {
  return (
    newItem.value.principalFirmName == " " ||
    newItem.value.firmReferenceNumber == "" ||
    newItem.value.startDate == 0 ||
    newItem.value.endDate == 0 ||
    newItem.value.reasonForTermination == ""
  );
});

const setUniqueIdentifier = (value: string): string => {
  const identifier = `${AppConstants.arFirmDetailsRoute}${value}`;
  return identifier.replace(/\s+/g, "").replace("/", "");
};

const convertEpochValueToDate = (
  input: number | undefined
): Date | undefined => {
  if (!input) {
    return undefined;
  }

  if (input === -1) {
    return undefined;
  }

  return helperService.convertEpochToDateTime(input);
};

const onCompanyValueChanged = (company: CompanyEntity): void => {
  newItem.value.principalFirmName = company.companyName;
  newItem.value.firmReferenceNumber = company.firmReferenceNo;
};

const onCompanyValueChangedList = (
  company: CompanyEntity,
  index: number
): void => {
  items.value[index].principalFirmName = company.companyName;
  items.value[index].firmReferenceNumber = company.firmReferenceNo;
};

const onCompanyNameInput = (value: string, index: number) => {
  items.value[index].principalFirmName = value;
};
</script>

<template>
  <div class="PrincipalFirmTerminationTable">
    <div class="container-fluid">
      <div class="PrincipalFirmTerminationTable-head row">
        <div class="PrincipalFirmTerminationTable-cell col-lg-3">
          Principal Firm Name
        </div>
        <div class="PrincipalFirmTerminationTable-cell col-lg-1p5">FRN</div>
        <div class="PrincipalFirmTerminationTable-cell col-lg-2">
          Start Date
        </div>
        <div class="PrincipalFirmTerminationTable-cell col-lg-2">End Date</div>
        <div class="PrincipalFirmTerminationTable-cell col-lg-3">
          Reason For Termination
        </div>
        <div class="PrincipalFirmTerminationTable-cell col-lg-0p2">&nbsp;</div>
      </div>
    </div>

    <div
      v-for="(item, index) in items"
      :key="`fee-item-${index}`"
      class="container-fluid"
    >
      <div class="PrincipalFirmTerminationTable-row row">
        <div class="col-lg-3 py-3 px-2">
          <KendoCompaniesAutoCompleteComponent
            v-if="false"
            :mode="0"
            :id="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.principalFirmName`
              )
            "
            :name="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.principalFirmName`
              )
            "
            class="HasPreviouslyBeenAnARItems-input"
            :value="item?.principalFirmName"
            @onValueChange="
              (company: CompanyEntity) =>
                onCompanyValueChangedList(company, index)
            "
            @onInput="(value: string) => onCompanyNameInput(value, index)"
            :isShowHighLightColor="false"
            isValueReactive
            :isDataLoadedCompletely="true"
          />
          <KendoGenericInputComponent
            :name="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.principalFirmName`
              )
            "
            :id="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.principalFirmName`
              )
            "
            :value="item?.principalFirmName"
            :isCapitalizeFirstLetter="true"
            :isCheckForProfanity="true"
            isValueReactive
            :isDataLoadedCompletely="true"
          />
        </div>
        <div class="col-lg-1p5 py-3 px-2">
          <KendoNumericTextInputComponent
            :id="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.firmReferenceNumber`
              )
            "
            :name="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.firmReferenceNumber`
              )
            "
            :placeholder="'1234567'"
            :value="item?.firmReferenceNumber"
            isValueReactive
            :isDataLoadedCompletely="true"
            :maxlength="'7'"
          />
        </div>
        <div class="col-lg-2 py-3 px-2">
          <KendoDatePickerInputComponent
            :id="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.startDate`
              )
            "
            :name="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.startDate`
              )
            "
            class="HasPreviouslyBeenAnARItems-date"
            @onValueChange="
              (value: Date) =>
                (item.startDate = helperService.dateStringToEpoch(
                  value.toDateString()
                ))
            "
            :value="convertEpochValueToDate(item.startDate ?? undefined)"
            isValueReactive
            :isDataLoadedCompletely="true"
            :isRequired="true"
          />
        </div>
        <div class="col-lg-2 py-3 px-2">
          <KendoDatePickerInputComponent
            :id="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.endDate`
              )
            "
            :name="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.endDate`
              )
            "
            class="HasPreviouslyBeenAnARItems-date"
            @onValueChange="
              (value: Date) =>
                (item.endDate = helperService.dateStringToEpoch(
                  value.toDateString()
                ))
            "
            :value="convertEpochValueToDate(item.endDate ?? undefined)"
            isValueReactive
            :isDataLoadedCompletely="true"
            :isRequired="true"
          />
        </div>
        <div class="col-lg-3 py-3 px-2">
          <KendoDropdownListComponent
            :id="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.reasonForTermination`
              )
            "
            :name="
              setUniqueIdentifier(
                `-${index}-additionalInformation.hasPreviouslyBeenAnARItems.reasonForTermination`
              )
            "
            class="HasPreviouslyBeenAnARItems-dropdown"
            :data-items="terminationOptions"
            value-primitive
            isValueReactive
            :isDataLoadedCompletely="true"
            placeholder="Please Select"
            :is-required="false"
            :model-value="item.reasonForTermination"
            @update:model-value="item.reasonForTermination = $event"
          />
        </div>
        <div class="col-lg-0p2 px-2">
          <button
            type="button"
            class="ActionButton"
            @click.prevent="deleteItem(index)"
          >
            <IconComponent symbol="trash" class="text-error" size="20" />
          </button>
        </div>
      </div>
    </div>

    <div class="container-fluid">
      <div class="row">
        <div class="col-lg-3 py-3 px-2">
          <KendoCompaniesAutoCompleteComponent
            :id="
              setUniqueIdentifier(
                `-additionalInformation.hasPreviouslyBeenAnARItems.principalFirmName`
              )
            "
            :name="
              setUniqueIdentifier(
                `-additionalInformation.hasPreviouslyBeenAnARItems.principalFirmName`
              )
            "
            class="HasPreviouslyBeenAnARItems-input"
            :value="newItem?.principalFirmName"
            :isShowBottomResults="false"
            isValueReactive
            :isDataLoadedCompletely="true"
            :isEditable="true"
            @onValueChange="onCompanyValueChanged"
            :isShowHighLightColor="false"
          />
        </div>
        <div class="col-lg-1p5 py-3 px-2">
          <KendoNumericTextInputComponent
            :id="
              setUniqueIdentifier(
                `-additionalInformation.hasPreviouslyBeenAnARItems.firmReferenceNumber-new`
              )
            "
            :name="
              setUniqueIdentifier(
                `-additionalInformation.hasPreviouslyBeenAnARItems.firmReferenceNumber-new`
              )
            "
            :placeholder="'1234567'"
            :value="newItem?.firmReferenceNumber"
            isValueReactive
            :isDataLoadedCompletely="true"
            :maxlength="'7'"
            :isRequired="true"
          />
        </div>
        <div class="col-lg-2 py-3 px-2">
          <KendoDatePickerInputComponent
            :id="
              setUniqueIdentifier(
                `-additionalInformation.hasPreviouslyBeenAnARItems.startDate-new`
              )
            "
            :name="
              setUniqueIdentifier(
                `-additionalInformation.hasPreviouslyBeenAnARItems.startDate-new`
              )
            "
            class="HasPreviouslyBeenAnARItems-date"
            @onValueChange="
              (value: Date) =>
                (newItem.startDate = helperService.dateStringToEpoch(
                  value.toDateString()
                ))
            "
            :value="convertEpochValueToDate(newItem.startDate ?? undefined)"
            isValueReactive
            :isDataLoadedCompletely="true"
            :isRequired="true"
            :internalModel="'Start Date'"
          />
        </div>
        <div class="col-lg-2 py-3 px-2">
          <KendoDatePickerInputComponent
            :id="
              setUniqueIdentifier(
                `-additionalInformation.hasPreviouslyBeenAnARItems.endDate-new`
              )
            "
            :name="
              setUniqueIdentifier(
                `-additionalInformation.hasPreviouslyBeenAnARItems.endDate-new`
              )
            "
            class="HasPreviouslyBeenAnARItems-date"
            @onValueChange="
              (value: Date) =>
                (newItem.endDate = helperService.dateStringToEpoch(
                  value.toDateString()
                ))
            "
            :value="convertEpochValueToDate(newItem.endDate ?? undefined)"
            isValueReactive
            :isDataLoadedCompletely="true"
            :isRequired="true"
            :internalModel="'End Date'"
          />
        </div>
        <div class="col-lg-3 py-3 px-2">
          <KendoDropdownListComponent
            :id="
              setUniqueIdentifier(
                `-additionalInformation.hasPreviouslyBeenAnARItems.reasonForTermination-new`
              )
            "
            :name="'Reason For Termination'"
            class="HasPreviouslyBeenAnARItems-dropdown"
            :data-items="terminationOptions"
            value-primitive
            placeholder="Please Select"
            :model-value="newItem.reasonForTermination"
            @update:model-value="newItem.reasonForTermination = $event"
            :isRequired="true"
            isValueReactive
            :isDataLoadedCompletely="true"
          />
        </div>
        <div class="col-lg-0p2 pt-23px px-2">
          <button
            type="button"
            class="ActionButton"
            :disabled="unableToAdd"
            @click.prevent="addNewItem"
          >
            <IconComponent
              symbol="add-circle-27"
              :class="[!unableToAdd && 'text-primary']"
              size="20"
            />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.PrincipalFirmTerminationTable {
  width: 100%;
  border: 1px solid var(--infotable-accent-color);
  border-radius: 8px;
  overflow: hidden;

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
    border-bottom: 1px solid var(--infotable-accent-color);
    align-items: center;
  }

  &-foot {
    font-size: var(--font-size-sm);
    align-items: center;
  }

  &-cell {
    padding: 10px;

    &:last-child {
    }
  }

  .row {
    margin-top: 0;

    div {
      margin-top: 0;
    }
  }
}

.ActionButton {
  background: none;
  border: none;
  padding: 0;
  margin: 0;
}

.HasPreviouslyBeenAnARItems {
  &-date {
    div.k-align-items-stretch {
      gap: 0 !important;
    }
  }

  &-dropdown {
    div.k-dropdownlist {
      gap: 0 !important;
      margin-top: 0 !important;
    }
  }

  &-input {
    div.k-align-items-stretch {
      gap: 0 !important;
      height: 38px !important;
    }
  }
}

.HasPreviouslyBeenAnARItems-input > .k-align-items-stretch {
  gap: 0 !important;
  height: 38px !important;
}

.col-lg-1p5 {
  flex: 0 0 auto;
  width: 13.33%;
}

.col-lg-0p2 {
  flex: 0 0 auto;
  width: 3.338%;
}
</style>
