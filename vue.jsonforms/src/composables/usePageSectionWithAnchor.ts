import {
  MaybeRef,
  isRef,
  nextTick,
  onBeforeUnmount,
  onMounted,
  ref,
} from "vue";

export interface PageSection {
  id: string;
  top: number;
}

export interface Position {
  x: number;
  y: number;
}

export const usePageSectionWithAnchor = (
  initialSections: MaybeRef<PageSection[]>,
  headerSelector = "",
  offset = 0,
) => {
  const pageSections = isRef(initialSections)
    ? initialSections
    : ref<PageSection[]>(initialSections);
  const mainContentElement = ref<HTMLElement | null>(null);
  const stickyHeaderHeight = ref(0);
  const isManualScroll = ref(true);

  const activePageSectionId = ref("");
  const sectionElementCurrentPosition = ref<Position>({ x: 0, y: 0 });

  const setPageSections = (sections: PageSection[]) => {
    pageSections.value = sections;

    if (pageSections.value.length) {
      activePageSectionId.value = pageSections.value[0].id;
    }

    setStickyHeaderHeight();
    setScrollMarginTopStyle();
  };

  const setStickyHeaderHeight = () => {
    if (!mainContentElement.value) {
      return;
    }

    const stickyHeader = mainContentElement.value.querySelector(headerSelector);

    if (!stickyHeader) {
      return;
    }

    nextTick(() => {
      const rect = stickyHeader.getBoundingClientRect();
      stickyHeaderHeight.value = rect?.height ?? 0;
    });
  };

  const setScrollMarginTopStyle = () => {
    // auto set of scroll margin top
    nextTick(() => {
      pageSections.value.map((section) => {
        const sectionElement = document.getElementById(section.id);

        if (!sectionElement) {
          return;
        }
        sectionElement.style.scrollMarginTop = `${
          stickyHeaderHeight.value + offset
        }px`;
      });
    });
  };

  const updatePageSectionTop = (): void => {
    pageSections.value.forEach((section) => {
      const sectionElement = document.getElementById(section.id);

      if (mainContentElement.value) {
        const parentTop =
          mainContentElement.value.getBoundingClientRect().top ?? 0;
        const childTop = sectionElement?.getBoundingClientRect()?.top ?? 0;
        section.top = childTop - parentTop;
      }
    });
  };

  const scrollToPageSection = (sectionId: string): void => {
    const sectionElement = document.getElementById(sectionId);

    if (!sectionElement) {
      return;
    }

    isManualScroll.value = false;
    sectionElement.scrollIntoView({ behavior: "auto" });
    activePageSectionId.value = sectionId;
    // a bit of delay to make sure scrolling is done.
    setTimeout(() => (isManualScroll.value = true), 100);
  };

  const getPositionY = (sectionId: string): number => {
    const parent = mainContentElement.value;
    const element = document.getElementById(sectionId);

    if (!(element instanceof HTMLElement) || !(parent instanceof HTMLElement)) {
      return 0;
    }

    const parentRect = parent.getBoundingClientRect();
    const childRect = element.getBoundingClientRect();
    return childRect.top - parentRect.top;
  };

  const getPositionX = (sectionId: string): number => {
    const parent = mainContentElement.value;
    const element = document.getElementById(sectionId);

    if (!(element instanceof HTMLElement) || !(parent instanceof HTMLElement)) {
      return 0;
    }

    const parentRect = parent.getBoundingClientRect();
    const childRect = element.getBoundingClientRect();
    return childRect.left - parentRect.left;
  };

  const handleScroll = (): void => {
    if (!mainContentElement.value) {
      return;
    }

    if (!(mainContentElement.value instanceof HTMLElement)) {
      return;
    }

    if (!pageSections) {
      return;
    }

    for (let i = pageSections.value.length - 1; i >= 0; i--) {
      const section = pageSections.value[i];
      const yPos = getPositionY(section.id);
      const xPos = getPositionX(section.id);

      sectionElementCurrentPosition.value.y = yPos;
      sectionElementCurrentPosition.value.x = xPos;

      //when the position of the element went to zero meaning the section element top y pos hits sticky header
      //then we make it the active one. the 1 value is just offset
      const remainingSpaceDifference = stickyHeaderHeight.value + 1;

      if (yPos > remainingSpaceDifference) {
        continue;
      }

      if (activePageSectionId.value === section.id) {
        break;
      }

      if (isManualScroll.value) {
        activePageSectionId.value = section.id;
        break;
      }
    }
  };

  onMounted(() => {
    mainContentElement.value = document.querySelector(".MainContent");

    if (!(mainContentElement.value instanceof HTMLElement)) {
      return;
    }

    mainContentElement.value.addEventListener("scroll", handleScroll);
    updatePageSectionTop();

    if (pageSections.value.length) {
      activePageSectionId.value = pageSections.value[0].id;
    }

    setStickyHeaderHeight();
    setScrollMarginTopStyle();
  });

  onBeforeUnmount(() => {
    mainContentElement.value?.removeEventListener("scroll", handleScroll);
  });

  return {
    activePageSectionId,
    sectionElementCurrentPosition,
    setPageSections,
    scrollToPageSection,
    updatePageSectionTop,
    stickyHeaderHeight,
  };
};
