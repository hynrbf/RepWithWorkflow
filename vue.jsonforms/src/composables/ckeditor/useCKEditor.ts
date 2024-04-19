import { h, defineComponent, onMounted, ref, PropType } from "vue";
import { createEventHook } from "@vueuse/core";
import { initialize as initClassicEditorWithTrackChanges } from "./editors/ClassicWithTrackChangesEditor";
import { Editor } from "ckeditor5/src/core";

type EditorType = "ClassicEditor" | "ClassicWithTrackChangesEditor";

export const useCKEditor = (editorType: EditorType) => {
  const initEditor = createEventHook<Editor>();

  const init = (
    element: HTMLElement,
    initialData?: string,
    config?: Record<string, unknown>,
  ) => {
    switch (editorType) {
      case "ClassicWithTrackChangesEditor":
        initClassicEditorWithTrackChanges(element, initialData, config).then(
          (response) => {
            initEditor.trigger(response as Editor);
          },
        );
        break;
      default:
      // @TODO Initialize default editor
    }
  };

  const Editor = defineComponent({
    props: {
      initialData: {
        type: String,
        default: "",
      },
      config: {
        type: Object as PropType<Record<string, unknown>>,
        default: () => {
          return {};
        },
      },
    },
    setup(props) {
      const element = ref();

      onMounted(() => {
        init(element.value, props.initialData, props.config);
      });

      return {
        element,
      };
    },
    render() {
      return h("div", { ref: "element" });
    },
  });

  return { Editor, onInit: initEditor.on };
};
