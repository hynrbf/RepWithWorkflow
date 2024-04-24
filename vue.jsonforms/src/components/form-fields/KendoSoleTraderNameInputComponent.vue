<script lang="ts">
import { defineComponent } from "vue";
import { CompanyEntity } from "@/entities/CompanyEntity";
import { container } from "tsyringe";
import {
  IFcaService,
  IFcaServiceInfo,
} from "@/infra/dependency-services/rest/fca/IFcaService";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";

export default defineComponent({
  name: "KendoSoleTraderNameInputComponent",
  props: {
    id: String,
    isInitializing: {
      type: Boolean,
      default: false
    },
    firstName: {
      type: String,
      default: "",
    },
    lastName: {
      type: String,
      default: "",
    },
    firstNameLabel: {
      type: String,
      default: "Forename(s)",
    },
    lastNameLabel: {
      type: String,
      default: "Last Name",
    },
    gap: {
      type: Number,
      default: 8,
    },
  },
  data() {
    return {
      fcaService: container.resolve<IFcaService>(IFcaServiceInfo.name),
      firstNameInternal: "",
      lastNameInternal: "",
      isLoadingFcaDetails: false,
      companies: [] as CompanyEntity[],
      foundCompany: undefined as CompanyEntity | undefined,
      isSeekingToVaryPermission: false,
      debounceTimer: null as NodeJS.Timeout | null,
      debounceTimerInMs: 500,
      controlParentContainerId: "",
      iconRight: "",
    };
  },
  mounted() {
    this.firstNameInternal = this.firstName;
    this.lastNameInternal = this.lastName;
  },
  computed: {
    fullName(): string {
      let fullName = "";
      if (
        this.firstNameInternal.length > 0 &&
        this.lastNameInternal.length > 0
      ) {
        fullName = `${this.firstNameInternal} ${this.lastNameInternal}`;
      }

      this.$emit("onFullNameChange", [
        this.firstNameInternal,
        this.lastNameInternal,
      ]);
      return fullName;
    },
    isTriggerSearch(): boolean {
      return (
        this.fullName.length > 0 &&
        this.firstNameInternal.length > 2 &&
        this.lastNameInternal.length > 2
      );
    },
  },
  methods: {
    onFirstNameChange(value: string) {
      this.firstNameInternal = value;

      if (!this.isTriggerSearch) {
        return;
      }
      if (this.debounceTimer) {
        clearTimeout(this.debounceTimer);
      }

      this.debounceTimer = setTimeout(async () => {
        await this.searchSoleTraderAsync();
        this.getIconAndSetControlFirmStatus();
      }, this.debounceTimerInMs);
    },

    onLastNameChange(value: string) {
      this.lastNameInternal = value;

      if (!this.isTriggerSearch) {
        return;
      }

      if (this.debounceTimer) {
        clearTimeout(this.debounceTimer);
      }

      this.debounceTimer = setTimeout(async () => {
        await this.searchSoleTraderAsync();
        this.getIconAndSetControlFirmStatus();
      }, this.debounceTimerInMs);
    },

    async searchSoleTraderAsync() {
      this.companies =
        (await this.displayMatchedFirmsAsync(this.fullName, true)) ?? [];
      if (!(this.companies && this.companies.length > 0)) {
        // if no results
        let company: CompanyEntity = {
          companyName: this.fullName,
          address: "",
          tradingAddress: "",
          companyNumber: "Not Applicable",
          firmReferenceNo: "Not Applicable",
          isAuthorized: false,
          isConfirmedFirmDetails: false,
          isSelected: true,
          isVariedFirmPermissions: false,
          postcode: "Not Available",
          status: "Not Authorised",
          companyHouseStatus: "",
          type: "Firm",
          region: "",
          isSoleTrader: true,
          appointedRepresentatives: [] as AppointedRepresentative[],
          contactNumber: "",
          website: "",
          sicCode: "",
          countryCode: "",
        };

        this.companies?.push(company);
      }

      // TODO. how can I pick the specific sole trader if multiple results?
      // TEMP. Use first of default for now
      this.companies[0].isSelected = true;
      let selectedCompany = this.companies.find((firm) => firm.isSelected);

      if (selectedCompany) {
        this.foundCompany = selectedCompany;
        this.isSeekingToVaryPermission =
          this.foundCompany.isVariedFirmPermissions;
      } else {
        this.isSeekingToVaryPermission = false;
      }

      this.$emit("onFinishedSearch", this.foundCompany);
    },

    async displayMatchedFirmsAsync(
      keyword: string,
      isSoleTrader: boolean,
      companyNumber: string = "",
      companyAddress = "",
      fromLocalCache: boolean = false,
    ): Promise<CompanyEntity[] | undefined> {
      if (keyword?.length < 4) {
        return;
      }

      this.isLoadingFcaDetails = true;
      return await this.fcaService
        .getMatchedFirms(
          keyword,
          isSoleTrader,
          companyNumber,
          companyAddress,
          fromLocalCache,
        )
        .catch((error) => {
          throw new Error(error.message);
        })
        .finally(() => {
          this.isLoadingFcaDetails = false;
        });
    },

    getIconAndSetControlFirmStatus() {
      if (!this.foundCompany) {
        this.controlParentContainerId = "";
        this.iconRight = "";
        return;
      }

      this.controlParentContainerId = this.foundCompany.isAuthorized
        ? "soletrader-name-input-authorised"
        : "";
      this.iconRight = this.foundCompany.isAuthorized
        ? "/check-filled.svg"
        : "";
    },

    resetIconAndControlFirmStatusIndicator() {
      this.iconRight = "";
      this.controlParentContainerId = "";
    },
  },
});
</script>

<template>
  <StackLayout
    :id="controlParentContainerId"
    orientation="horizontal"
    :gap="gap"
    :align="{ horizontal: 'stretch' }"
  >
    <StackLayout orientation="vertical">
      <KendoInputWithCustomIconComponent
        :id="id + 'forename'"
        :name="id + 'forename'"
        placeholder="John"
        :label="firstNameLabel"
        :isCapitalizeFirstLetter="true"
        :value="firstNameInternal"
        :iconPathRight="iconRight"
        @onValueChange="onFirstNameChange"
        :isValueReactive="true"
        :isDataLoadedCompletely="!isInitializing"
      />

      <StackLayout
        v-if="isLoadingFcaDetails"
        class="mt-2 caption-figtree-medium"
      >
        <!-- TODO later -->
        Searching...
      </StackLayout>

      <Label
        v-if="!isLoadingFcaDetails && foundCompany"
        class="mt-2 caption-figtree-medium"
      >
        Firm Reference Number:&nbsp;<span class="caption-frn"
          >{{ foundCompany.firmReferenceNo }}
        </span></Label
      >
    </StackLayout>

    <KendoInputWithCustomIconComponent
      :id="id + 'lastname'"
      :name="id + 'lastname'"
      placeholder="Smith"
      :label="lastNameLabel"
      :isCapitalizeFirstLetter="true"
      :value="lastNameInternal"
      :iconPathRight="iconRight"
      @onValueChange="onLastNameChange"
      :isValueReactive="true"
      :isDataLoadedCompletely="!isInitializing"
    />
  </StackLayout>
</template>

<style scoped />