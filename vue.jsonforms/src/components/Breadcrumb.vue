<script setup lang="ts">
import { computed, ref } from "vue";
import { RouteRecordName, RouteRecordNormalized, useRoute } from "vue-router";
import { container } from "tsyringe";
import {
  IHelperService,
  IHelperServiceInfo,
} from "@/infra/dependency-services/helper/IHelperService";
import { AppConstants } from "@/infra/AppConstants";
import { OnboardingType } from "@/infra/base";

const helperService = container.resolve<IHelperService>(
  IHelperServiceInfo.name,
);

const route = useRoute();
const listClasses = ref(["breadcrumb"]);

const routeName = (routeName?: RouteRecordName): RouteRecordName | string => {
  if (!routeName) {
    return "";
  }

  let cachedOnboardingType = localStorage.getItem(
    AppConstants.onboardingTypeKey,
  );

  if (routeName === AppConstants.portalRouteName) {
    switch (cachedOnboardingType) {
      case OnboardingType.Ar.toString():
        routeName = "Appointed Representative";
        break;
      case OnboardingType.Employee.toString():
        routeName = "Employee";
        break;
      default: {
        routeName = "Firm Profile";
        break;
      }
    }
  }

  return routeName;
};

const breadcrumbItems = computed(() => {
  const matchedRoutes: RouteRecordNormalized[] = route.matched;

  // Add this when user role permission is available
  // matchedRoutes.shift();

  return matchedRoutes.map((route, index) => ({
    label: route.meta?.breadcrumb || routeName(route.name) || "Unnamed",
    to: route.path,
    active: index === matchedRoutes.length - 1,
    nonClickable: index === 0,
    capitalized: route.meta?.removeFormatting !== true,
  }));
});
</script>

<template>
  <nav aria-label="breadcrumb">
    <ol :class="listClasses">
      <li class="breadcrumb-item-home">
        <router-link :to="'/'">
          <IconComponent
            symbol="home-breadcrumbs"
            size="23"
            class="home-button"
          />
        </router-link>

        <IconComponent symbol="chevron-breadcrumbs" size="19" class="chevron" />
      </li>

      <li
        class="breadcrumb-item"
        v-for="(item, index) in breadcrumbItems"
        :key="index"
      >
        <router-link
          v-if="!item.nonClickable"
          :to="item.to"
          :aria-current="item.active ? 'page' : null"
          :class="{ active: item.active }"
        >
          {{
            !item.capitalized
              ? item.label.toString()
              : helperService.convertToCapitalizedWords(item.label.toString())
          }}
        </router-link>

        <span v-else>
          {{
            !item.capitalized
              ? item.label.toString()
              : helperService.convertToCapitalizedWords(item.label.toString())
          }}
        </span>
      </li>
    </ol>
  </nav>
</template>
