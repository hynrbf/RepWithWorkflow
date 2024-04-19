<script lang="ts">
import { defineComponent } from "vue";
import KendoBasicInputComponent from "@/components/form-fields/KendoBasicInputComponent.vue";

export default defineComponent({
  name: "TestOnly",
  components: {
    KendoBasicInputComponent,
  },
  data() {
    return {
      urlVal: "",
      inputVal: "",
      isVisible: true,
      name: "",
      description: "",
      persons: [] as { id: string; name: string; age: number }[],
      hasPerson: false,
    };
  },
  mounted() {
    // this.companyInternal.firmName = "Richde";
  },
  methods: {
    async handleSubmitAsync() {},
    onInputName(event: any) {
      this.name = event.target.value;
    },
    onInputDescription(event: any) {
      this.description = event.target.value;
    },
    validateName(currentValue: string): string {
      let errorMessage = "Please enter a valid Name";

      if (!currentValue) {
        return errorMessage;
      }

      return "";
    },
    validateDescription(currentValue: string): string {
      let errorMessage = "Please enter a valid Description";

      if (!currentValue) {
        return errorMessage;
      }

      return "";
    },
    onAdd() {
      this.persons.push({
        id: `${this.persons?.length ?? 0}`,
        name: "",
        age: 1,
      });
    },
    onRemove(index: number) {
      this.persons.splice(index, 1);
    },
  },
});
</script>

<template>
  <div style="padding: 50px">
    <Form @submit="handleSubmitAsync">
      <FormElement>
        <StackLayout orientation="vertical" :gap="10">
          <KendoBasicInputComponent
            id="website"
            class="col-md-4"
            type="url"
            :name="`url`"
            label="Website"
            :isRequired="true"
            :value="urlVal"
            @onValueChange="(value: string) => (urlVal = value)"
          />

          <KendoBasicInputComponent
            v-if="isVisible"
            class="col-md-4"
            :name="`nonUrl`"
            label="Any input"
            :isRequired="false"
            :value="inputVal"
            @onValueChange="(value: string) => (inputVal = value)"
          />

          <div class="mt-3 d-flex flex-column col-3 gap-2">
            <Button type="button" @click="isVisible = !isVisible"
              >{{ isVisible ? "Hide" : "Show" }} one field
            </Button>
            <!--            <Button type="submit" @click="isVisible = !isVisible"-->
            <!--              >Submit</Button-->
            <!--            >-->
          </div>
        </StackLayout>
      </FormElement>
    </Form>
  </div>

  <div class="px-4 col-6">
    <!--    <h3>Basic Inputs</h3>-->
    <!--    <Form>-->
    <!--      <FormElement>-->
    <!--        <KendoBasicInputComponent />-->
    <!--        <Field-->
    <!--          name="name"-->
    <!--          id="name"-->
    <!--          :validator="validateName"-->
    <!--          :component="'myTemplate'"-->
    <!--        >-->
    <!--          <template v-slot:myTemplate="{ props }">-->
    <!--            <StackLayout orientation="vertical" :gap="10">-->
    <!--              <Label :editor-id="props.id" class="control-label">-->
    <!--                Name is {{ name-->
    <!--                }}<span class="isRequiredAsterisk" style="color: red">*</span>-->
    <!--              </Label>-->

    <!--              <Input-->
    <!--                v-bind="props"-->
    <!--                :id="props.id"-->
    <!--                :name="props.name"-->
    <!--                :valid="props.valid"-->
    <!--                :value="name"-->
    <!--                @input="onInputName"-->
    <!--              />-->

    <!--              <Error v-if="props.touched && props.validationMessage"-->
    <!--                >{{ props.validationMessage }}-->
    <!--              </Error>-->
    <!--            </StackLayout>-->
    <!--          </template>-->
    <!--        </Field>-->

    <!--        <Field v-if="isVisible"-->
    <!--          name="description"-->
    <!--          id="description"-->
    <!--          :validator="validateDescription"-->
    <!--          :component="'myTemplate'"-->
    <!--        >-->
    <!--          <template v-slot:myTemplate="{ props }">-->
    <!--            <StackLayout orientation="vertical" :gap="10">-->
    <!--              <Label :editor-id="props.id" class="control-label">-->
    <!--                Description is {{ description-->
    <!--                }}<span class="isRequiredAsterisk" style="color: red">*</span>-->
    <!--              </Label>-->

    <!--              <Input-->
    <!--                v-bind="props"-->
    <!--                :id="props.id"-->
    <!--                :name="props.name"-->
    <!--                :valid="props.valid"-->
    <!--                :value="description"-->
    <!--                @input="onInputDescription"-->
    <!--              />-->

    <!--              <Error v-if="props.touched && props.validationMessage"-->
    <!--                >{{ props.validationMessage }}-->
    <!--              </Error>-->
    <!--            </StackLayout>-->
    <!--          </template>-->
    <!--        </Field>-->

    <!--        <Button class="mt-4" type="button" @click="isVisible = !isVisible">{{ isVisible ? 'Hide' : 'Show' }} Description</Button>-->
    <!--      </FormElement>-->
    <!--    </Form>-->
  </div>

  <div>
    <!--    <Form>-->
    <!--      <FormElement>-->
    <!--        <div v-if="hasPerson">-->
    <!--          <div v-for="(person, index) of persons" :key="person.id">-->
    <!--            <div class="mx-4 d-flex gap-4 justify-content-start">-->
    <!--              <KendoBasicInputComponent-->
    <!--                class="col-md-4"-->
    <!--                :id="`person[${index}]-name`"-->
    <!--                :name="`person[${index}]-name`"-->
    <!--                :label="`Name (${person.id})`"-->
    <!--                :isRequired="true"-->
    <!--                :value="person?.name"-->
    <!--                @onValueChange="(value: string) => (person.name = value)"-->
    <!--              />-->

    <!--              <KendoBasicInputComponent-->
    <!--                class="col-md-4"-->
    <!--                :id="`person[${index}]-age`"-->
    <!--                :name="`person[${index}]-age`"-->
    <!--                label="Age"-->
    <!--                :isRequired="true"-->
    <!--                :value="person?.age?.toString() ?? '0'"-->
    <!--                @onValueChange="-->
    <!--                  (value: string) => (person.age = parseInt(value))-->
    <!--                "-->
    <!--              />-->

    <!--              <Button-->
    <!--                class="align-self-end"-->
    <!--                type="button"-->
    <!--                @click="onRemove(index)"-->
    <!--                >Remove</Button-->
    <!--              >-->
    <!--            </div>-->
    <!--          </div>-->

    <!--          <Button class="mt-4" type="button" @click="onAdd">Add Person</Button>-->
    <!--        </div>-->

    <!--        <Button class="mt-4" type="button" @click="hasPerson = !hasPerson"-->
    <!--          >{{ hasPerson ? "Hide" : "Show" }} Person list-->
    <!--        </Button>-->
    <!--      </FormElement>-->
    <!--    </Form>-->
  </div>
</template>

<style scoped />