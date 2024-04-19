import { customRef } from "vue";
import {
  LocationQueryValue,
  //ToDo. LocationQueryValueRaw,
  useRoute,
  //ToDo. useRouter,
} from "vue-router";
import isArray from "lodash/isArray";

type Value = string | number | boolean | null | Value[];

export const useUrlParamRef = <T = Value>(key: string, initialValue?: T) => {
  const route = useRoute();
  //const router = useRouter()

  const parseValue = (
    value: LocationQueryValue | LocationQueryValue[],
  ): Value => {
    if (isArray(value)) {
      return value.map((item) => parseValue(item));
    } else {
      try {
        const parsedValue = JSON.parse(String(value));
        return parsedValue as Value;
      } catch {
        return value;
      }
    }
  };

  const setValue = (_value: T) => {
    //ToDo. Move to service, do not push
    //router.push({
    //  query: {
    //    ...route.query,
    //    [key]: value as unknown as
    //      | LocationQueryValueRaw
    //      | LocationQueryValueRaw[],
    //  },
    //})
  };

  if (initialValue !== undefined) {
    setValue(initialValue);
  }

  return customRef<T>((track, trigger) => {
    return {
      get() {
        track();
        return parseValue(route.query[key]) as unknown as T;
      },
      set(newValue) {
        setValue(newValue);
        trigger();
      },
    };
  });
};