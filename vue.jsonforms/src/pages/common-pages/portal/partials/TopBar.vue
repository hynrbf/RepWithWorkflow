<script setup lang="ts">
import { ref, computed, inject } from "vue";
import { storeToRefs } from "pinia";
import logo from "@/assets/img/logo.svg";
import { useCustomerStore } from "@/stores/useCustomerStore";
import { MenuSelectEvent } from "@progress/kendo-vue-layout";
import { container } from "tsyringe";
import { LANDING_URL } from "@/config";
import { AppConstants } from "@/infra/AppConstants";
import {
  IAppService,
  IAppServiceInfo,
} from "@/infra/dependency-services/app/IAppService";
import { useAuth0 } from "@auth0/auth0-vue";
import {
  IFirmPagesRouteService,
  IFirmPagesRouteServiceInfo,
} from "@/infra/dependency-services/pages-route-firm/IFirmPagesRouteService";
import { RouteProps } from "@/entities/owners-and-controllers/RouteProps";
import { Emitter, EventType } from "mitt";
import { onClickOutside } from "@vueuse/core";

const { currentCustomerName } = storeToRefs(useCustomerStore());
const componentInstance = window.location.href;

const logOutText = "Logout";
const dashBoardText = "Dashboard";
const taskManagementText = "Task Management";

const menuButton = ref();
const isMenuShown = ref(false);
const menuItems = computed(() => [
  {
    text: dashBoardText,
  },
  {
    text: taskManagementText,
  },
  {
    text: "My Documents",
  },
  {
    text: "CPD",
  },
  {
    text: "CRM",
  },
  {
    text: "Profile",
  },
  {
    text: logOutText,
  },
]);

const appService = ref(container.resolve<IAppService>(IAppServiceInfo.name));
const firmPagesRouteService = ref(
  container.resolve<IFirmPagesRouteService>(IFirmPagesRouteServiceInfo.name),
);
const eventBusService =
  inject<Emitter<Record<EventType, RouteProps[]>>>("$eventBusService");
const auth0 = useAuth0();

const logoutAsync = async () => {
  await auth0.logout({
    logoutParams: {
      returnTo: LANDING_URL,
    },
  });
  appService.value.clearAllLocalCacheWithToken();
};

const onMenuSelectAsync = async (e: MenuSelectEvent) => {
  const textMenu = e.item.text;

  if (!textMenu) {
    return;
  }

  switch (textMenu) {
    case logOutText:
      await logoutAsync();
      break;
    case dashBoardText:
      eventBusService?.emit(
        AppConstants.routesChangedEvent,
        firmPagesRouteService.value.getCurrentRoutes(),
      );
      break;
    case taskManagementText:
    default:
      alert("Not supported as of now");
  }
};

const menuPopupElement = ref();
onClickOutside(
  menuPopupElement,
  () => {
    if (isMenuShown.value) {
      isMenuShown.value = false;
    }
  },
  { ignore: [menuButton] },
);
</script>

<template>
  <KendoAppBar class="TopBar">
    <KendoAppBarSection>
      <a href="" title="Richdale">
        <img :src="logo" alt="Richdale" width="128" height="21" />
      </a>
    </KendoAppBarSection>

    <KendoAppBarSpacer />

    <div
      class="logo-name-right"
      v-if="
        componentInstance.includes(AppConstants.signUpRoute) ||
        componentInstance.includes(AppConstants.changePasswordRoute)
      "
    >
      <Label
        style="
          color: #000;
          line-height: 20.8px;
          font-size: 16px;
          font-weight: 400;
        "
        >Already a customer?
      </Label>

      <Label style="color: #036; margin-left: 5px">
        <a
          href="#"
          style="font-weight: 700; font-size: 16px; line-height: 24px"
          @click="logoutAsync"
          >Log In</a
        >
      </Label>
    </div>

    <KendoAppBarSection class="TopBar-nav" v-else>
      <KendoButton
        type="button"
        size="small"
        fill-mode="flat"
        rounded="full"
        shape="square"
      >
        <IconComponent symbol="search-1" size="20" />
      </KendoButton>

      <KendoButton
        type="button"
        size="small"
        fill-mode="flat"
        rounded="full"
        shape="square"
      >
        <IconComponent symbol="alarm-bell" size="20" />
      </KendoButton>

      <KendoButton
        type="button"
        size="small"
        fill-mode="flat"
        rounded="full"
        shape="square"
        class="p-0"
      >
        <DynamicAvatarComponent
          :text="currentCustomerName"
          avatar-only
          rounded="full"
          type="image"
        />
      </KendoButton>

      <KendoButton
        ref="menuButton"
        type="button"
        size="small"
        fill-mode="flat"
        rounded="full"
        shape="square"
        :class="['MenuButton']"
        @click.stop="isMenuShown = !isMenuShown"
      >
        <IconComponent
          v-show="!isMenuShown"
          symbol="hamburger-menu-1-5"
          size="20"
        />
        <IconComponent v-show="isMenuShown" symbol="delete-1-19" size="20" />
      </KendoButton>
      <KendoPopup
        ref="menuPopupElement"
        anchor="menuButton"
        :show="isMenuShown"
        :anchor-align="{ horizontal: 'right', vertical: 'bottom' }"
        :popup-align="{ horizontal: 'right', vertical: 'top' }"
        popup-class="TopBar-menuPopup"
        :animate="false"
      >
        <KendoMenu
          :items="menuItems"
          vertical
          :style="{ display: 'inline-block' }"
          @select="onMenuSelectAsync"
        />
      </KendoPopup>
    </KendoAppBarSection>
  </KendoAppBar>
</template>

<style scoped lang="scss">
.TopBar {
  background-color: var(--topbar-bg-color);
  box-shadow: 0px 1px 0px 0px #00000014;
  padding: 8px 25px;
  margin-bottom: 1px;
  position: relative;
  z-index: 3;

  &-nav {
    display: flex;
    gap: 3px;

    :deep(.k-button-text) {
      color: var(--topbar-nav-color);
    }
  }

  :global(#{&}-menuPopup) {
    border-radius: 10px;
  }

  :global(#{&}-menuPopup .k-menu-item) {
    padding: 5px;
  }

  :global(#{&}-menuPopup .k-menu-link-text) {
    color: var(--topbar-menu-link-color);
    font-weight: 500;
  }
}

.logo-name-right {
  font-size: 15px;
  font-weight: 500;
  margin: 7px 0 7px 0;
}
</style>
