<script lang="ts">
import { defineComponent, inject } from "vue";
import { Emitter, EventType } from "mitt";
import { AppConstants } from "@/infra/AppConstants";
import { MultiSelectChangeEvent } from "@progress/kendo-vue-dropdowns";
import axios from "axios";
import { usePageComponentValidationValueStore } from "@/stores/progress-bar/usePageComponentValidationValueStore";

export default defineComponent({
  name: "KendoNationalityInputComponent",
  props: {
    name: {
      type: String,
      default: "",
    },
    id: String,
    label: {
      type: String,
      default: "",
    },
    placeholder: {
      type: String,
      default: "",
    },
    isRequired: {
      type: Boolean,
      default: true,
    },
    value: {
      type: Array as () => string[],
      default: [] as string[],
    },
    optionalText: {
      type: String,
      default: "Optional",
    },
    //these both should be used for reactive data,
    //otherwise the validation of Kendo behaves incorrectly
    isValueReactive: {
      type: Boolean,
      default: false,
    },
    isDataLoadedCompletely: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      isFieldValidOnFirstLoad: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      nationalitiesWithFlags: [] as {
        name: string;
        flag: string;
        code: string;
        nationality: string;
      }[],
      nationalities: [] as { countryCode: string; nationality: string }[],
      valuesAsObjectInternal: [] as {
        name: string;
        flag: string;
        code: string;
        nationality: string;
      }[],
      additionalNationality: "",
      isDropdownOpened: false,
      isAddingOther: false,
      isUnmounted: false,
      errorMessage: ""
    };
  },
  computed: {
    computedPlaceholder() {
      if (!this.$t("common-placeholder-text-dropdown")) {
        return this.placeholder;
      }

      return this.$t("common-placeholder-text-dropdown");
    },
  },
  watch: {
    isDataLoadedCompletely(newValue, oldValue): boolean {
      if (newValue == oldValue) {
        return newValue;
      }

      this.isFieldValidOnFirstLoad =
        this.validate(this.value as string[]) == "";
      return newValue;
    },
  },
  setup() {
    const pageComponentValidationValueStore =
      usePageComponentValidationValueStore();
    const { addComponentValidationValue, removeValidationValueByFieldId } =
      pageComponentValidationValueStore;

    return {
      addComponentValidationValue,
      removeValidationValueByFieldId,
    };
  },
  async created() {
    await this.setupFlags();
    this.initValue(this.value);

    if (this.isValueReactive) {
      return;
    }

    this.isFieldValidOnFirstLoad = this.validate(this.value as string[]) == "";
  },
  mounted() {
    this.isUnmounted = false;

    if (this.isValueReactive && this.isDataLoadedCompletely) {
      this.isFieldValidOnFirstLoad =
        this.validate(this.value as string[]) == "";
    }
  },
  async updated() {
    this.isFieldValidOnFirstLoad = this.validate(this.value as string[]) == "";
    await this.setupFlags();
    this.initValue(this.value);
  },
  unmounted() {
    if (!this.id) {
      return;
    }

    this.removeValidationValueByFieldId(this.id);
    this.isUnmounted = true;
  },
  methods: {
    async setupFlags() {
      const nationalitiesResponse = await axios.get(
        "/api/country-nationalities.json",
      );
      let nationalitiesObj = JSON.parse(
        JSON.stringify(nationalitiesResponse.data),
      );
      this.nationalities = nationalitiesObj.map((n: any) => {
        return {
          countryCode: n.alpha_2_code,
          nationality: n.nationality,
        };
      });

      const response = await axios.get("/api/countries-all.json");
      let countryObj = JSON.parse(JSON.stringify(response.data));

      const nationalities = countryObj.map((p: any) => {
        return {
          name: p.name?.common,
          flag: p.flags?.svg,
          code: p.cca2,
          nationality: this.getNationality(p.cca2),
        };
      });

      this.nationalitiesWithFlags = nationalities
        .filter(
          (n: {
            name: string;
            flag: string;
            code: string;
            nationality: string;
          }) => n.nationality,
        )
        .slice()
        .sort(
          (
            a: {
              name: string;
              flag: string;
              code: string;
              nationality: string;
            },
            b: {
              name: string;
              flag: string;
              code: string;
              nationality: string;
            },
          ) => {
            const aNationality = this.getFirstNationality(a.nationality);
            const bNationality = this.getFirstNationality(b.nationality);
            return aNationality.localeCompare(bNationality);
          },
        );
    },

    initValue(values: string[]) {
      this.valuesAsObjectInternal = [];

      for (const value of values) {
        const found = this.nationalitiesWithFlags.find((f) =>
          f.nationality.startsWith(value),
        );

        this.valuesAsObjectInternal.push({
          nationality: value,
          flag: found?.flag ?? "",
          code: found?.code ?? "",
          name: found?.name ?? "",
        });
      }
    },

    getNationality(countryCode: string): string {
      if (!countryCode) {
        return "NO COUNTRY CODE";
      }

      const found = this.nationalities?.find(
        (n) => n.countryCode === countryCode,
      );
      return found?.nationality ?? "";
    },

    getFlagSvg(countryCode: string): string {
      // reference: https://github.com/HatScripts/circle-flags
      return `https://hatscripts.github.io/circle-flags/flags/${countryCode.toLowerCase()}.svg`;
    },

    getFirstNationality(nationality: string): string {
      if (!nationality.includes(",")) {
        return nationality;
      }

      const items = nationality.split(",");
      return items[0]?.trim();
    },

    onSelect(event: MultiSelectChangeEvent) {
      this.isFieldValidOnFirstLoad = false;
      this.valuesAsObjectInternal = event.value as {
        name: string;
        flag: string;
        code: string;
        nationality: string;
      }[];
      this.$emit(
        "onValueChange",
        this.valuesAsObjectInternal.map((v) => v.nationality),
      );
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      this.isAddingOther = false;
    },

    validate(currentValue: string[]) {
      this.errorMessage = "";
      //note, even if you remove the component in the DOM 'using v-if', the vue still preserves it in memory
      //that's why we need this variable
      if (this.isUnmounted) {
        return "";
      }

      if (!this.isRequired) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (this.isFieldValidOnFirstLoad) {
        this.addIdKeyAndErrorValue("");
        return "";
      }

      if (!currentValue || currentValue.length < 1) {
        const errorMessage = `Please enter a valid ${this.label}`;
        this.addIdKeyAndErrorValue(errorMessage);
        this.errorMessage = errorMessage;
        return errorMessage;
      }

      this.addIdKeyAndErrorValue("");
      return "";
    },

    onItemClear(selectedItem: any) {
      let indexOf = this.valuesAsObjectInternal.indexOf(selectedItem);
      this.valuesAsObjectInternal.splice(indexOf, 1);
      this.$emit(
        "onValueChange",
        this.valuesAsObjectInternal.map((v) => v.nationality),
      );
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      this.outFocusAddOther();
    },

    onFocus() {
      // need delay to fully open the dropdown
      setTimeout(() => (this.isDropdownOpened = true), 100);
    },

    onBlur() {
      if (this.isAddingOther) {
        return;
      }

      this.isDropdownOpened = false;
    },

    onAddOtherClick(event: any) {
      this.isAddingOther = true;
      event.target?.focus();
    },

    onAddOther() {
      const newNationalityItem = {
        nationality: this.additionalNationality,
        name: "",
        code: "",
        flag: "",
      };
      this.nationalitiesWithFlags.push(newNationalityItem);
      this.valuesAsObjectInternal.push(newNationalityItem);
      this.$emit(
        "onValueChange",
        this.valuesAsObjectInternal.map((v) => v.nationality),
      );
      this.eventBus.emit(AppConstants.formFieldChangedEvent);
      this.eventBus.emit(AppConstants.formFieldPageLevelChangedEvent);
      this.outFocusAddOther();
    },

    outFocusAddOther() {
      this.isAddingOther = false;
      this.isDropdownOpened = false;
    },

    addIdKeyAndErrorValue(value: string) {
      if (!this.id) {
        return;
      }

      const key = this.id;
      let errorItem: Record<string, string> = {};
      errorItem[key] = value;
      this.addComponentValidationValue(key, errorItem);
    },
  },
});
</script>

<template>
  <div>
    <Field
      :id="id"
      :name="name"
      :component="'myTemplate'"
      :validator="validate"
      class="nationality-field"
    >
      <template v-slot:myTemplate="{ props }">
        <StackLayout orientation="vertical" style="gap: 6px">
          <Label :editor-id="props.id" class="control-label">
            {{ label }}
            <span v-if="!isRequired" class="fineprint ms-1">
              ({{ optionalText }})
            </span>
          </Label>

          <MultiSelect
            ref="multiselect"
            v-bind="props"
            :id="props.id"
            :name="props.name"
            :valid="!(props.touched && errorMessage)"
            :placeholder="computedPlaceholder"
            :value="valuesAsObjectInternal"
            :opened="isDropdownOpened"
            size="small"
            style="border-radius: 8px"
            @change="onSelect"
            @blur="onBlur"
            @focus="onFocus"
            :data-items="nationalitiesWithFlags"
            :text-field="'nationality'"
            :data-item-key="'code'"
            :item-render="'myTemplate'"
            :tag-render="'myTag'"
            :footer="'myFooter'"
          >
            <template v-slot:myTemplate="{ props }">
              <StackLayout
                class="p-2"
                orientation="horizontal"
                @click="(ev: Event) => props.onClick(ev)"
                :gap="8"
                :align="{ horizontal: 'start', vertical: 'middle' }"
              >
                <Checkbox :value="props.selected" />

                <img
                  class="flag-icon"
                  :src="getFlagSvg(props.dataItem['code'])"
                  :alt="props.dataItem['code']"
                />

                <Label class="flex-grow-1">{{
                  getFirstNationality(props.dataItem["nationality"])
                }}</Label>
              </StackLayout>
            </template>

            <template v-slot:myTag="{ props }">
              <StackLayout
                class="nationality-tag-item"
                orientation="horizontal"
                :align="{ horizontal: 'start', vertical: 'middle' }"
              >
                <img
                  class="flag-icon"
                  :src="getFlagSvg(props.tagData.data[0].code)"
                  :alt="props.tagData.data[0].code"
                />

                <Label>{{ getFirstNationality(props.tagData.text) }}</Label>

                <span
                  class="k-icon k-i-close-circle tag-clear"
                  @click="() => onItemClear(props.tagData.data[0])"
                ></span>
              </StackLayout>
            </template>

            <template #myFooter>
              <StackLayout
                class="nationality-footer"
                orientation="horizontal"
                :gap="8"
                :align="{ horizontal: 'start', vertical: 'middle' }"
              >
                <KendoGenericInputComponent
                  class="flex-grow-1"
                  :isRequired="false"
                  :value="additionalNationality"
                  :customStyle="{ borderWidth: 0 }"
                  placeholder="Other"
                  @click="onAddOtherClick"
                  @focusout="outFocusAddOther"
                  @onValueChange="(v: string) => (additionalNationality = v)"
                />

                <IconComponent
                  class="add-other-icon"
                  symbol="add-circle-27"
                  size="20"
                  @click="onAddOther"
                />
              </StackLayout>
            </template>
          </MultiSelect>

          <Error v-if="props.touched && errorMessage"
            >{{ errorMessage }}
          </Error>
        </StackLayout>
      </template>
    </Field>
  </div>
</template>

<style scoped>
.flag-icon {
  width: 18px;
  height: 18px;
}

.nationality-tag-item {
  padding: 2px 3px;
  display: flex;
  align-items: center;
  height: 24px;
  gap: 2px;
  border: 1px solid var(--neutral-gray-300);
  border-radius: 35px;
  margin: 0 2px;
}

.tag-clear {
  color: var(--text-text-disabled);
  scale: 1.2; /* Estimated only to match icon dimension as close as possible to the design */
  cursor: pointer;
}

.nationality-footer {
  padding: 4px 12px 4px 6px;
  margin: 12px;
  border: 1px solid var(--content-content-07);
  border-radius: 8px;
}

.add-other-icon {
  color: var(--brand-color-brand-primary);
  cursor: pointer;
}

.nationality-field :deep(.k-input-values) {
  padding-top: 0 !important;
  padding-bottom: 0 !important;
}
</style>