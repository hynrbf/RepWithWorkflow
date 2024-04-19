<script lang="ts">
import {defineComponent} from "vue";
import {Label, Hint, Error} from "@progress/kendo-vue-labels";
import {Input} from "@progress/kendo-vue-inputs";
import {Field} from "@progress/kendo-vue-form";

export default defineComponent({
  name: "KendoEmailInputFormElement",
  components: {
    Field,
    Label,
    Error,
    Hint,
    Input
  },
  props: {
    disabled: Boolean,
    touched: Boolean,
    label: String,
    validationMessage: String,
    name: {
      type:String,
      default: ''
    },
    id: String,
    value: {
      type: String,
      default: ''
    },
    // custom property
    isRequired: Boolean,
    placeholder: {
      type: String,
      default: 'Type...'
    },
    maxLength: {
      type: Number,
      default: 100
    }
  },
  methods: {
    onInput(event: any) {
      this.$emit('onValueChange', event.target.value);
    },
    onBlur(event: any) {
      this.$emit('blur', event);
    },
    handleFocus(event: any) {
      this.$emit('focus', event);
    },
    validate(value: string): string {
      const emailRegex: RegExp = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return emailRegex.test(value) ? "" : `Please enter a valid ${this.label}`;
    }
  }
});

</script>

<template>
  <Field :name="name"
         :id="id"
         :type="'email'"
         :validator="validate"
         :component="'myTemplate'">
    <template v-slot:myTemplate="{props}">
      {{props}}
      <div class="d-flex flex-column gap-2">
        <Label :editor-valid="props.valid"
               :editor-id="props.id"
               class="control-label">{{label}}<span v-if="isRequired" style="color:red;">*</span>
        </Label>

        <Input v-bind="props"
               :id="props.id"
               :valid="props.valid"
               :maxlength="maxLength.toString()"
               :placeholder="placeholder"
               @input="onInput"
               @blur="props.blur"
               @focus="props.onFocus"/>

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
</style>