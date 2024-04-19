import { App } from "vue";
import stickyDirective from "./stickyDirective";
import devOnlyDirective from "./devOnlyDirective";

export const customDirectives = {
  install(app: App) {
    app.directive("sticky", stickyDirective);
    app.directive("devOnly", devOnlyDirective);

    // Initialize more custom directives here ...
  },
};
