import {
  h,
  defineComponent,
  render,
  shallowRef,
  reactive,
  PropType,
} from "vue";
import { v4 as uuidv4 } from "uuid";
import {
  Notification,
  NotificationGroup,
} from "@progress/kendo-vue-notification";
import { Fade } from "@progress/kendo-vue-animation";

export enum NotificationType {
  DEFAULT = "default",
  SUCCESS = "success",
  ERROR = "error",
  WARNING = "warning",
  INFO = "info",
}

export enum NotificationArea {
  TOP_LEFT = "topLeft",
  BOTTOM_LEFT = "bottomLeft",
  TOP_CENTER = "topCenter",
  BOTTOM_CENTER = "bottomCenter",
  TOP_RIGHT = "topRight",
  BOTTOM_RIGHT = "bottomRight",
  CENTER = "center",
}

export interface NotificationConfig {
  id: string;
  type: NotificationType;
  content: string;
  area: NotificationArea;
  visible: boolean;
  interval: number;
  closeable?: boolean;
}

const notifications = shallowRef<NotificationConfig[]>([]);

export const useNotification = (config: Partial<NotificationConfig>) => {
  const DEFAULT_CONFIG = {
    id: `notification-${uuidv4()}`,
    type: NotificationType.DEFAULT,
    content: "",
    area: NotificationArea.CENTER,
    visible: true,
    closeable: false,
    interval: 5e3,
  };

  const allConfig: NotificationConfig = {
    ...DEFAULT_CONFIG,
    ...config,
  };

  const Body = document.body;

  notifications.value = [...notifications.value, allConfig];

  if (allConfig.interval) {
    setTimeout(() => {
      updateNotification(allConfig.id, { visible: false });
    }, allConfig.interval);
  }

  const updateNotification = (
    id: string,
    payload: Partial<NotificationConfig>
  ) => {
    notifications.value = notifications.value.map((notification) => {
      if (notification.id === id) {
        return {
          ...notification,
          ...payload,
        };
      }

      return notification;
    });
  };

  const removeNotification = (id: string) => {
    notifications.value = notifications.value.filter(
      (notification) => notification.id !== id
    );
  };

  const Notifications = defineComponent({
    props: {
      area: String as PropType<NotificationArea>,
    },
    setup(props) {
      return () =>
        notifications.value
          .filter((item) => item.area === props.area)
          .map((item) =>
            h(
              Fade,
              {
                appear: item.visible,
                onExited() {
                  removeNotification(item.id);
                },
              },
              () =>
                h(
                  Notification,
                  {
                    class: "Notification",
                    type: { style: item.type as any, icon: true },
                    closable: item.closeable,
                    onClose() {
                      updateNotification(item.id, { visible: false });
                    },
                  },
                  () => h("span", { innerHTML: item.content })
                )
            )
          );
    },
  });

  const NotificationGroups = defineComponent({
    setup() {
      const position = reactive({
        topLeft: { top: 0, left: 0, alignItems: "flex-start" },
        topCenter: { top: 0, left: "50%", transform: "translateX(-50%)" },
        topRight: { top: 0, right: 0, alignItems: "flex-end" },
        bottomLeft: { bottom: 0, left: 0, alignItems: "flex-start" },
        bottomCenter: { bottom: 0, left: "50%", transform: "translateX(-50%)" },
        bottomRight: { bottom: 0, right: 0, alignItems: "flex-end" },
        center: { top: "50%", left: "50%", transform: "translate(-50%, -50%)" },
      });
      return () =>
        Object.values(NotificationArea).map((area) => {
          const name = `NotficationArea-${area}`;
          return h(
            NotificationGroup,
            { key: name, class: [name, "p-2"], style: position[area] },
            () => h(Notifications, { area })
          );
        });
    },
  });

  !document.querySelector("[class*=NotficationArea-]") &&
    render(h(NotificationGroups), Body);
};
