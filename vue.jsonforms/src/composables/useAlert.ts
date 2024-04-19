import { h, render } from "vue";
import { v4 as uuidv4 } from "uuid";
import { Dialog, DialogActionsBar } from "@progress/kendo-vue-dialogs";
import { Button } from "@progress/kendo-vue-buttons";

export enum AlertType {
  ALERT = "alert",
  CONFIRM = "confirm",
  SAVEDETAILS = "saveDetails",
}

export interface AlertConfig {
  id: string;
  type: AlertType;
  title?: string;
  content?: string;
  additionalContent?: string;
  visible: boolean;
  confirmButtonText?: string;
  cancelButtonText?: string;
  confirmButtonThemeColor?: string;
  cancelButtonThemeColor?: string;
  confirmButtonFillMode?: string;
  cancelButtonFillMode?: string;
  width?: number;
  saveDetailsThemeColor?: string;
  isShowSaveDetailsButton?: boolean;
  saveDetailsText?: string;
  onConfirm?: () => void;
  onCancel?: () => void;
  onClose?: () => void;
  onSaveDetails?: () => void;
}

export const useAlert = (config: Partial<AlertConfig> = {}) => {
  const DEFAULT_CONFIG = {
    id: `alert-${uuidv4()}`,
    type: AlertType.ALERT,
    visible: true,
  };

  const allConfig: AlertConfig = {
    ...DEFAULT_CONFIG,
    ...config,
  };

  // Create container in body
  const containerClass = "AlertContainer";
  let container = document.querySelector(`body > .${containerClass}`);

  if (!container) {
    container = document.createElement("div");
    container.classList.add(containerClass);
    document.body.appendChild(container);
  }

  // Create wrapper
  const wrapper = document.createElement("div");
  wrapper.id = allConfig.id;
  container.appendChild(wrapper);

  const getDefaultTitle = (type: AlertType): string => {
    switch (type) {
      case AlertType.CONFIRM:
        return "Confirm";

      case AlertType.SAVEDETAILS:
        return "SaveDetails";

      case AlertType.ALERT:
      default:
        return "Alert";
    }
  };

  const getActionButtons = (type: AlertType): any => {
    const ConfirmButton = h(
      Button,
      {
        themeColor: allConfig.confirmButtonThemeColor || "primary",
        fillMode: allConfig.confirmButtonFillMode,
        onClick: () => {
          allConfig?.onConfirm?.();
          dismiss();
        },
      },
      allConfig.confirmButtonText || "Ok",
    );

    const SavedDetailsButton = h(
      Button,
      {
        fillMode: "outline",
        themeColor: allConfig?.saveDetailsThemeColor || "primary",
        onClick: () => {
          allConfig?.onSaveDetails?.();
          dismiss();
        },
      },

      allConfig?.saveDetailsText || "Save Details",
    );

    const CancelButton = h(
      Button,
      {
        themeColor: allConfig.cancelButtonThemeColor || "error",
        fillMode: allConfig.cancelButtonFillMode,
        onClick: () => {
          allConfig?.onCancel?.();
          dismiss();
        },
      },
      allConfig.cancelButtonText || "Cancel",
    );

    switch (type) {
      case AlertType.CONFIRM:
        return [CancelButton, ConfirmButton];

      case AlertType.SAVEDETAILS:
        return [SavedDetailsButton, ConfirmButton];

      case AlertType.ALERT:
      default:
        return [ConfirmButton];
    }
  };

  const dismiss = () => {
    const wrapper = document.querySelector(`#${allConfig.id}`);
    wrapper?.remove();

    if (!container) {
      return;
    }

    document.body.removeChild(container);
  };

  const dialog = h(
    Dialog,
    {
      class: "Alert",
      width: allConfig.width,
      title: allConfig.title || getDefaultTitle(allConfig.type),
      onClose: () => {
        allConfig?.onClose?.();
        dismiss();
      },
    },
    [
      h("p", {}, config.content),
      config.additionalContent && h("p", {}, config.additionalContent),
      h(
        DialogActionsBar,
        { layout: "center" },
        getActionButtons(allConfig.type),
      ),
    ],
  );

  render(dialog, wrapper);
};
