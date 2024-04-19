<script lang="ts">
import { StackLayout } from "@progress/kendo-vue-layout";
import { defineComponent } from "vue";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "KendoAccordionItemComponent",
  props: {
    title: {
      type: String,
      default: "",
    },
    isOpen: {
      type: Boolean,
      default: true,
    },
    isActive: {
      type: Boolean,
      default: false,
    },
  },
  components: { StackLayout },
  data() {
    return {
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      isOpenInternal: false,
      isActiveInternal: false,
    };
  },
  updated() {
    this.isActiveInternal = this.isActive;
  },
  mounted() {
    this.isOpenInternal = this.isOpen;
    this.isActiveInternal = this.isActive;

    //when accordion is open, make child accordion to fit all it's child elements
    if (!this.isOpenInternal) {
      return;
    }

    this.helperService.resizeExpander();
  },
  methods: {
    toggleSection() {
      this.isOpenInternal = !this.isOpenInternal;

      if (this.isOpenInternal) {
        this.helperService.resizeExpander();
      }

      this.$emit("onToggle", this.isOpenInternal);
    },
  },
});
</script>

<template>
  <StackLayout
    :style="`gap: ${isOpenInternal ? '20px' : '0px'}`"
    class="frame"
    :class="isActiveInternal ? 'active' : 'inactive'"
    orientation="vertical"
  >
    <StackLayout
      class="d-flex align-items-center"
      orientation="horizontal"
      @click="toggleSection"
      :align="{ horizontal: 'stretch' }"
    >
      <Label class="section-title k-color-primary">{{ title }}</Label>

      <div class="container-flex">
        <slot name="removing" class="flex-grow-0"></slot>

        <a href="#" class="p-2 flex-grow-0">
          <img
            :src="
              isOpenInternal
                ? '/accordion-arrow-up.svg'
                : '/accordion-arrow-down.svg'
            "
            alt="arrow"
          />
        </a>
      </div>
    </StackLayout>

    <slot name="header" v-if="isOpenInternal"></slot>

    <div v-show="isOpenInternal">
      <slot></slot>
    </div>
  </StackLayout>
</template>

<style scoped>
.inactive {
  border: 1.5px solid var(--text-text-disabled);
}

.active {
  border: 1.5px solid var(--color-primary);
}

.frame {
  padding: 20px;
  border-radius: 8px;
  background-color: white;
  gap: 20px;
}

.container-flex {
  display: flex;
  width: 100%;
  justify-content: flex-end;
  align-items: flex-end;
}
</style>