import { Directive } from "vue";

const IS_DEV = import.meta.env.DEV;

export default <Directive<HTMLElement>>{
  mounted(el) {
    if (!IS_DEV) {
      el.remove();
    }
  },
};
