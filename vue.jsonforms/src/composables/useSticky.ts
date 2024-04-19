import { useResizeObserver } from "@vueuse/core";

export interface StickyOptions {
  marginTop?: number;
  marginBottom?: number;
  breakpoint?: number;
  stickyClass?: string;
  parentSelector?: string;
  delay?: number;
}

export interface StickyRectangle {
  top: number;
  left: number;
  width: number;
  height: number;
}

export const useSticky = (
  targetElement: HTMLElement,
  containerElement?: HTMLElement | null,
  options: StickyOptions = {},
) => {
  let active: boolean;
  let marginTop: number;
  let marginBottom: number;
  let breakpoint: number;
  let stickyClass: string;
  let parentSelector: string;
  let delay: number;
  let rect: StickyRectangle;
  let parentElement: HTMLElement;
  let parentElementRect: StickyRectangle;
  let viewport: { width: number; height: number };
  let scrollTop = 0;
  let wrapperElement: HTMLElement;

  const setStyle = (
    element: HTMLElement,
    styles: Partial<CSSStyleDeclaration>,
  ) => {
    for (const style in styles) {
      if (Object.prototype.hasOwnProperty.call(styles, style)) {
        element.style[style] = `${styles[style]}`;
      }
    }
  };

  const setWrapper = (element: HTMLElement) => {
    element.insertAdjacentHTML("beforebegin", "<div></div>");
    element?.previousSibling?.appendChild?.(element);
    return element.parentNode as HTMLElement;
  };

  const getViewportSize = () => {
    return {
      width: Math.max(
        document.documentElement.clientWidth,
        window.innerWidth || 0,
      ),
      height: Math.max(
        document.documentElement.clientHeight,
        window.innerHeight || 0,
      ),
    };
  };

  const getRectangle = (element: HTMLElement): StickyRectangle => {
    setStyle(element, {
      position: "",
      width: "",
      top: "",
      left: "",
    });

    const width = Math.max(
      element.offsetWidth,
      element.clientWidth,
      element.scrollWidth,
    );
    const height = Math.max(
      element.offsetHeight,
      element.clientHeight,
      element.scrollHeight,
    );

    let top = 0;
    let left = 0;

    do {
      top += element.offsetTop || 0;
      left += element.offsetLeft || 0;
      element = element.offsetParent as HTMLElement;
    } while (element);

    return { top, left, width, height };
  };

  const getParentElement = (element: HTMLElement) => {
    let container = <HTMLElement>element.parentNode;

    while (
      parentSelector &&
      !container.matches(parentSelector) &&
      container !== document.body
    ) {
      container = <HTMLElement>container.parentNode;
    }

    return container;
  };

  const updateScrollTop = () => {
    if (containerElement) {
      scrollTop =
        (containerElement?.scrollTop || 0) + (containerElement?.clientTop || 0);
    } else {
      scrollTop = window.scrollY || document.documentElement.scrollTop || 0;
    }
  };

  const resizeEvent = () => {
    viewport = getViewportSize();

    rect = getRectangle(targetElement);
    parentElementRect = getRectangle(parentElement);

    if (
      rect.top + rect.height <
        parentElementRect.top + parentElementRect.height &&
      breakpoint < viewport.width &&
      !active
    ) {
      active = true;
    } else if (
      rect.top + rect.height >=
        parentElementRect.top + parentElementRect.height ||
      (breakpoint >= viewport.width && active)
    ) {
      active = false;
    }

    setPosition();
  };

  const scrollEvent = () => {
    if (active) {
      setPosition();
    }
  };

  const setPosition = () => {
    setStyle(targetElement, { position: "", width: "", top: "", left: "" });

    if (viewport.height < rect.height || !active) {
      return;
    }

    if (!rect.width) {
      rect = getRectangle(targetElement);
    }

    if (rect.top === 0 && parentElement === document.body) {
      setStyle(targetElement, {
        position: "fixed",
        top: rect.top + "px",
        left: rect.left + "px",
        width: rect.width + "px",
      });

      if (stickyClass) {
        targetElement.classList.add(...stickyClass.trim().split(/\s+/));
      }

      wrapperElement.style.height = `${targetElement.clientHeight}px`;
    } else if (scrollTop > rect.top - marginTop) {
      setStyle(targetElement, {
        position: "fixed",
        width: rect.width + "px",
        left: rect.left + "px",
      });

      if (
        scrollTop + rect.height + marginTop >
        parentElementRect.top + parentElement.offsetHeight - marginBottom
      ) {
        if (stickyClass) {
          targetElement.classList.remove(...stickyClass.trim().split(/\s+/));
        }

        wrapperElement.style.height = "";

        setStyle(targetElement, {
          top:
            parentElementRect.top +
            parentElement.offsetHeight -
            (scrollTop + rect.height + marginBottom) +
            "px",
        });
      } else {
        if (stickyClass) {
          targetElement.classList.add(...stickyClass.trim().split(/\s+/));
        }

        wrapperElement.style.height = `${targetElement.clientHeight}px`;

        setStyle(targetElement, { top: marginTop + "px" });
      }
    } else {
      if (stickyClass) {
        targetElement.classList.remove(...stickyClass.trim().split(/\s+/));
      }

      wrapperElement.style.height = "";

      setStyle(targetElement, { position: "", width: "", top: "", left: "" });
    }
  };

  const render = async (options: StickyOptions) => {
    if (!targetElement) {
      return;
    }

    active = false;
    marginTop = options.marginTop || 0;
    marginBottom = options.marginBottom || 0;
    breakpoint = options.breakpoint || 0;
    stickyClass = options.stickyClass || "";
    parentSelector = options.parentSelector || "";
    delay = options.delay || 0;

    if (delay) {
      await new Promise((resolve) => setTimeout(resolve, delay));
    }

    rect = getRectangle(targetElement);
    parentElement = getParentElement(targetElement);
    parentElementRect = getRectangle(parentElement);
    wrapperElement = setWrapper(targetElement);

    if (
      rect.top + rect.height <
        parentElementRect.top + parentElementRect.height &&
      breakpoint < viewport.width &&
      !active
    ) {
      active = true;
    }

    if (containerElement) {
      containerElement.addEventListener("resize", resizeEvent);
      containerElement.addEventListener("scroll", scrollEvent);
    } else {
      window.addEventListener("resize", resizeEvent);
      window.addEventListener("scroll", scrollEvent);
    }

    // Reposition when parentElement has been resized
    useResizeObserver(parentElement, () => {
      refresh();
    });
  };

  const refresh = () => {
    rect = getRectangle(targetElement);
    parentElementRect = getRectangle(parentElement);
    setPosition();
  };

  const activate = () => {
    if (!targetElement) {
      return;
    }

    viewport = getViewportSize();
    updateScrollTop();

    if (containerElement) {
      containerElement.addEventListener("load", updateScrollTop);
      containerElement.addEventListener("scroll", updateScrollTop);
    } else {
      window.addEventListener("load", updateScrollTop);
      window.addEventListener("scroll", updateScrollTop);
    }

    render(options);
  };

  const deactivate = () => {
    if (containerElement) {
      containerElement.removeEventListener("load", updateScrollTop);
      containerElement.removeEventListener("scroll", updateScrollTop);
      containerElement.removeEventListener("resize", resizeEvent);
      containerElement.removeEventListener("scroll", scrollEvent);
    } else {
      window.removeEventListener("load", updateScrollTop);
      window.removeEventListener("scroll", updateScrollTop);
      window.removeEventListener("resize", resizeEvent);
      window.removeEventListener("scroll", scrollEvent);
    }
  };

  const reactivate = () => {
    deactivate();
    activate();
  };

  return {
    activate,
    deactivate,
    reactivate,
    refresh,
  };
};
