<script lang="ts">
import { defineComponent, inject } from "vue";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";
import { container } from "tsyringe";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { IPagesRouteService } from "@/infra/dependency-services/navigation/IPagesRouteService";
import { RouteProps } from "@/entities/owners-and-controllers/RouteProps";
import {
  ISequenceNoKeeperService,
  ISequenceNoKeeperServiceInfo,
} from "@/infra/dependency-services/sequence-no/ISequenceNoKeeperService";
import {
  IUserSubmittedChangesService,
  IUserSubmittedChangesServiceInfo,
} from "@/infra/dependency-services/user-submission/IUserSubmittedChangesService";
import { useAutoSaveStore } from "@/stores/useAutoSaveStore";
import { OnboardingType } from "@/infra/base";
import {
  IPageRouteServiceFactory,
  IPageRouteServiceFactoryInfo,
} from "@/infra/dependency-services/pages-route-service-factory/IPageRouteServiceFactory";

export default defineComponent({
  name: "KendoSaveOrNextComponent",
  props: {
    isNextButtonDisabled: {
      type: Boolean,
      default: false,
    },
    isShowSaveButton: {
      type: Boolean,
      default: true,
    },
    isShowNoSubmitSaveButton: {
      type: Boolean,
      default: false,
    },
    pagesRouteService: {
      type: Object as () => IPagesRouteService,
      default: undefined,
    },
  },
  data() {
    return {
      AppConstants,
      groupRoutes: {} as Record<number, RouteProps>, //this is zero base index
      isNextButtonDisabledInternal: false,
      isBackButtonDisabled: false,
      navigationService: container.resolve<INavigationService>(
        INavigationServiceInfo.name
      ),
      helperService: container.resolve<IHelperService>(IHelperServiceInfo.name),
      userSubmittedChangesService:
        container.resolve<IUserSubmittedChangesService>(
          IUserSubmittedChangesServiceInfo.name
        ),
      eventBusService: inject("$eventBusService") as Emitter<
        Record<EventType, unknown>
      >,
      eventBusFormSaved: inject("$eventBusService") as Emitter<
        Record<EventType, boolean>
      >,
      isSaveButtonDisabled: true,
      isIgnoreFormFieldChange: false,
      nextPageTitle: "",
      sequenceNoKeeperService: container.resolve<ISequenceNoKeeperService>(
        ISequenceNoKeeperServiceInfo.name
      ),
      currentRouteSequence: 0,
      isNextButtonVisible: true,
    };
  },
  computed: {
    nextPageTitleComputed(): string {
      if (this.isNextButtonDisabledInternal) {
        return "";
      }

      return this.nextPageTitle;
    },
    isAutoSaveFunctionCompleted(): boolean {
      const autoSaveStore = useAutoSaveStore();
      return autoSaveStore.isAutoSaveFunctionCompleted;
    },
  },
  created() {
    this.isNextButtonDisabledInternal = this.isNextButtonDisabled;
  },
  mounted() {
    this.eventBusService.on(
      AppConstants.formExpandOrCollapseEvent,
      this.expandOrCollapseFormAsync
    );
    this.eventBusService.on(AppConstants.userClickSideMenuRouteEvent, () => {
      this.getNextRouteTitle();
    });
    this.eventBusService.on(
      AppConstants.autoNextEvent,
      async () => await this.nextPageAsync()
    );
    this.eventBusService.on(
      AppConstants.formFieldChangedEvent,
      this.pageFieldChanged
    );

    this.createCurrentGroupRoutes();
    this.getNextRouteTitle();
  },
  unmounted() {
    this.eventBusService.off(AppConstants.formFieldChangedEvent);
    this.eventBusService.off(AppConstants.formExpandOrCollapseEvent);
    this.eventBusService.off(AppConstants.userClickSideMenuRouteEvent);
    this.eventBusService.off(AppConstants.autoNextEvent);
  },
  updated() {
    this.isNextButtonDisabledInternal = this.isNextButtonDisabled;
  },
  setup() {
    const { setDisableAutoSave } = useAutoSaveStore();

    return {
      setDisableAutoSave,
    };
  },
  methods: {
    async nextPageAsync() {
      if (
        !this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi
      ) {
        this.setDisableAutoSave(true);
        // Pass 'true' because we want to auto next after save
        this.eventBusFormSaved.emit(AppConstants.formSavedEvent, true);
        return;
      }

      this.$emit("onNext"); // I think this is already not in use
      this.createCurrentGroupRoutes();
      const count = Object.keys(this.groupRoutes).length;
      this.sequenceNoKeeperService.incrementSequenceNo();
      this.getNextRouteTitle();
      const currentSequenceNo =
        this.sequenceNoKeeperService.getCurrentSequenceNo();
      let currentRoute = this.groupRoutes[currentSequenceNo];

      if (currentSequenceNo >= count - 1) {
        this.isNextButtonDisabledInternal = true;
      }

      await this.navigationService.navigateAsync(currentRoute.route);
    },

    save() {
      this.setDisableAutoSave(true);
      this.$emit("onSave");
      this.eventBusFormSaved.emit(AppConstants.formSavedEvent, false);
    },

    getNextRouteTitle() {
      if (!Object.values(this.groupRoutes).length) {
        return;
      }

      const count = Object.keys(this.groupRoutes).length;
      const currentSequenceNo =
        this.sequenceNoKeeperService.getCurrentSequenceNo();

      const isAtLastPage = currentSequenceNo >= count - 1;
      this.isNextButtonDisabledInternal = isAtLastPage;
      this.isNextButtonVisible = !isAtLastPage;

      if (!this.isNextButtonVisible) {
        return;
      }

      const nextRoute = this.groupRoutes[currentSequenceNo + 1];
      this.nextPageTitle = nextRoute.title;
    },

    pageFieldChanged() {
      if (this.isIgnoreFormFieldChange) {
        return;
      }

      this.isSaveButtonDisabled = false;
    },

    async expandOrCollapseFormAsync() {
      this.isIgnoreFormFieldChange = true;
      await this.helperService.delayAsync(1000);
      this.isIgnoreFormFieldChange = false;
    },

    createCurrentGroupRoutes() {
      let pages = [] as RouteProps[];

      if (!this.pagesRouteService) {
        const cachedOnboardingType =
          localStorage.getItem(AppConstants.onboardingTypeKey) ??
          OnboardingType.Firm.toString();

        const pageRouteServiceFactory =
          container.resolve<IPageRouteServiceFactory>(
            IPageRouteServiceFactoryInfo.name
          );

        let defaultPageRouteService =
          pageRouteServiceFactory.createPageRouteService(cachedOnboardingType);
        pages = defaultPageRouteService.getCurrentRoutes();
      } else {
        pages = this.pagesRouteService.getCurrentRoutes();
      }

      let counter = 0;

      pages.forEach((item) => {
        if (item.isDisabled) {
          return;
        }

        this.groupRoutes[counter] = item;

        counter++;
      });

      const count = Object.keys(this.groupRoutes).length;

      if (this.currentRouteSequence >= count) {
        this.isNextButtonDisabledInternal = true;
      }

      if (this.currentRouteSequence <= 1) {
        this.isBackButtonDisabled = true;
      }
    },
  },
});
</script>

<template>
  <div class="BottomBar">
    <StackLayout
      class="BottomBar-container"
      :align="{ horizontal: 'start', vertical: 'middle' }"
    >
      <div class="flex-grow-1 align-self-stretch">
        <div class="BottomBar-note">
          <!-- @TODO Save time note here -->
        </div>
      </div>

      <StackLayout :gap="10">
        <span
          v-if="isAutoSaveFunctionCompleted"
          class="BottomBar-saveAction text-success"
          >Saved!</span
        >

        <div class="BottomBar-saveAction">
          <Button
            type="button"
            theme-color="primary"
            fill-mode="flat"
            :disabled="isSaveButtonDisabled"
            @click="save"
          >
            <IconComponent symbol="save" class="me-1" size="15" />
            Save
          </Button>
        </div>

        <div v-if="isNextButtonVisible" class="BottomBar-nextAction">
          <Button
            type="button"
            theme-color="primary"
            @click="nextPageAsync"
            :disabled="isNextButtonDisabledInternal"
          >
            {{
              nextPageTitleComputed
                ? `Next: ${$t(nextPageTitleComputed)}`
                : "Next"
            }}
            <IconComponent symbol="arrow-right-1-50" size="24" class="ms-1" />
          </Button>
        </div>
      </StackLayout>
    </StackLayout>
  </div>
</template>

<style scoped lang="scss">
.BottomBar {
  background-color: var(--topbar-bg-color);
  border-top: 1px solid #0000001f;
  padding: 0 10px;

  &-container {
    max-width: 1280px;
    margin: 0 auto;
  }

  &-note {
    width: 218px;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
  }

  &-saveAction {
    display: flex;
    justify-content: center;
    align-items: center;
  }

  &-nextAction {
    padding: 10px;
  }
}
</style>
