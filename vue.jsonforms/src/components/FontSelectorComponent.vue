<script setup lang="ts">
const items = [
  {
    text: "Arial",
    value: "Arial, sans-serif",
  },
  {
    text: "Figtree",
    value: "Figtree, sans-serif",
  },
  {
    text: "Inter",
    value: "Inter, sans-serif",
  },
  {
    text: "Tahoma",
    value: "Tahoma, sans-serif",
  },
  {
    text: "Trebuchet MS",
    value: "'Trebuchet MS', sans-serif",
  },
  {
    text: "Verdana",
    value: "Verdana, sans-serif",
  },
];
</script>

<template>
  <KendoDropDownList
    :data-items="items"
    data-item-key="value"
    text-field="text"
    value-field="value"
    value-primitive
  >
  </KendoDropDownList>
</template>