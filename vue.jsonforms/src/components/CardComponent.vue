<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "CardComponent",
  props: {
    cardId: {
      type: String,
      default: "",
    },
    cardName: {
      type: String,
      default: "",
    },
    cardfcaFirmRefNo: {
      type: String,
      default: "",
    },
    cardTasks: {
      type: Array,
      default: [],
    },
    cardIndex: {
      type: Number,
      default: 0,
    },
  },
  data() {
    return {
      colors: [
        "red",
        "blue",
        "green",
        "orange",
        "purple",
        "magenta",
        "teal",
        "navy",
        "indigo",
        "maroon",
        "violet",
      ],
    };
  },
  mounted() {},
  created() {},
  methods: {
    getColor(index: number) {
      return this.colors[index % this.colors.length];
    },
  },
});
</script>

<template>
  <KendoCard
    class="cardview-style"
    :style="{ boxShadow: `0px 1px 0px 0px ${getColor(cardIndex)} inset` }"
  >
    <KendoCardBody>
      <DynamicAvatarComponent
        type="image"
        rounded="full"
        :text="`${cardName} (${cardfcaFirmRefNo})`"
        :sub-text="`${cardName}`"
      >
        <template #text="{ text }">
          <strong>{{ text }}</strong>
        </template>
      </DynamicAvatarComponent>

      <div class="d-flex justify-content-center my-3">
        <StackLayout class="d-flex align-items-center" orientation="vertical">
          <div class="pt-2 flex-grow-0 font-styling">Products</div>

          <div class="flex-grow-0 item-font-styling" id="cardview-email">
            Mortgage Broking, Insurance Broking
          </div>
        </StackLayout>
      </div>

      <div class="d-flex align-items-center justify-content-between gap-2">
        <KendoButton
          class="button-styling"
          type="button"
          fill-mode="outline"
          :style="{ border: `1px solid ${getColor(cardIndex)}` }"
          @click="$emit('viewTasks', cardName, cardTasks)"
        >
          View Tasks
        </KendoButton>

        <div class="d-flex gap-2">
          <KendoButton
            type="button"
            size="small"
            rounded="full"
            shape="square"
            theme-color="light"
            title="View"
            class="ActionButton"
            @click="$emit('viewProvider', cardId)"
          >
            <IconComponent symbol="eye" size="20" />
          </KendoButton>

          <KendoButton
            type="button"
            size="small"
            rounded="full"
            shape="square"
            theme-color="light"
            title="Pen"
            class="ActionButton"
            @click="$emit('editProvider', cardId)"
          >
            <IconComponent symbol="edit-pen" size="20" />
          </KendoButton>
        </div>
      </div>
    </KendoCardBody>
  </KendoCard>
</template>

<style scoped lang="scss">
.k-card-body {
  padding-block: 20px;
  padding-inline: 15px;
}

.cardview-style {
  width: 346.5px;
  border-radius: 8px;
  border: 0.5px solid var(--content-content-07);
}

.button-styling {
  height: 30px;
  padding: 4px 12px;
  border-radius: 100px;
  width: 100%;
}

.font-styling {
  font-size: var(--font-size-xs);
  font-style: normal;
}

.item-font-styling {
  font-size: var(--font-size-sm);
  font-style: normal;
  text-decoration-line: underline;
  color: var(--brand-color-brand-primary);

  &#cardview-contact {
    text-decoration-line: none !important;
  }
}
</style>