<script lang="ts">
import {defineComponent} from "vue";
import {Field} from "@progress/kendo-vue-form";
import {Input} from "@progress/kendo-vue-inputs";
import UkFlagComponent from "../../components/UkFlagComponent.vue";
import {Label, Error} from "@progress/kendo-vue-labels";

export default defineComponent({
  name: "KendoContactNumberInputFormElement",
  props: {
    optional: Boolean,
    disabled: Boolean,
    touched: Boolean,
    label: {
      type: String,
      default: 'Contact Number'
    },
    validationMessage: String,
    name: {
      type: String,
      default: ''
    },
    hint: String,
    id: String,
    value: {
      type: String,
      default: ''
    },
    // custom
    placeholder: {
      type: String,
      default: ''
    },
    isRequired: {
      type: Boolean,
      default: false
    },
    maxLength: {
      type: Number,
      default: 11
    }
  },
  components: {
    Field,
    Error,
    Label,
    Input,
    UkFlagComponent
  },
  data() {
    return {
      maxLengthInternal: 0,
      valueInternal: ''
    }
  },
  computed: {
    showValidationMessage() {
      let result = this.$props.touched && this.$props.validationMessage;
      return result;
    },
    isValid(props: { valid: boolean }): boolean {
      return props.valid;
    }
    // showHint() {
    //   return !this.showValidationMessage && this.$props.hint;
    // },
    // hintId() {
    //   return this.showHint ? `${this.$props.id}_hint` : "";
    // },
    // errorId() {
    //   return this.showValidationMessage ? `${this.$props.id}_error` : "";
    // }
  },
  created() {
    // this.labelInternal = this.label;
    // this.placeholderInternal = this.placeholder;
    // this.isRequiredInternal = this.isRequired;
    this.maxLengthInternal = this.maxLength;
    this.valueInternal = this.value;
    this.autoFormatNumbersOnLoad(this.valueInternal);
  },
  methods: {
    onInput(event: any) {
      let value = event.target.value;
      this.calculateMaxLength(value);
      this.autoUkNumberFormat(value);
      this.$emit('onValueChange', event.target.value);
    },
    onBlur(event: any) {
      this.$emit('blur', event);
    },
    handleFocus(event: any) {
      this.$emit('focus', event);
    },
    validateNumber(value: string): string {
      //return value ? '' : 'Please...';
      if (!this.isRequired) {
        return "";
      }

      if (!value || value.length < 10) {
        return `Please enter a valid ${this.label}`;
      }

      return "";
    },

    autoFormatNumbersOnLoad(value: string) {
      this.calculateMaxLength(value);
      this.autoUkNumberFormat(value);
    },

    calculateMaxLength(value: string) {
      if (value) {
        let firstChar: string = value[0];

        if (firstChar == "0" && this.maxLengthInternal != 11) {
          this.maxLengthInternal = 11;
        }

        if (firstChar != "0" && this.maxLengthInternal != 10) {
          this.maxLengthInternal = 10;
        }
      }
    },

    // ref. https://grantify.io/eic-book-a-call/
    autoUkNumberFormat(value: string) {
      if (value.length === this.maxLengthInternal) {
        // the last digit is supplied
        if (value[0] === "0") {
          // remove the first zero
          value = value.substring(1);
        }

        value = value.replace(/(\d{4})(\d{6})/, '$1 $2');
      } else {
        value = value.replaceAll(" ", "");
      }

      this.valueInternal = value;
      this.$emit("onValueChange", this.valueInternal);
    }
  }
});

</script>

<template>
  <Field :name="name"
         :id="id"
         :type="'email'"
         :validator="validateNumber"
         :component="'myTemplate'">
    <template v-slot:myTemplate="{props}">
      {{props}}
      <div class="d-flex flex-column gap-2">
        <Label :editor-valid="props.valid"
               :editor-id="props.id"
               class="control-label">{{ label }}<span v-if="isRequired" style="color:red;">*</span></Label>

        <div class="input-container d-flex align-items-center px-3 py-0">
          <UkFlagComponent/>

          <div class="col-auto" style="margin-left: 5px;">
            <Label>+44 (0)</Label>
          </div>

          <Input v-bind="props"
                 class="input"
                 size="small"
                 fill-mode="null"
                 :id="props.id"
                 :valid="props.valid"
                 :placeholder="placeholder"
                 :maxlength="maxLengthInternal.toString()"
                 :value="valueInternal"
                 @blur="props.blur"
                 @focus="props.onFocus"
                 @input="onInput"/>
        </div>

        <Error v-if="props.touched && props.validationMessage">{{ props.validationMessage }}</Error>
      </div>
    </template>
  </Field>
</template>

<style scoped>
.control-label {
  font-size: var(--font-size-default);
  font-weight: 500;
  line-height: 20px;
  letter-spacing: 1px;
}

.input-container {
  border: 1px solid #E4E4E4;
  border-radius: 10px;
}

.input {
  border-color: transparent;
  padding-left: 0;
}
</style>