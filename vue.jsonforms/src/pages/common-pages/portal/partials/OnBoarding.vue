<script setup lang="ts">
import { nextTick, ref, onMounted, inject } from "vue";
import { Emitter, EventType } from "mitt";
import Shepherd from "shepherd.js";
import { onUnmounted } from "vue";
import { useStorage } from "@vueuse/core";
import { container } from "tsyringe";
import { AppConstants } from "@/infra/AppConstants";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import {
  ICustomerService,
  ICustomerServiceInfo,
} from "@/infra/dependency-services/rest/forms-compliance/ICustomerService";
import {
  INavigationService,
  INavigationServiceInfo,
} from "@/infra/dependency-services/navigation/INavigationService";
import { CustomerEntity } from "@/entities/CustomerEntity";
import { OnboardingType } from "@/infra/base";
import { OnBoardingStep } from "@/pages/common-pages/portal/partials/OnBoardingTypes";
import {
  IAppointedRepresentativeService,
  IAppointedRepresentativeServiceInfo,
} from "@/infra/dependency-services/rest/appointed-representative/IAppointedRepresentativeService";
import { AppointedRepresentative } from "@/entities/appointed-representatives/AppointedRepresentative";
import {
  IOrganizationalStructureService,
  IOrganizationalStructureServiceInfo,
} from "@/infra/dependency-services/rest/organizational-structure/IOrganizationalStructureService";
import { ProvidersEntity } from "@/entities/providers-and-introducers/ProvidersEntity";
import {
  IProvidersServiceInfo,
  IProvidersService,
} from "@/infra/dependency-services/rest/providers/IProvidersService";
import {
  IIntroducersServiceInfo,
  IIntroducersService,
} from "@/infra/dependency-services/rest/introducers/IIntroducersService";
import { IntroducersEntity } from "@/entities/providers-and-introducers/IntroducersEntity";

const eventBus = inject("$eventBusService") as Emitter<
  Record<EventType, unknown>
>;
const navigationService = container.resolve<INavigationService>(
  INavigationServiceInfo.name,
);

const customerService = container.resolve<ICustomerService>(
  ICustomerServiceInfo.name,
);

const customerArService = container.resolve<IAppointedRepresentativeService>(
  IAppointedRepresentativeServiceInfo.name,
);

const customerEmployeeService =
  container.resolve<IOrganizationalStructureService>(
    IOrganizationalStructureServiceInfo.name,
  );

const customerProviderService = container.resolve<IProvidersService>(
  IProvidersServiceInfo.name,
);

const customerIntroducerService = container.resolve<IIntroducersService>(
  IIntroducersServiceInfo.name,
);

const appService = container.resolve<IAppService>(IAppServiceInfo.name);

const isCompleted = useStorage(AppConstants.onboardingCompletedKey, false);
const isActive = ref(false);
const stepsIds = ref<string[]>([]);
const activeStep = ref();
const customer = ref(new CustomerEntity());
const customerAr = ref(new AppointedRepresentative());
const customerProvider = ref(new ProvidersEntity());
const customerIntroducer = ref(new IntroducersEntity());
const onboardingType: string | null =
  localStorage.getItem(AppConstants.onboardingTypeKey) ??
  OnboardingType.Firm.toString();

const onBoard = () => {
  const tour = new Shepherd.Tour({
    useModalOverlay: true,
    defaultStepOptions: {
      scrollTo: true,
    },
  });

  //ToDo. OnBoarding. Move all texts to localizations soon...
  const steps: Array<OnBoardingStep> = [
    {
      id: "step-1",
      classes: "shepherd-step-1",
      text: () => {
        let onBoardingTypeTitle: string;

        switch (onboardingType) {
          case OnboardingType.Ar.toString():
            onBoardingTypeTitle = "<br/>Appointed Representative (AR)";
            break;
          case OnboardingType.Employee.toString():
            onBoardingTypeTitle = "Employee";
            break;
          case OnboardingType.Provider.toString():
            onBoardingTypeTitle = "Provider";
            break;
          case OnboardingType.Introducer.toString():
            onBoardingTypeTitle = "Introducer";
            break;
          default:
            onBoardingTypeTitle = "Firm";
        }

        return `
          <img src="/waving-hand.svg" alt="Waving hand" width="28" height="30" class="mb-3" />
          <h4 class="font-size-3xl font-weight-bold text-black">Let’s set up your ${onBoardingTypeTitle} Profile</h4>
          <p>Here are some tips to help you set up your ${onboardingType}  Profile<br /> and maximise the potential of ${AppConstants.AppName}.</p>
        `;
      },
      buttons: [
        {
          text: "Skip Tips",
          classes: "shepherd-button-flat text-decoration-underline",
          action: tour.cancel,
        },
        {
          text: "Get Started",
          action: tour.next,
        },
      ],
      cancelIcon: {
        enabled: true,
      },
      arrow: false,
    },
    {
      id: "step-2",
      classes: "shepherd-step-2 shepherd-step-blue",
      attachTo: {
        element: ".MainDrawer .k-drawer",
        on: "right-start",
      },
      text: () => {
        return `
          <img src="/align-left.svg" alt="Vector" class="mb-3" />
          <h5 class="font-size-default font-weight-bold text-black">Navigation</h5>
          <p class="font-size-sm">Hover over these icons to view relevant sections of the ${onboardingType} Profile.</p>
          <p class="font-size-sm">Fully completed sections will be marked with a ✅</p>
        `;
      },
      buttons: [
        {
          text: "Skip",
          classes: "shepherd-button-flat-black text-decoration-underline",
          action: tour.cancel,
        },
        {
          text: "Got it",
          classes: "shepherd-button-black",
          action: tour.next,
        },
      ],
      cancelIcon: {
        enabled: true,
      },
      arrow: false,
    },
    {
      id: "step-3",
      classes: "shepherd-step-3 shepherd-step-blue",
      attachTo: {
        element: ".SideMenuToggle",
        on: "right-end",
      },
      text: () => {
        return `
          <img src="/align-left.svg" alt="Vector" class="mb-3" />
          <h5 class="font-size-default font-weight-bold text-black">Navigation</h5>
          <p class="font-size-sm">Click here to expand the sidebar menu.</p>
        `;
      },
      buttons: [
        {
          text: "Skip",
          classes: "shepherd-button-flat-black text-decoration-underline",
          action: tour.cancel,
        },
        {
          text: "Got it",
          classes: "shepherd-button-black",
          action: tour.next,
        },
      ],
      cancelIcon: {
        enabled: true,
      },
      arrow: false,
    },
    {
      id: "step-4",
      classes: "shepherd-step-4 shepherd-step-secondary",
      attachTo: {
        element: ".FirmDetailNav",
        on: "bottom-start",
      },
      text: () => {
        return `
          <img src="/cursor.svg" alt="Cursor" class="mb-3" />
          <h5 class="font-size-default font-weight-bold text-black">Click</h5>
          <p class="font-size-sm">Click these tab buttons to navigate to a subdivision of the ${onboardingType}  Profile.</p>
        `;
      },
      buttons: [
        {
          text: "Skip",
          classes: "shepherd-button-flat-black text-decoration-underline",
          action: tour.cancel,
        },
        {
          text: "Got it",
          classes: "shepherd-button-black",
          action: tour.next,
        },
      ],
      cancelIcon: {
        enabled: true,
      },
      arrow: false,
    },
    {
      id: "step-5",
      classes: "shepherd-step-5 shepherd-step-primary",
      attachTo: {
        element: ".BottomBar-note",
        on: "top-start",
      },
      text: () => {
        return `
          <img src="/letter.svg" alt="Letter" class="mb-3" />
          <h5 class="font-size-default font-weight-bold text-white">Don’t worry</h5>
          <p class="font-size-sm text-white font-weight-light">Autosave is enabled every 2 minutes, so you don't lose data you have already entered.</p>
        `;
      },
      buttons: [
        {
          text: "Skip",
          classes: "shepherd-button-flat-white text-decoration-underline",
          action: tour.cancel,
        },
        {
          text: "Got it",
          classes: "shepherd-button-black",
          action: tour.next,
        },
      ],
      cancelIcon: {
        enabled: true,
      },
      arrow: false,
    },
    {
      id: "step-6",
      classes: "shepherd-step-6",
      attachTo: {
        element: ".BottomBar-saveAction",
        on: "top-end",
      },
      text: () => {
        return `
          <img src="/light-bulb-shine.svg" alt="Light Bulb" class="mb-3" />
          <h5 class="font-size-default font-weight-bold text-black">Quick tip</h5>
          <p class="font-size-sm">Or you can click "Save" at any time, so you can return when you're ready to continue.</p>
        `;
      },
      buttons: [
        {
          text: "Skip",
          classes: "shepherd-button-flat-black text-decoration-underline",
          action: tour.cancel,
        },
        {
          text: "Got it",
          action: tour.next,
        },
      ],
      cancelIcon: {
        enabled: true,
      },
      arrow: false,
    },
    {
      id: "step-7",
      classes: "shepherd-step-7",
      attachTo: {
        element: ".BottomBar-nextAction",
        on: "top-start",
      },
      text: () => {
        return `
          <img src="/light-bulb-shine.svg" alt="Light Bulb" class="mb-3" />
          <h5 class="font-size-default font-weight-bold text-black">Last one!</h5>
          <p class="font-size-sm">Click "Next" to navigate to the next section of the ${onboardingType} Profile.</p>
        `;
      },
      buttons: [
        {
          text: "Done",
          action: tour.next,
        },
      ],
      cancelIcon: {
        enabled: true,
      },
      arrow: false,
    },
  ];

  stepsIds.value = steps.map(({ id }) => id);

  tour.on("show", (event: { step: Shepherd.Step }) => {
    activeStep.value = event.step.id;
  });
  tour.on("active", () => {
    isActive.value = true;
  });
  tour.on("inactive", () => {
    isActive.value = false;
  });
  tour.on("complete", async () => {
    await saveOnboardingAsync();
  });

  tour.addSteps(steps);
  tour.start();
};

const init = async () => {
  if (!isCompleted.value) {
    let onboardingCompleted: boolean | undefined = false;

    switch (onboardingType) {
      case OnboardingType.Ar.toString():
        customerAr.value =
          appService.getCachedCustomerAppointedRepresentative() ??
          new AppointedRepresentative();
        onboardingCompleted = fetchOnboardingCompletedValue(customerAr.value);
        break;
      case OnboardingType.Firm.toString():
      default:
        customer.value = appService.getCachedCustomer() ?? new CustomerEntity();
        onboardingCompleted = fetchOnboardingCompletedValue(customer.value);
    }

    isCompleted.value = onboardingCompleted;
    useStorage(AppConstants.onboardingCompletedKey, onboardingCompleted);
  }

  if (isCompleted.value || isActive.value) {
    return;
  }

  switch (onboardingType) {
    case OnboardingType.Ar:
      if (!navigationService.isActivePage(AppConstants.arFirmDetailsRoute)) {
        await navigationService.navigateRootAsync(
          AppConstants.arFirmDetailsRoute,
        );
      }
      break;
    case OnboardingType.Employee:
      if (
        !navigationService.isActivePage(AppConstants.employeeFirmDetailsRoute)
      ) {
        await navigationService.navigateRootAsync(
          AppConstants.employeeFirmDetailsRoute,
        );
      }
      break;
    case OnboardingType.Provider:
      if (
        !navigationService.isActivePage(
          AppConstants.providerProfileDetailsRoute,
        )
      ) {
        await navigationService.navigateRootAsync(
          AppConstants.providerProfileDetailsRoute,
        );
      }
      break;
    case OnboardingType.Introducer:
      if (
        !navigationService.isActivePage(
          AppConstants.introducerProfileFirmDetailsRoute,
        )
      ) {
        await navigationService.navigateRootAsync(
          AppConstants.introducerProfileFirmDetailsRoute,
        );
      }
      break;
    case OnboardingType.Firm:
    default:
      if (!navigationService.isActivePage(AppConstants.firmDetailsRoute)) {
        await navigationService.navigateRootAsync(
          AppConstants.firmDetailsRoute,
        );
      }
  }

  await nextTick();
  onBoard();
};

const fetchOnboardingCompletedValue = (
  customerValue: CustomerEntity | AppointedRepresentative,
): boolean => {
  const { onboardingCompleted } = customerValue;

  if (onboardingCompleted !== undefined) {
    return onboardingCompleted;
  } else {
    return false;
  }
};

const saveOnboardingAsync = async () => {
  isCompleted.value = true;
  useStorage(AppConstants.onboardingCompletedKey, true);

  switch (onboardingType) {
    case OnboardingType.Ar.toString():
      // eslint-disable-next-line no-case-declarations
      const tourCustomerArData =
        await customerArService.getAppointedRepresentativesByEmailAsync(
          customerAr.value.email ?? "",
        );

      tourCustomerArData.onboardingCompleted = true;
      await customerArService.saveOrUpdateAppointedRepresentativeAsync(
        tourCustomerArData,
      );
      break;
    case OnboardingType.Employee.toString():
      // eslint-disable-next-line no-case-declarations
      const tourCustomerEmployeeData =
        await customerEmployeeService.getEmployeeByEmailAsync(
          customerAr.value.email ?? "",
        );

      if (tourCustomerEmployeeData) {
        tourCustomerEmployeeData.onboardingCompleted = true;
        await customerEmployeeService.saveOrUpdateEmployeeAsync(
          tourCustomerEmployeeData,
        );
      }

      break;
    case OnboardingType.Provider.toString():
      // eslint-disable-next-line no-case-declarations
      const tourProviderData =
        await customerProviderService.getProviderByEmailAsync(
          customerProvider.value.email ?? "",
        );

      if (tourProviderData) {
        tourProviderData.onboardingCompleted = true;
        await customerProviderService.saveOrUpdateProvidersAsync(
          tourProviderData,
        );
      }

      break;
    case OnboardingType.Introducer.toString():
      // eslint-disable-next-line no-case-declarations
      const tourIntroducerData =
        await customerIntroducerService.getIntroducerByEmailAsync(
          customerIntroducer.value.email ?? "",
        );

      if (tourIntroducerData) {
        tourIntroducerData.onboardingCompleted = true;
        await customerIntroducerService.saveOrUpdateIntroducersAsync(
          tourIntroducerData,
        );
      }

      break;
    case OnboardingType.Firm.toString():
    default:
      // eslint-disable-next-line no-case-declarations
      const tourCustomerData = await customerService.getCustomerByEmailAsync(
        customer.value.email ?? "",
      );

      tourCustomerData.onboardingCompleted = true;
      await customerService.saveCustomerAsync(JSON.stringify(tourCustomerData));
  }
};

onMounted(() => {
  eventBus.on(AppConstants.portalLoadedEvent, init);
});

onUnmounted(() => {
  eventBus.off(AppConstants.portalLoadedEvent);
});
</script>

<template>
  <div :class="['shepherd-dots', !isActive && 'shepherd-dots-hidden']">
    <span
      v-for="stepId in stepsIds"
      :key="`step-dot-${stepId}`"
      :style="{
        backgroundColor: stepId === activeStep ? '#3FC6EB' : undefined,
      }"
    ></span>
  </div>
</template>

<style lang="scss">
@import "~shepherd.js/dist/css/shepherd.css";

%shepherd-step-arrow {
  max-width: 100%;

  &:before {
    content: "";
    display: block;
    width: 130px;
    height: 104px;
    position: absolute;
    background: no-repeat transparent url("@/assets/img/swirl-arrow.svg");
    background-size: cover;
  }
}

%shepherd-text-narrow {
  .shepherd-text {
    margin-top: -20px;
  }
}

.shepherd-modal-overlay-container.shepherd-modal-is-visible {
  opacity: 0.8;
}

.shepherd-element {
  border-radius: 8px;
}

.shepherd-text {
  padding-top: 0;

  p {
    color: var(--color-black);
  }
}

.shepherd-footer {
  padding: 15px;
}

.shepherd-cancel-icon {
  font-weight: 200;
  color: #000;
}

.shepherd-button {
  background-color: var(--color-primary);
  color: #fff !important;
  border-radius: 100px;
  padding: 8px 25px;
  font-weight: 600;

  &:hover {
    background-color: var(--color-primary-shade) !important;
  }
}

.shepherd-button-black {
  background-color: var(--color-black) !important;

  &:hover {
    background-color: var(--color-black) !important;
  }
}

.shepherd-button-flat {
  &,
  &:hover {
    background-color: transparent !important;
  }

  &:hover {
    text-decoration: underline;
  }

  color: var(--color-primary) !important;
}

.shepherd-button-flat-black {
  &,
  &:hover {
    background-color: transparent !important;
  }

  &:hover {
    text-decoration: underline;
  }

  color: var(--color-black) !important;
}

.shepherd-button-flat-black {
  &,
  &:hover {
    background-color: transparent !important;
  }

  &:hover {
    text-decoration: underline;
  }

  color: var(--color-black) !important;
}

.shepherd-button-flat-white {
  &,
  &:hover {
    background-color: transparent !important;
  }

  &:hover {
    text-decoration: underline;
  }

  color: var(--color-white) !important;
  font-weight: normal;
}

.shepherd-step-blue {
  background-color: var(--brand-color-brand-light);
}

.shepherd-step-secondary {
  background-color: var(--color-secondary);
}

.shepherd-step-primary {
  background-color: var(--color-primary);
}

.shepherd-step-1 {
  max-width: 100%;
  width: 600px;

  .shepherd-text {
    text-align: center;
  }

  .shepherd-footer {
    justify-content: center;
  }
}

.shepherd-step-2 {
  @extend %shepherd-step-arrow;
  @extend %shepherd-text-narrow;
  width: 300px;
  margin-top: 30px;
  margin-left: 30px;

  &:before {
    top: 100%;
    left: -10%;
  }
}

.shepherd-step-3 {
  @extend %shepherd-step-arrow;
  @extend %shepherd-text-narrow;
  margin-top: 90px;
  margin-left: 30px;
  width: 300px;

  &:before {
    bottom: calc(100% + 10px);
    left: -30px;
    transform: rotateX(180deg) rotateZ(-20deg);
  }
}

.shepherd-step-4 {
  @extend %shepherd-step-arrow;
  @extend %shepherd-text-narrow;
  width: 300px;
  margin-top: 20px;

  &:before {
    top: -20px;
    right: calc(100% - 20px);
    transform: rotate(120deg);
  }
}

.shepherd-step-5 {
  @extend %shepherd-step-arrow;
  @extend %shepherd-text-narrow;
  width: 300px;
  margin-top: -100px;
  margin-left: 50px;

  &:before {
    right: 0;
    top: calc(100% + 20px);
    transform: rotate(-40deg);
  }

  .shepherd-cancel-icon {
    color: var(--color-white);
  }
}

.shepherd-step-6 {
  @extend %shepherd-step-arrow;
  @extend %shepherd-text-narrow;
  width: 300px;
  margin-top: -90px;

  &:before {
    left: 20%;
    top: calc(100% + 10px);
    transform: rotateY(180deg) rotateZ(-40deg);
  }
}

.shepherd-step-7 {
  @extend %shepherd-step-arrow;
  @extend %shepherd-text-narrow;
  width: 300px;
  margin-top: -90px;
  margin-left: -15%;

  &:before {
    left: 45%;
    top: calc(100% + 10px);
    transform: rotateY(180deg) rotateZ(-40deg);
  }
}

.shepherd-dots {
  display: flex;
  position: fixed;
  z-index: 100000;
  bottom: 30px;
  left: 50%;
  transform: translateX(-50%);
  gap: 10px;

  span {
    width: 14px;
    height: 14px;
    background: #97a1af;
    border-radius: 50%;
    transition: 0.3s ease all;
  }

  &.shepherd-dots-hidden {
    display: none;
    opacity: 0;
    visibility: hidden;
  }
}
</style>