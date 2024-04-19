import { Directive } from "vue";
import { useSticky, StickyOptions } from "@/composables/useSticky";
import { AppConstants } from "@/infra/AppConstants";

export default <Directive<HTMLElement, StickyOptions>>{
  mounted(element: HTMLElement, { value }, vnode: any) {
    const { activate, deactivate, refresh } = useSticky(
      element,
      document.querySelector(".MainContent") as HTMLElement,
      value
    );
    activate();
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
  unmounted(element: HTMLElement, vnode: any) {
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
