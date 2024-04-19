<script lang="ts">
import { IntroducersEntity } from "@/entities/providers-and-introducers/IntroducersEntity";
import { defineComponent } from "vue";

export default defineComponent({
  name: "IntroducerCard",
  props: {
    introducers: {
      type: Object as () => IntroducersEntity[],
      default: [],
    },
  },
});
</script>

<template>
  <div class="row mt-4">
    <div v-for="item in introducers" :key="`card-${item.id}`" class="col-lg-4">
      <KendoCard class="mb-3 cardview-style">
        <KendoCardBody>
          <DynamicAvatarComponent
            type="image"
            rounded="full"
            :text="`${item.details?.name} (${item.details?.fcaFirmRefNo})`"
            :sub-text="`${item.representative?.forename} ${item.representative?.surname}`"
          >
            <template #text="{ text }">
              <strong>{{ text }}</strong>
            </template>
          </DynamicAvatarComponent>

          <div class="d-flexjustify-content-between my-3">
            <StackLayout
              class="d-flex align-items-center"
              orientation="vertical"
            >
              <div class="pt-2 flex-grow-0 font-styling">Products</div>

              <div class="flex-grow-0 item-font-styling" id="cardview-email">
                Mortgage Broking, Insurance Broking
              </div>
            </StackLayout>
          </div>

          <div class="d-flex align-items-center justify-content-between gap-2">
            <!-- <KendoButton
              class="button-styling"
              type="button"
              fill-mode="outline"
              theme-color="primary"
              @click="$emit('view-tasks', item.details?.name, item.tasks)"
            >
              View Tasks
            </KendoButton> -->

            <div class="d-flex gap-2">
              <KendoButton
                type="button"
                size="small"
                rounded="full"
                shape="square"
                theme-color="light"
                title="View"
                class="ActionButton"
                @click="$emit('view-introducer', item.id)"
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
                @click="$emit('edit-introducer', item.id)"
              >
                <IconComponent symbol="edit-pen" size="20" />
              </KendoButton>
            </div>
          </div>
        </KendoCardBody>
      </KendoCard>
    </div>
  </div>
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
  box-shadow: 0px 1px 0px 0px #0073e6 inset;
}

.button-styling {
  height: 30px;
  padding: 4px 12px;
  border-radius: 100px;
  border: 1px solid var(--success-700, #309161);
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
