/// <reference types="vite/client" />
//ToDo. research on this if this will affect in live version
declare module "*.vue" {
  import type {DefineComponent} from "vue";
  // ToDo. part of 18 IMPT errors to fix
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const component: DefineComponent<object, object, any>;
  export default component;
}

declare module "sticky-js";
