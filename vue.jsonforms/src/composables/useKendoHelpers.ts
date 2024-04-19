import setWith from "lodash/setWith";

export const useKendoHelpers = () => {
  const formValueToObject = (kendoForm: any, name: string) => {
    if (!name.includes(".")) {
      return;
    }

    const values = kendoForm.values || [];
    const names = name.split(".");
    const currentName = names.shift();
    const currentValue = kendoForm.values?.[`${currentName}`];

    kendoForm.onChange(currentName, {
      value: {
        ...setWith(currentValue || {}, names, values[name], Object),
      },
    });
  };

  return {
    formValueToObject,
  };
};
