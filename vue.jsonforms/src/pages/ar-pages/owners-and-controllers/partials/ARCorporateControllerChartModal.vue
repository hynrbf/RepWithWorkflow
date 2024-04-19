<script lang="ts">
import OrganizationChart, {
  OrganizationChartNode,
} from "primevue/organizationchart";
import { defineComponent } from "vue";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { CorporateController } from "@/entities/owners-and-controllers/CorporateController";

export default defineComponent({
  name: "ARCorporateControllerChartModal",
  props: {
    registeredCustomer: {
      type: Object as () => CustomerEntity,
      default: new CustomerEntity(),
    },
  },
  data() {
    return {
      corporateControllers: this.registeredCustomer.corporateControllers,
      selection: {},
    };
  },
  components: {
    OrganizationChart,
  },
  computed: {
    transformedData(): OrganizationChartNode {
      if (!(this.registeredCustomer.noOfCorporateShareholders > 0)) {
        return { key: undefined };
      }

      const parent = this.corporateControllers[0];
      const children = this.corporateControllers.slice(1);

      return {
        ...this.mapCompanyToOrganizationChartNode(parent),
        children: children.map((child) =>
          this.mapCompanyToOrganizationChartNode(child),
        ),
      };
    },
  },
  methods: {
    mapCompanyToOrganizationChartNode(
      corporateController: CorporateController,
    ): OrganizationChartNode {
      return {
        key: corporateController.companyName,
        type: "firm",
        data: {
          name: corporateController.companyName,
          companyNo: corporateController.companyNumber,
          votingRights: corporateController.percentageOfVotingRights,
          ownership: corporateController.percentageOfCapital,
          directors: corporateController.directors.map(
            (d) => `${d.forename} ${d.surname}`,
          ),
        },
      };
    },
  },
});
</script>
<template>
  <ModalComponent
    ref="modalElement"
    width="1031"
    height="800"
    :title="'Owners & Controllers Structure Chart'"
    :isShowDownloadPDF="true"
  >
    <div class="d-flex flex-column" style="margin-top: 20px">
      <OrganizationChart
        v-model:selectionKeys="selection"
        :value="transformedData"
        collapsible
        selectionMode="multiple"
      >
        <template #firm="slotProps">
          <div class="node-content">
            <div class="avatar-circle-frame">
              <img
                :alt="'corporate-controller-chart-avatar'"
                :src="'/corporate-controller-chart-avatar.png'"
                class="node-avatar"
              />
            </div>

            <span class="node-name">{{ slotProps.node?.data.name }}</span>

            <span class="company-no-text">{{
              slotProps.node?.data.companyNo
            }}</span>

            <div
              class="d-flex"
              style="justify-content: start; margin: 10px 0 15px 0"
            >
              <div
                class="d-flex flex-column"
                style="width: 100px; height: 24px; margin-right: 30px"
              >
                <span class="text-value"
                  >{{ slotProps.node.data.votingRights }}%
                </span>

                <span class="text-label">Voting Rights</span>
              </div>

              <span
                style="
                  width: 0.5px;
                  height: 40px;
                  background: var(--text-text-disabled);
                "
              />

              <div
                class="d-flex flex-column"
                style="width: 100px; height: 24px; margin-left: 30px"
              >
                <span class="text-value"
                  >{{ slotProps.node.data.ownership }}%
                </span>

                <span class="text-label">Ownership</span>
              </div>
            </div>

            <div
              v-if="slotProps.node?.data.directors.length > 0"
              class="directors-section"
            >
              <div class="directors-section-title">
                <Label>Directors</Label>
              </div>

              <Label
                v-for="fullName of slotProps.node?.data.directors"
                class="directors-names"
                >{{ fullName }}
              </Label>
            </div>
          </div>
        </template>
      </OrganizationChart>
    </div>
  </ModalComponent>
</template>

<style scoped>
.node-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  height: auto;
  gap: 5px;
  width: 330px;
  padding: 20px 15px;
  border-radius: 8px;
  border: 0.5px solid var(--neutral-gray-300);
  background: var(--color-white);
}

.node-name {
  margin-top: 5px;
  color: var(--color-black);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semi-bold);
  line-height: 17.5px;
  text-align: center;
}

.company-no-text {
  color: var(--color-success-600);
  text-align: center;
  font-size: var(--font-size-xs);
  font-style: normal;
  font-weight: 400;
  line-height: 16px; /* 133.333% */
}

.text-value {
  color: var(--color-black);
  font-size: var(--font-size-default);
  font-style: normal;
  font-weight: var(--font-weight-bold);
  line-height: 20px;
}

.text-label {
  color: var(--color-content-50);
  text-align: center;
  font-size: var(--font-size-xs);
  font-style: normal;
  font-weight: var(--font-weight-medium);
  line-height: 16px;
}

.avatar-circle-frame {
  width: 35px;
  height: 35px;
  border: 1px solid var(--text-text-disabled);
  border-radius: 100%;
  position: relative;
}

.node-avatar {
  width: 18px;
  height: 18px;
  position: absolute;
  top: calc(50% - 9px);
  left: calc(50% - 9px);
}

.directors-section {
  display: flex;
  width: 300px;
  padding: 0 0 8px 0;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  background: var(--color-white);
  border-radius: 8px;
  border: 1px solid var(--text-text-disabled);
}

.directors-section-title {
  display: flex;
  height: 24px;
  padding: 0 15px;
  margin-bottom: 4px;
  justify-content: center;
  align-items: center;
  align-self: stretch;
  background-color: #e4e7ec;
  width: 100%;
  border-radius: 8px 8px 0 0;
  color: #000;
  text-align: center;
  font-size: var(--font-size-xs);
  font-style: normal;
  font-weight: 600;
  line-height: 16px; /* 133.333% */
}

.directors-names {
  margin: 4px 6px;
  color: var(--text-text-secondary);
  text-align: center;
  font-size: var(--font-size-xs);
  font-style: normal;
  font-weight: 400;
  line-height: 130%; /* 15.6px */
}

/* Organizational chart lines */
.p-organizationchart .p-organizationchart-line-down {
  background: var(--brand-color-brand-primary);
}

.p-organizationchart .p-organizationchart-line-top {
  border-top: 1px solid var(--brand-color-brand-primary);
  border-color: var(--brand-color-brand-primary);
}

.p-organizationchart .p-organizationchart-line-left {
  border-right: 1px solid var(--brand-color-brand-primary);
  border-color: var(--brand-color-brand-primary);
}

/* Down icon toggle to expand */
.p-organizationchart-node-content .p-node-toggler .p-node-toggler-icon {
  margin-bottom: 8px;
}

.p-organizationchart .p-organizationchart-node-content .p-node-toggler {
  background: var(--brand-color-brand-primary);
  color: var(--color-white);
  border-radius: 50%;
}
</style>
