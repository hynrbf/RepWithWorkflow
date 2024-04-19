<script lang="ts">
import { defineComponent, PropType } from "vue";
import { Affiliate } from "@/entities/Affiliate/Affiliate";
import { AffiliateDetails } from "@/entities/Affiliate/AffiliateDetails";

export default defineComponent({
  name: "ARAffiliatesCard",
  components: {},
  props: {
    affiliatesValues: {
      type: Array as PropType<Affiliate[]>,
      default: () => [],
    },
  },
  data() {
    return {};
  },
  created() {},
  watch: {},
  computed: {
    AffiliateDetails() {
      return AffiliateDetails;
    },
  },
  methods: {},
});
</script>

<template>
  <div class="row">
    <div
      v-for="item in affiliatesValues"
      :key="`card-${item.id}`"
      class="col-lg-4"
    >
      <KendoCard class="mb-3 cardview-style">
        <KendoCardBody>
          <DynamicAvatarComponent
            type="image"
            rounded="full"
            :text="`${item?.details.name} (${item?.details.firmReferenceNumber})`"
            :sub-text="`${item.representative?.forename} ${item.representative?.surname}`"
          >
            <template #text="{ text }">
              <strong>{{ text }}</strong>
            </template>
          </DynamicAvatarComponent>

          <StackLayout
            class="d-flex align-items-center mt-3"
            orientation="vertical"
          >
            <div class="pt-2 flex-grow-0 font-styling">Products</div>

            <div class="flex-grow-0 item-font-styling" id="cardview-products">
              {{ item?.products }}
            </div>
          </StackLayout>

          <div
            class="d-flex align-items-center justify-content-between gap-2 mt-3"
          >
            <KendoButton
              class="button-styling"
              type="button"
              fill-mode="outline"
              theme-color="primary"
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
                @click="$emit('view-affiliate', item.id)"
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
                @click="$emit('edit-affiliate', item.id)"
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
</style>