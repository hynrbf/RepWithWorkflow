<script lang="ts">
import { defineAsyncComponent, defineComponent, inject } from "vue";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import { Employee } from "@/entities/firm-details/Employee";
import {
  IOrganizationalStructureService,
  IOrganizationalStructureServiceInfo,
} from "@/infra/dependency-services/rest/organizational-structure/IOrganizationalStructureService";
import { container } from "tsyringe";
import { v4 as uuidv4 } from "uuid";
import { ContactNumber } from "@/entities/ContactNumber";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import { useAlert } from "@/composables/useAlert";
import { usePageLifeCycleStore } from "@/stores/progress-bar/usePageLifeCycleStore";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import {
  NotificationType,
  useNotification,
} from "@/composables/useNotification";
import StaticList from "@/infra/StaticListService";
import { MultiSelectTree } from "@progress/kendo-vue-dropdowns";
import DataItemModel from "@/components/models/DataItemModel";
import PillComponent from "@/components/PillComponent.vue";
import { useOrganizationalStructureStore } from "@/stores/organizational-structure/useOrganizationalStructureStore";
import { GridPdfExport } from "@progress/kendo-vue-pdf";
import {ProfileStatuses} from "@/entities/enums/ProfileStatuses";

export default defineComponent({
  name: "AROrganizationalStructure",
  components: {
    PillComponent,
    MultiSelectTree,
    AROrganizationalStructureList: defineAsyncComponent(
      () => import("./partials/AROrganizationalStructureList.vue"),
    ),
    AROrganizationalStructureCard: defineAsyncComponent(
      () => import("./partials/AROrganizationalStructureCard.vue"),
    ),
    AROrganizationalStructureChart: defineAsyncComponent(
      () => import("./partials/AROrganizationalStructureChart.vue"),
    ),
    ARAddStaffModal: defineAsyncComponent(
      () => import("./partials/ARAddStaffModal.vue"),
    ),
    pdfexport: GridPdfExport,
  },
  data() {
    return {
      isComplete: false,
      eventBus: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      isShowOrganizationalStructureList: false,
      isShowOrganizationalStructureCard: false,
      isShowOrganizationalStructureChart: false,
      searchNameEmailAddress: "",
      currentTab: 0,
      tabs: [
        { title: "Current", content: "current" },
        { title: "All", content: "all" },
      ],
      organizationalStructureService:
        container.resolve<IOrganizationalStructureService>(
          IOrganizationalStructureServiceInfo.name,
        ),
      appService: container.resolve<IAppService>(IAppServiceInfo.name),
      isStaffModalVisible: false,
      isFormSaving: false,
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name,
        ),
      isSavingAlertOpened: false,
      isFilterOpen: false,
      selectedEmployee: "",
      currentEmployee: null as Employee | null,
      employeesFiltered: [] as Employee[],
      employeesSuggestions: null as Employee[] | null,
      selectedStatusForFilter: null as DataItemModel | null,
      selectedRolesForFilter: null as DataItemModel[] | null,
      hasSearchSelection: false,
      isEdit: false,
      columnsToPDF: [],
      itemsToPDF: [],
      isInitializing: true
    };
  },
  computed: {
    AppConstants() {
      return AppConstants;
    },
    roles() {
      const roles = StaticList.getRoles();

      if (!roles?.length) {
        return [] as DataItemModel[];
      }

      return roles.map((role) => {
        return {
          label: role,
          value: role,
        } as DataItemModel;
      });
    },
    filterCount() {
      if (this.selectedRolesForFilter && this.selectedStatusForFilter) {
        return "(2)";
      }

      return "";
    },
    employees(): Employee[] {
      return this.getEmployees();
    },
    companyNo(): string {
      const cachedCustomer =
        this.appService.getCachedCustomerAppointedRepresentative();
      return cachedCustomer?.companyNumber ?? "";
    },
  },
  setup() {
    const pageLifeCycleStore = usePageLifeCycleStore();
    const { changeLifeCycleName } = pageLifeCycleStore;

    const organizationalStructureStore = useOrganizationalStructureStore();
    const { fetchInitialEmployeesAsync, getEmployees } =
      organizationalStructureStore;

    return {
      changeLifeCycleName,
      fetchInitialEmployeesAsync,
      getEmployees,
    };
  },
  created() {
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameCreated);
    this.eventBus.on(AppConstants.formFieldChangedEvent, () => {
      this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
        false;
    });
  },
  async mounted() {
    this.eventBus.emit(AppConstants.bottomBarEnableEvent, true);
    this.eventBusFormSaved.on(AppConstants.formSavedEvent, (isAutoNext) =>
      this.handleSubmit(isAutoNext),
    );
    await this.fetchInitialEmployeesAsync(this.companyNo);
    this.reloadEmployees();

    this.isShowOrganizationalStructureList = true;
    this.changeLifeCycleName(AppConstants.pageLifeCycleNameMounted);
    this.isInitializing = false;
  },
  unmounted() {
    this.eventBusFormSaved.off(AppConstants.formSavedEvent);
  },
  methods: {
    reloadEmployees() {
      if (this.employees?.length) {
        this.employeesFiltered = [...this.employees];
        this.employeesSuggestions = [...this.employees];
      }
    },

    toggleOrganizationalStructureList() {
      this.isShowOrganizationalStructureList = true;
      this.isShowOrganizationalStructureCard = false;
      this.isShowOrganizationalStructureChart = false;
    },

    toggleOrganizationalStructureCard() {
      this.isShowOrganizationalStructureCard = true;
      this.isShowOrganizationalStructureList = false;
      this.isShowOrganizationalStructureChart = false;
    },

    toggleOrganizationalStructureChart() {
      this.isShowOrganizationalStructureChart = true;
      this.isShowOrganizationalStructureList = false;
      this.isShowOrganizationalStructureCard = false;
    },

    onSelectAsync(event: any) {
      if (event.type === "select") {
        return;
      }

      const employee = event.target.value as Employee;

      if (!employee || !employee.firstName || !employee.lastName) {
        this.selectedEmployee = "";
        this.hasSearchSelection = false;
        return;
      }

      this.selectedEmployee = `${employee.firstName} ${employee.lastName}`;
      this.employeesFiltered = this.employees.filter(
        (emp) => emp.id === employee?.id,
      );
      this.hasSearchSelection = true;
    },

    addExport() {
      (this.$refs.gridPdfExport as any).save(this.itemsToPDF);
    },

    onInput(event: any) {
      const searchValue = event.target.value;
      this.selectedEmployee = searchValue;

      if (!searchValue) {
        this.employeesSuggestions = [...this.employees];
        this.employeesFiltered = [...this.employees];
        return;
      }

      this.employeesSuggestions = this.employees.filter(
        (e) =>
          e.email?.includes(searchValue) ||
          e.firstName?.includes(searchValue) ||
          e.lastName?.includes(searchValue),
      );
    },

    async saveOrUpdateOrgStructureEmployee(
      updatedEmployee: Employee,
    ): Promise<void> {
      if (!updatedEmployee || this.isSavingAlertOpened) {
        return;
      }

      useAlert({
        title: this.$t("common-alert-title"),
        content: this.$t("organizationStructurePage-alert-content"),
        confirmButtonText: this.$t("common-alert-buttonText"),
        onConfirm: async () => {
          const employee: Employee = {
            affiliates: [],
            closeLinks: [],
            corporateControllers: [],
            employersLiabilities: [],
            financialPromotions: [],
            firmProfileEditStatus: "",
            individualControllers: [],
            isAuthorised: false,
            isCompanyNotApplicable: false,
            isFinishedSignUp: false,
            isUserPasswordSet: false,
            mediaMarketingOutlets: [],
            noOfCorporateShareholders: 0,
            noOfIndividualShareholders: 0,
            professionalIndemnities: [],
            profileStatus: ProfileStatuses.Basic.toString(),
            stationeries: [],
            id: updatedEmployee.id ?? uuidv4(),
            title: updatedEmployee.title ?? "",
            firstName: updatedEmployee.firstName ?? "",
            lastName: updatedEmployee.lastName ?? "",
            lineManager: updatedEmployee.lineManager,
            primaryRole: updatedEmployee.primaryRole,
            otherRoles: updatedEmployee.otherRoles,
            productType: updatedEmployee.productType ?? [],
            email: updatedEmployee.email ?? "",
            contactNumber: updatedEmployee.contactNumber ?? new ContactNumber(),
            companyNo: this.companyNo,
            isRoot: updatedEmployee.isRoot ?? false,
            employmentStatus: updatedEmployee.employmentStatus,
            tasks: updatedEmployee.tasks
          };

          this.isFormSaving = true;
          this.isSavingAlertOpened = false;
          console.log(employee);
          await this.organizationalStructureService
            .saveOrUpdateEmployeeAsync(employee)
            .then(async () => {
              await this.fetchInitialEmployeesAsync(this.companyNo);
              this.reloadEmployees();

              this.isStaffModalVisible = false;
              this.isFormSaving = false;
              this.currentEmployee = null;

              useNotification({
                type: NotificationType.SUCCESS,
                content: this.$t("organizationStructurePage-changes-saved"),
                interval: AppConstants.notificationPopupTimeOut,
              });
            });
        },
        onCancel: () => {
          this.isSavingAlertOpened = false;
        },
        onClose: () => {
          this.isSavingAlertOpened = false;
        },
      });

      this.isSavingAlertOpened = true;
    },

    handleEditEmployee(employee: Employee): void {
      if (!employee) {
        return;
      }

      this.currentEmployee = employee;
      this.isStaffModalVisible = true;
      this.isEdit = true;
    },

    handleAddEmployee() {
      this.currentEmployee = new Employee();
      this.isStaffModalVisible = true;
      this.isEdit = false;
    },

    handleExportingPDF(columnsToPDF: [], itemsToPDF: []) {
      this.columnsToPDF = columnsToPDF;
      this.itemsToPDF = itemsToPDF;
    },

    selectTab(event: any) {
      this.currentTab = event.selected;
    },

    getIconColor(isSelected: boolean): string {
      return isSelected
        ? "var(--brand-color-brand-primary)"
        : "var(--text-text-disabled)";
    },

    getStatuses() {
      return StaticList.getStatuses().map((type) => ({
        label: type,
        value: type,
      }));
    },

    handleStatusChange(selectedStatusItem: DataItemModel) {
      this.selectedStatusForFilter = selectedStatusItem;
    },

    handleRolesChange(selectedRoles: DataItemModel[]) {
      if (!selectedRoles?.length) {
        this.selectedRolesForFilter = null;
        return;
      }

      this.selectedRolesForFilter = selectedRoles;
    },

    filter(): Employee[] {
      this.employeesFiltered = [] as Employee[];

      if (!this.employees) {
        return this.employeesFiltered;
      }

      this.eventBus.emit(AppConstants.filterGridEvent);

      if (this.selectedStatusForFilter && this.selectedRolesForFilter) {
        this.employeesFiltered = this.employees.filter((emp) => {
          return (
            emp.employmentStatus === this.selectedStatusForFilter?.value &&
            this.selectedRolesForFilter
              ?.map((i) => i.value)
              .includes(emp.primaryRole?.name)
          );
        });

        return this.employeesFiltered;
      }

      if (this.selectedStatusForFilter && !this.selectedRolesForFilter) {
        this.employeesFiltered = this.employees.filter((emp) => {
          return emp.employmentStatus === this.selectedStatusForFilter?.value;
        });

        return this.employeesFiltered;
      }

      if (this.selectedRolesForFilter) {
        this.employeesFiltered = this.employees.filter((emp) => {
          return this.selectedRolesForFilter
            ?.map((i) => i.value)
            .includes(emp.primaryRole?.name);
        });
      }

      return this.employeesFiltered;
    },

    onClearSearch() {
      this.selectedEmployee = "";
      this.employeesSuggestions = [...this.employees];
      this.employeesFiltered = [...this.employees];
      this.hasSearchSelection = false;
    },

    clearFilter() {
      this.selectedRolesForFilter = null;
      this.selectedStatusForFilter = null;
      this.employeesFiltered = [...this.employees];
      this.isFilterOpen = false;
    },

    closeForm() {
      this.isStaffModalVisible = false;
      this.currentEmployee = null;
    },

    handleSubmit(isAutoNext: boolean) {
      // ToDo. later.
      if (this.isSavingAlertOpened) {
        return;
      }
      useAlert({
        title: this.$t("common-alert-title"),
        content: this.$t("common-alert-content"),
        confirmButtonText: this.$t("common-alert-buttonText"),
        onConfirm: async () => {
          // Logic here
          this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi =
            true;
          this.isSavingAlertOpened = false;

          useNotification({
            type: NotificationType.SUCCESS,
            content: this.$t("common-notification-saved"),
            interval: AppConstants.notificationPopupTimeOut,
          });

          if (isAutoNext) {
            setTimeout(() => this.eventBus.emit(AppConstants.autoNextEvent), 0);
          }
        },
        onCancel: () => {
          this.isSavingAlertOpened = false;
        },
        onClose: () => {
          this.isSavingAlertOpened = false;
        },
      });

      this.isSavingAlertOpened = true;
    },

    async requestStaffToCompleteDetailsAsync(
      staff: Partial<Employee>,
    ): Promise<void> {
      // TODO. to continue
      if (!staff) {
        return;
      }

      const staffName = "Staff Name"; //`${staff.foreName} ${staff.surName}`;

      useAlert({
        title: this.$t("common-alert-title"),
        content: `Please confirm that you are happy for us to email ${staffName} to complete their profile.`,
        confirmButtonText: "Confirm & Send Email",
        onConfirm: async () => {
          useNotification({
            type: NotificationType.SUCCESS,
            content:
              "New Staff Added! <br /> <br /> We have emailed " +
              staffName +
              " to complete their profile",
            interval: AppConstants.notificationPopupTimeOut,
          });
        },
        onCancel: () => {
          this.isSavingAlertOpened = false;
        },
        onClose: () => {
          this.isSavingAlertOpened = false;
        },
      });

      this.isSavingAlertOpened = true;
    },

    setUniqueIdentifier(value: string): string {
      const identifier = `${AppConstants.arOrganizationalStructureRoute}${value}`;
      return this.helperService.removeStringSpacesThenSlash(identifier);
    }
  },
});
</script>

<template src="./ar-organizational-structure.html" />

<style scoped src="./ar-organizational-structure.css" />