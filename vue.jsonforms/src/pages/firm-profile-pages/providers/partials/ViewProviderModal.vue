<script lang="ts">
import { ProvidersEntity } from "@/entities/providers-and-introducers/ProvidersEntity";
import { container } from "tsyringe";
import { defineComponent } from "vue";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";

export default defineComponent({
  name: "ProvidersList",
  props: {
    provider: {
      type: ProvidersEntity,
      default: () => [],
    },
  },
  data() {
    return {
      hasProviders: false,
      filter: null,
      isShowTasksModal: false,
      fcaFirmRefNo: "",
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
    };
  },
  methods: {
    closeForm() {
      this.isShowTasksModal = false;
    },
    emitViewEditProvider(id: string) {
      this.$emit("viewEditProvider", id);
    },
    emitViewProviderPermissions(firmRef: string) {
      this.$emit("view-provider-permissions", firmRef);
    }
  },
});
</script>

<template>
  <ModalComponentFlexible ref="modalElement">
    <div class="d-flex flex-row vstack align-center">
      <text class="provider-name"> {{ provider.details?.name }}</text>
    </div>

    <div class="hstack align-center">
      <PillComponent :themeColor="'success-tint'" style="margin-top: 5px">
        {{ "Active" }}
        <!-- provider.status -->
      </PillComponent>
      <KendoButton
        type="button"
        size="small"
        rounded="full"
        shape="square"
        theme-color="light"
        title="Edit"
        class="ActionButton"
        @click="emitViewEditProvider(provider.id)"
      >
        <IconComponent symbol="edit-pen" size="20" />
      </KendoButton>
    </div>

    <Label class="flex-grow-1 section-title k-color-primary">{{
      $t("providersPage-formHeader")
    }}</Label>

    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width"> Provider Name </Label>

        <text class="another-text-style text-width"
          >{{ provider.details?.name }}
        </text>
      </div>

      <div class="vstack">
        <Label class="text-style text-width">Company Number</Label>
        <text class="another-text-style text-width">
          {{ provider?.details?.companyNumber }}
        </text>
      </div>

      <div class="vstack">
        <Label class="text-style text-width">Firm Reference Number</Label>

        <text class="another-text-style text-width"
          >{{ provider.details?.fcaFirmRefNo }}
        </text>
      </div>

      <div class="vstack">
        <Label class="text-style text-width">PRA Authorised</Label>

        <text class="another-text-style text-width"
          >{{ provider.details.praAuthorised ? "Yes" : "No" }}
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
            @click="emitViewProviderPermissions(provider.details.fcaFirmRefNo as string)"
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
          >{{ provider?.details?.registeredAddress }}
        </text>
      </div>
    </StackLayout>

    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width">Trading Address</Label>

        <text class="another-text-style"
          >{{ provider?.details?.tradingAddress }}
        </text>
      </div>
    </StackLayout>
    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width"> Email Address </Label>

        <text class="another-text-style text-width"
          >{{ provider?.details?.emailAddress }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Contact Number</Label>

        <text class="another-text-style text-width"
          >{{ provider?.details?.contactNumberDisplay }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Website</Label>

        <text class="another-text-style text-width"
          >{{ provider?.details?.website }}
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
      $t("providersPage-providersRepresentativeText")
    }}</Label>
    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width">Title</Label>

        <text class="another-text-style text-width"
          >{{ provider?.representative?.title }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Forename(s)</Label>

        <text class="another-text-style text-width"
          >{{ provider?.representative?.forename }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Surname</Label>

        <text class="another-text-style text-width"
          >{{ provider?.representative?.surname }}
        </text>
      </div>
    </StackLayout>

    <StackLayout orientation="horizontal" class="mt-4">
      <div class="vstack">
        <Label class="text-style text-width"> Email Address </Label>

        <text class="another-text-style text-width"
          >{{ provider?.representative?.emailAddress }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Contact Number</Label>

        <text class="another-text-style text-width"
          >{{ provider?.representative?.contactNumberDisplay }}
        </text>
      </div>
      <div class="vstack">
        <Label class="text-style text-width">Job Title</Label>

        <text class="another-text-style text-width"
          >{{ provider?.representative?.jobTitle }}
        </text>
      </div>
    </StackLayout>
  </ModalComponentFlexible>
</template>

<style scoped>
.provider-name {
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