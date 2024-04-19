import { AlertConfig } from "@/composables/useAlert";

export {};

declare module "vue" {
  interface ComponentCustomProperties {
    alertService: (config: Partial<AlertConfig>) => void;
  }
}
