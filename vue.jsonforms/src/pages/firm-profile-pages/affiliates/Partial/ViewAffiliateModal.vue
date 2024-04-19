<script lang="ts">
import { Affiliate } from "@/entities/Affiliate/Affiliate";
import { container } from "tsyringe";
import { defineComponent } from "vue";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "ViewAffiliateModal",
  props: {
    affiliate: {
      type: Affiliate,
      default: () => [],
    },
  },
  data() {
    return {
      isShowTasksModal: false,
      fcaFirmRefNo: "",
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
    };
  },
  methods: {
    closeForm() {
      this.isShowTasksModal = false;
    },
    emitViewEditAffiliate(id: string) {
      this.$emit("view-edit-affiliate", id);
    },
    emitViewAffiliatePermissions(firmRef: string) {
      this.$emit("view-affiliate-permissions", firmRef);
    }
  },
});
</script>

<template>
  <ModalComponentFlexible ref="modalElement">
    <div class="d-flex flex-row vstack align-center">
      <text class="affiliate-name"> {{ affiliate.details?.name }}</text>
    </div>

    <div class="hstack align-center">
      <PillComponent :theme-color="'success-tint'" style="margin-top: 5px">
        {{ "Active" }}
        <!-- affiliate.status -->
      </PillComponent>
      <KendoButton
        type="button"
        size="small"
        rounded="full"
        shape="square"
        theme-color="light"
        title="Edit"
        class="ActionButton"
        @click="emitViewEditAffiliate(affiliate.id)"
      >
        <IconComponent symbol="edit-pen" size="20" />
      </KendoButton>
    </div>

    <Label class="flex-grow-1 section-title k-color-primary">{{
      $t("affiliatePage-formHeader")
    }}</Label>

    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width"> Provider Name </Label>

        <text class="another-text-style text-width"
          >{{ affiliate.details?.name }}
        </text>
      </div>

      <div class="vstack">
        <Label class="text-style text-width">Company Number</Label>
        <text class="another-text-style text-width">
          {{ affiliate?.details?.companyNumber }}
        </text>
      </div>

      <div class="vstack">
        <Label class="text-style text-width">Firm Reference Number</Label>

        <text class="another-text-style text-width"
          >{{ affiliate.details?.firmReferenceNumber }}
        </text>
      </div>

      <div class="vstack">
        <Label class="text-style text-width">PRA Authorised</Label>

        <text class="another-text-style text-width"
          >{{ affiliate.details.isPRAAuthorised !== null ? "Yes" : "No" }}
        </text>
      </div>
    </StackLayout>

    <StackLayout orientation="horizontal" class="mt-4">
      <PanelComponent bodyClass="!k-p-0">
        <StackLayout
          class="k-m-0 k-d-flex k-justify-items-center k-align-items-center k-py-1 k-px-3"
          orientation="horizontal"
          :gap="20"
          :align="{ horizontal: 'start' }"
        >
          <IconComponent symbol="account-setting-40" size="20" />
          <Label class="flex-grow-1 section-title k-color-primary">View Permissions</Label>
          <KendoButton
            type="button"
            size="small"
            rounded="full"
            shape="square"
            theme-color="light"
            title="Upload"
            class="ActionButton"
            @click="emitViewAffiliatePermissions(affiliate.details.firmReferenceNumber as string)"
          >
            <IconComponent symbol="download-file-41" size="20" />
          </KendoButton>
        </StackLayout>
      </PanelComponent>
    </StackLayout>

    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width">Registered Address</Label>

        <text class="another-text-style"
          >{{ affiliate?.details?.registeredAddress }}
        </text>
      </div>
    </StackLayout>

    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width">Trading Address</Label>

        <text class="another-text-style"
          >{{ affiliate?.details?.tradingAddress }}
        </text>
      </div>
    </StackLayout>
    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width"> Email Address </Label>

        <text class="another-text-style text-width"
          >{{ affiliate?.details?.emailAddress }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Contact Number</Label>

        <text class="another-text-style text-width"
          >{{ affiliate?.details?.contactNumberDisplay }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Website</Label>

        <text class="another-text-style text-width"
          >{{ affiliate?.details?.website }}
        </text>
      </div>
    </StackLayout>
    <div>
      <DividerComponent
        :height="1"
        :width="960"
        :spacing="0"
        style="margin-top: 20px; margin-bottom: 20px"
      />
    </div>

    <Label class="flex-grow-1 section-title k-color-primary">{{
      `${affiliate.details.name} ${$t("common-rep-label")}`
    }}</Label>
    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width">Title</Label>

        <text class="another-text-style text-width"
          >{{ affiliate?.representative?.title }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Forename(s)</Label>

        <text class="another-text-style text-width"
          >{{ affiliate?.representative?.forename }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Surname</Label>

        <text class="another-text-style text-width"
          >{{ affiliate?.representative?.surname }}
        </text>
      </div>
    </StackLayout>

    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width"> Email Address </Label>

        <text class="another-text-style text-width"
          >{{ affiliate?.representative?.emailAddress }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Contact Number</Label>

        <text class="another-text-style text-width"
          >{{ affiliate?.representative?.contactNumberDisplay }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Job Title</Label>

        <text class="another-text-style text-width"
          >{{ affiliate?.representative?.jobTitle }}
        </text>
      </div>
    </StackLayout>
  </ModalComponentFlexible>
</template>

<style scoped>
.affiliate-name {
  color: var(--color-black);
  text-align: center;
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-bold);
  font-style: normal;
  line-height: 28px;
}

.align-center {
  align-items: center;
  justify-content: center;
}

.text-width {
  width: 180px;
}

.text-style {
  color: var(--color-black);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-semi-bold);
  font-style: normal;
  line-height: 16px;
}

.another-text-style {
  color: var(--color-black);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-normal);
  font-style: normal;
  line-height: 17.5px;
  margin-top: 8px;
}
</style>