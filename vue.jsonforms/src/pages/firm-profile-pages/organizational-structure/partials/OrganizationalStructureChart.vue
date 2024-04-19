<script lang="ts">
import { defineComponent } from "vue";
import { Employee } from "@/entities/firm-details/Employee";
import OrganizationChart, {
  OrganizationChartNode,
} from "primevue/organizationchart";

export default defineComponent({
  name: "OrganizationalStructureChart",
  props: {
    employeesValues: {
      type: Object as () => Employee[],
      default: [] as Employee[],
    },
  },
  data() {
    return {
      employees: this.employeesValues,
      selection: {},
    };
  },
  components: {
    OrganizationChart,
  },
  computed: {
    transformedData(): OrganizationChartNode {
      const rootEmployeeRole = [
        "Director (SMF3)".toLowerCase(),
        "Director (SMF16)".toLowerCase(),
      ];
      this.employees.forEach(
        (employee) =>
          (employee.isRoot = rootEmployeeRole?.includes(
            employee.primaryRole?.name?.toLowerCase() ?? ""
          ))
      );

      const roots = this.employees.filter((e) => e.isRoot) as Employee[];
      const rootNodes = [] as OrganizationChartNode[];

      for (const employee of roots) {
        rootNodes.push(this.mapEmployeeToOrganizationChartNode(employee));
      }

      return {
        key: "absolute-root",
        styleClass: "absolute-root",
        type: "",
        data: {},
        children: rootNodes,
      } as OrganizationChartNode;
    },

    unAssignedEmployees(): OrganizationChartNode[] {
      const roots = this.employees.filter(
        (e) => !e.lineManager && !e.isRoot
      ) as Employee[];
      const rootNodes = [] as OrganizationChartNode[];

      for (const employee of roots) {
        rootNodes.push(this.mapEmployeeToOrganizationChartNode(employee));
      }

      return rootNodes;
    },
  },
  mounted() {
    this.setupChartLineStylesForMultipleRoot();
  },
  updated() {
    let allElements = document.querySelectorAll("tr.p-organizationchart-lines");

    allElements.forEach(function (element) {
      const htmlElement = element as HTMLElement;

      if (!htmlElement) {
        return;
      }

      const visibility = window.getComputedStyle(htmlElement).visibility;

      if (visibility === "hidden") {
        htmlElement.classList.add("hide-all-child");
      } else {
        htmlElement.classList.remove("hide-all-child");
      }
    });
  },
  methods: {
    mapEmployeeToOrganizationChartNode(
      employee: Employee
    ): OrganizationChartNode {
      let name = "";

      if (employee.title) {
        name += `${employee.title} `;
      }

      if (employee.firstName) {
        name += `${employee.firstName} `;
      }

      if (employee.lastName) {
        name += employee.lastName;
      }

      return {
        key: employee.id,
        type: "person",
        data: {
          // use the image hardcoded for now
          image:
            "https://primefaces.org/cdn/primevue/images/avatar/amyelsner.png",
          name: name,
          role: employee.primaryRole?.name,
          status: employee.employmentStatus,
        },
        styleClass: employee.employmentStatus,
        children: this.employees
          .filter((e) => e.lineManager?.id === employee.id)
          .map((child) => this.mapEmployeeToOrganizationChartNode(child)),
      };
    },

    setupChartLineStylesForMultipleRoot() {
      let absoluteRoot = document.querySelector(
        ".p-organizationchart-table:has(.absolute-root)"
      );

      if (!absoluteRoot) {
        return;
      }

      const absoluteRootChildrenNode =
        absoluteRoot.children[0]?.children[3]?.children;

      if (!absoluteRootChildrenNode) {
        return;
      }

      const parents = Array.from(absoluteRootChildrenNode);

      for (const parent of parents) {
        if (!parent) {
          continue;
        }

        this.setLinesStyleRecursive(parent);
      }
    },

    setLinesStyleRecursive(parent: Element) {
      const table = parent.querySelector(".p-organizationchart-table");

      if (!table) {
        return;
      }

      const parentChildrenNodes = table.children[0]?.children[3]?.children;

      if (!parentChildrenNodes?.length) {
        return;
      }

      const children = Array.from(parentChildrenNodes);

      if (!children) {
        return;
      }

      const lines = table.children[0]?.children[2]?.children;

      if (!lines?.length) {
        return;
      }

      let lineIndex = 0;

      for (const data of children) {
        const currentNode = data.querySelector(
          ".p-organizationchart-table .node-content"
        );

        if (!currentNode) {
          continue;
        }

        const classList = Array.from(currentNode.classList);
        const status = classList.filter((c) => c !== "node-content")[0];

        if (lines.length < 2) {
          const lineDown = lines[lineIndex].children[0] as HTMLElement;
          lineDown.style.background = this.getStatusColor(status);
          lineDown.style.width = "1px";
          continue;
        }

        this.setLineStyle(
          lineIndex,
          lines[lineIndex] as HTMLElement,
          lines[lineIndex + 1] as HTMLElement,
          status,
          lines.length
        );
        lineIndex += 2;
      }

      for (const child of children) {
        if (!child) {
          continue;
        }

        this.setLinesStyleRecursive(child);
      }
    },

    setLineStyle(
      currentLineIndex: number,
      leftLine: HTMLElement,
      rightLine: HTMLElement,
      status: string,
      linesCount: number
    ) {
      const color = this.getStatusColor(status);
      leftLine.style.borderRightColor = color;

      if (currentLineIndex > 0) {
        leftLine.style.borderTopColor = color;
      }

      if (currentLineIndex < linesCount - 2) {
        rightLine.style.borderColor = `${color} transparent transparent transparent`;
      }
    },

    getStatusColor(status: string): string {
      const activeGreen = "var(--color-success-600)";
      const onboardingOrange = "var(--color-warning-600)";
      const redTerminated = "var(--color-error-600)";
      return status === "Active"
        ? activeGreen
        : status === "Onboarding"
        ? onboardingOrange
        : redTerminated;
    },

    formatRole(value: string, substrNum: number) {
      return value.length > substrNum
        ? value.substring(0, substrNum) + "..."
        : value;
    },
  },
});
</script>

<template>
  <div class="d-flex flex-column">
    <OrganizationChart
      v-model:selectionKeys="selection"
      :value="transformedData"
      collapsible
      selectionMode="multiple"
    >
      <template #person="slotProps">
        <div class="node-content" :class="`${slotProps.node.data.status}`">
          <img
            :alt="slotProps.node.data.name"
            :src="slotProps.node.data.image"
            class="node-avatar"
          />

          <span class="node-name">{{ slotProps.node.data.name }}</span>

          <template v-if="slotProps.node.data.role.length > 20">
            <span class="node-role is-clickable">
              {{ formatRole(slotProps.node.data.role, 20) }}
            </span>
            <PillComponent
              :text="slotProps.node.data.role"
              class="node-role-tooltip is-zindex-base"
            />
          </template>
          <span v-else class="node-role">{{ slotProps.node.data.role }}</span>
        </div>
      </template>
    </OrganizationChart>

    <!-- OrganizationChart for unassigned employees -->
    <label class="unassigned-text">Unassigned</label>

    <div class="orphans-section">
      <div
        class="my-2"
        v-for="(orphans, index) of unAssignedEmployees"
        :key="index"
      >
        <OrganizationChart :value="orphans">
          <template #person="slotProps">
            <div class="node-content" :class="`${slotProps.node.data.status}`">
              <img
                :alt="slotProps.node.data.name"
                :src="slotProps.node.data.image"
                class="node-avatar"
              />

              <span class="node-name">{{ slotProps.node.data.name }}</span>

              <template v-if="slotProps.node.data.role.length > 20">
                <span class="node-role is-clickable">
                  {{ formatRole(slotProps.node.data.role, 20) }}
                </span>
                <PillComponent
                  :text="slotProps.node.data.role"
                  class="node-role-tooltip is-zindex-base"
                />
              </template>
              <span v-else class="node-role">{{
                slotProps.node.data.role
              }}</span>
            </div>
          </template>
        </OrganizationChart>
      </div>
    </div>
  </div>
</template>

<!--intentionally not scoped to apply the styles globally with this name of css class-->
<style>
.unassigned-text {
  margin-left: 26px;
  margin-bottom: 14px;
  margin-top: 14px;

  color: var(--brand-color-brand-primary);
  font-size: var(--font-size-default);
  font-weight: var(--font-weight-semi-bold);
}

.p-organizationchart .node-content {
  display: inline-flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: 99px;
  width: 169px;
  padding: 20px 15px;
  border-radius: 8px;
  border: 0.5px solid var(--neutral-gray-300);
  background: var(--color-white);
}

.p-organizationchart .node-avatar {
  width: 35px;
  height: 35px;
}

.p-organizationchart .node-name {
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

.node-role:hover ~ .node-role-tooltip {
  display: block;
}

.p-organizationchart .absolute-root {
  display: none;
}

/* This style hides the absolute root lines */
div > .p-organizationchart-table > tbody > tr.p-organizationchart-lines * {
  display: none;
}

/* Down icon toggle to expand */
.p-organizationchart-node-content .p-node-toggler .p-node-toggler-icon {
  margin-bottom: 8px;
}

/* Organizational chart lines */
.p-organizationchart .p-organizationchart-line-down {
  background: var(--color-success-600);
  width: 1px;
}

.p-organizationchart .p-organizationchart-node-content .p-node-toggler {
  background: var(--brand-color-brand-primary);
  color: var(--color-white);
  border-radius: 50%;
}

.p-organizationchart .p-organizationchart-line-left {
  border: 1px solid transparent;
}

.p-organizationchart .p-organizationchart-line-right {
  border: 1px solid transparent;
}

.hide-all-child * {
  display: none;
}

.orphans-section {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
}

.node-role-tooltip {
  position: fixed;
  background-color: var(--neutral-gray-300);
  color: var(--color-black);
  display: none;
}

.node-role-tooltip::after {
  content: "";
  width: 0;
  height: 0;
  border-left: 10px solid transparent;
  border-right: 10px solid transparent;
  border-top: 10px solid var(--neutral-gray-300);
  position: absolute;
  bottom: -10px;
  left: 50%;
  margin-left: -10px;
}
</style>
