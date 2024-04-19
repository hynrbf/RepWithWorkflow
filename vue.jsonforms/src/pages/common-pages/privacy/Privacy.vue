<!--
ToDo. issue in router/index.ts because we can't do like this
const routes = [
    ...
    {
        path: '/privacy-notice',
        name: 'Privacy Notice',
        component: PrivacyNotice
    },
    {
        path: '/portal',
        name: 'Portal',
        component: Portal,
        props: (route: RouteLocationNormalizedLoaded) => ({
            email: route.query.email
        }),
        children: [
            ...
            {
                path: '/privacy-notice',
                name: 'Privacy Notice',
                component: PrivacyNotice
            }
        ]
    }
]
-->
<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "Privacy",
});
</script>

<template>
  <div class="container row-12 mt-5">
    <div class="col-12">
      <h1>Private Notice</h1>

      <p class="text-dark lh-base">
        Our privacy notice informs you of what to expect when the Financial
        Conduct Authority (FCA) collects information about individuals (which we
        will call ‘personal data’ in this privacy notice). The FCA is the
        controller responsible for the processing of your personal data.
        <br />
        <br />
        To help you understand how we use personal data across the FCA, in this
        privacy notice we explain core activities that we undertake and how we
        may use the personal data that we collect to carry out these activities.
        We also provide information about your rights and how to contact us if
        you have any questions.
        <br />
        <br />
        The FCA may at times process personal data as a joint controller (such
        as, under section 166 of FSMA, where we make joint decisions around
        processing personal data with Skilled Persons or with the Prudential
        Regulation Authority (PRA)) or as a separate controller (where we share
        personal data) with other authorities (such as the PRA).
        <br />
        <br />
        In most cases, individuals can still exercise their rights in relation
        to the personal data we process as set out below. Where we will allocate
        responsibilities between the controllers and we will, if necessary,
        notify or redirect individuals to other authority or controller, in
        relation to exercising their individual rights.
      </p>
    </div>
  </div>
</template>

<style scoped>
.container {
  padding: 20px;
  background-color: #ffffff;
  width: 50%;
}

h1 {
  padding-bottom: 10px;
  color: rgb(10, 10, 10);
}

p {
  font-size: var(--font-size-default);
  text-align: justify;
  margin-top: 20px;
  margin-bottom: 10px;
  padding-bottom: 10px;
}
</style>