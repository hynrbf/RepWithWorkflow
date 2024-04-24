import { Directive } from "vue";
import { useSticky, StickyOptions } from "@/composables/useSticky";
import { AppConstants } from "@/infra/AppConstants";

export default <Directive<HTMLElement, StickyOptions>>{
  // ToDo. part of 18 IMPT errors to fix
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  mounted(element: HTMLElement, { value }, vnode: any) {
    const { activate, deactivate, refresh } = useSticky(
      element,
      document.querySelector(".MainContent") as HTMLElement,
      value
    );
    activate();
    // ToDo. part of 18 IMPT errors to fix
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    (element as any).deactivateSticky = deactivate;

    if (!vnode?.ctx?.appContext) {
      return;
    }

    // access event bus from root
    const { $eventBusService } = vnode.ctx.appContext.provides;
    if ($eventBusService) {
      $eventBusService.on(AppConstants.sideMenuToggledEvent, () => {
        setTimeout(() => refresh(), 250);
      });
    }
  },
  // ToDo. part of 18 IMPT errors to fix
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  unmounted(element: HTMLElement, vnode: any) {
    // ToDo. part of 18 IMPT errors to fix
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const deactivate = (element as any).deactivateSticky;
    if (typeof deactivate === "function") {
      deactivate();
    }

    if (!vnode?.ctx?.appContext) {
      return;
    }

    // access event bus from root
    const { $eventBusService } = vnode.ctx.appContext.provides;
    if ($eventBusService) {
      $eventBusService.off(AppConstants.sideMenuToggledEvent);
    }
  },
};
