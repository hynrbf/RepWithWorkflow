<script lang="ts">
import OrganizationChart, {
  OrganizationChartNode,
} from "primevue/organizationchart";
import { defineComponent } from "vue";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { IndividualController } from "@/entities/owners-and-controllers/IndividualController";

export default defineComponent({
  name: "IndividualControllerChartModal",
  props: {
    registeredCustomer: {
      type: Object as () => CustomerEntity,
      default: new CustomerEntity(),
    },
  },
  data() {
    return {
      individualControllers: this.registeredCustomer.individualControllers,
      selection: {},
    };
  },
  components: {
    OrganizationChart,
  },
  computed: {
    transformedData(): OrganizationChartNode {
      // make the zero index as parent while the rest are children from individualControllers
      if (!(this.individualControllers.length > 0)) {
        return {key: undefined};
      }

      const parent = this.individualControllers[0];
      const children = this.individualControllers.slice(1);
      return {
        ...this.mapEmployeeToOrganizationChartNode(parent),
        children: children.map((child) =>
          this.mapEmployeeToOrganizationChartNode(child),
        ),
      };
    },
  },
  methods: {
    mapEmployeeToOrganizationChartNode(
      individualController: IndividualController,
    ): OrganizationChartNode {
      return {
        // no id property in individualController
        // so the key will be the title for now
        key: individualController.detail.title,
        type: "person",
        data: {
          name: `${individualController.detail.forename} ${individualController.detail.surname}`,
          votingRights: `${individualController.detail.percentageOfVotingRights}`,
          ownership: `${individualController.detail.percentageOfCapital}`,
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
    height="600"
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
        <template #person="slotProps">
          <div class="node-content">
            <img
              :alt="'individual-avatar'"
              :src="'/individual-avatar.png'"
              class="node-avatar"
            />

            <span class="node-name">{{ slotProps.node.data.name }}</span>

            <div
              class="d-flex"
              style="justify-content: start; margin-top: 15px"
            >
              <div
                class="d-flex flex-column"
                style="width: 100px; height: 24px; margin-right: 30px"
              >
                <span class="text-value"
                  >{{ slotProps.node.data.votingRights }}
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
                  >{{ slotProps.node.data.ownership }}
                </span>

                <span class="text-label">Ownership</span>
              </div>
            </div>
          </div>
        </template>
      </OrganizationChart>
    </div>
  </ModalComponent>
</template>

<style>
.node-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  height: 153px;
  width: 330px;
  padding: 20px 15px;
  border-radius: 8px;
  border: 0.5px solid var(--neutral-gray-300);
  background: var(--color-white);
}

.node-avatar {
  width: 35px;
  height: 35px;
}

.node-name {
  margin-top: 5px;
  color: var(--color-black);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semi-bold);
  line-height: 17.5px;
  text-align: center;
}

.node-role {
  color: var(--content-content-05);
  font-size: var(--font-size-xs);
  font-style: normal;
  font-weight: var(--font-weight-normal);
  line-height: 16px;
  text-align: center;
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

.text-label {
  color: var(--color-content-50);
  text-align: center;
  font-size: var(--font-size-xs);
  font-style: normal;
  font-weight: var(--font-weight-medium);
  line-height: 16px;
}

.text-value {
  color: var(--color-black);
  font-size: var(--font-size-default);
  font-style: normal;
  font-weight: var(--font-weight-bold);
  line-height: 20px;
}
</style>