export const useElementHelpers = () => {
  // Get all previous siblings of an element
  const getPrevAll = (element: Element) => {
    const prevElements = [];
    let prevElement = element.parentNode?.firstElementChild;

    if (!prevElement) {
      return [];
    }

    while (prevElement !== element) {
      prevElements.push(prevElement);
      prevElement = prevElement?.nextElementSibling;
    }

    return prevElements;
  };

  return {
    getPrevAll,
  };
};
