<template>
  <div class="p-5">
    <KendoForm>
      <KendoFormElementComponent v-slot="{ form }">
        {{ form.values }}
        <KendoCountryComponent label="Country" name="country" :value="form.values.country" />
      </KendoFormElementComponent>
    </KendoForm>
  </div>
</template>