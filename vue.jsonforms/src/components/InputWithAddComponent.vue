<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  inheritAttrs: false,
  data() {
    return {
      value: "",
    };
  },
  methods: {
    add() {
      this.$emit("add", this.value);
      this.value = "";
    },
  },
});
</script>

<template>
  <div class="InputWithAdd">
    <KendoInput :value="value" @input="value = $event.value" v-bind="$attrs" />
    <button
      class="InputWithAdd-button"
      type="button"
      :disabled="!value"
      @click.prevent="add"
    >
      <IconComponent symbol="add-circle-27" size="20" />
    </button>
  </div>
</template>

<style scoped lang="scss">
.InputWithAdd {
  position: relative;

  &-button {
    background: none;
    border: none;
    padding: 0;
    margin: 0;
    position: absolute;
    top: 0;
    right: 0;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 5px 10px;
    color: var(--color-primary);
    cursor: pointer;
    z-index: 1;
  }
}
</style>