import { defineStore } from "pinia";
import { AppConstants } from "@/infra/AppConstants";
import { Emitter, EventType } from "mitt";

export const usePageFieldsInvalidHandlerStore = defineStore(
  AppConstants.pageFieldsInvalidHandlerStore,
  {
    state: () => ({
      invalidValues: [] as {
        page: string;
        fieldId: string;
      }[],
    }),
    actions: {
      addInvalidFieldEntry(fieldId: string | undefined) {
        AppConstants.firmProfilePages.forEach((item) => {
          if (!fieldId) {
            return;
          }

          const routeCleaned = item.route.replace("/", "");

          if (!fieldId.toLowerCase().includes(routeCleaned.toLowerCase())) {
            return;
          }

          const existing = this.invalidValues.find(
            (i) => i.page === item.route && i.fieldId === fieldId,
          );

          if (existing) {
            return;
          }

          this.invalidValues.push({
            page: item.route,
            fieldId: fieldId,
          });
        });
      },

      clearInvalidEntries(fieldId: string | undefined) {
        if (!fieldId) {
          return;
        }

        const index = this.invalidValues.findIndex(
          (i) => i.fieldId === fieldId,
        );

        if (index < 0) {
          return;
        }

        this.invalidValues.splice(index, 1);
      },

      processInvalidFields(
        routeName: string,
        eventBus: Emitter<Record<EventType, unknown>>,
      ) {
        const fieldIds = this.getPageInvalidEntryValues(routeName);

        if (fieldIds && fieldIds.length > 0) {
          for (const fieldId of fieldIds) {
            eventBus.emit(`${AppConstants.pageFieldInvalidEvent}-${fieldId}`);
          }
        }
      },

      getPageInvalidEntryValues(routeName: string): string[] {
        const foundInvalidEntries = this.invalidValues.filter(
          (v) => v.page === routeName,
        );
        return foundInvalidEntries?.map((e) => e.fieldId) ?? ([] as string[]);
      },
    },
    persist: true,
  },
);
