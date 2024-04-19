import { ref } from "vue";

export const usePageCompletionProgress = () => {
  const pageCompletionPercentage = ref(0);

  /*please note. in your component to set these :isValueReactive="true", :isDataLoadedCompletely="!isInitializing"
      otherwise, the count will be incorrect in page first load. please read the KendoGenericInput of how these
      properties fixed the issue.
    */
  const computePageCompletionProgress = (errors: Record<string, string>) => {
    const total = Object.values(errors).length;
    const partial = Object.values(errors).filter(
      (error) => error === "",
    ).length;
    const result = 100 * (partial / total);

    if (isNaN(result)) {
      return;
    }

    pageCompletionPercentage.value = +result.toFixed(2);
  };

  return {
    pageCompletionPercentage,
    computePageCompletionProgress,
  };
};
