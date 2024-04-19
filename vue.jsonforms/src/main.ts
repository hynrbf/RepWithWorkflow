import { createApp } from "vue";
import App from "./App.vue";
import router from "./infra/router";
import { createAuth0 } from "@auth0/auth0-vue";
import { LANDING_URL } from "./config";
import mitt from "mitt";
import { createPinia } from "pinia";
import piniaPluginPersistedstate from "pinia-plugin-persistedstate";
import { svgSpritePlugin } from "vue-svg-sprite";
import { createI18n } from "vue-i18n";
import { customDirectives } from "@/directives";
import EN from "@/assets/json/localization.json";
import DE from "@/assets/json/localization.de.json";

const app = createApp(App);

// Initialize vue-svg-sprite
// Reference: https://www.npmjs.com/package/vue-svg-sprite
app.use(svgSpritePlugin, {
  url: "sprite/sprite-icons.svg",
  class: "",
});

// Initialize Pinia
const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);
app.use(pinia);

//do not put @ sign in localization.json. open issue https://github.com/intlify/vue-i18n-next/issues/118#issuecomment-698382032
app.use(
  createI18n({
    locale: "EN",
    fallbackLocale: "EN",
    messages: {
      EN: EN,
      DE: DE,
    },
  }),
);

// ref https://morioh.com/p/f1bc7c5f9ca4
// Create an event bus instance
const eventBus = mitt();
app.provide("$eventBusService", eventBus);
app.provide("alertService", useAlert);

// Initialize custom directives
app.use(customDirectives);

//ToDo. remove sentry for now, will put some error recording soon...
// //record error
// Sentry.init({
//   app,
//   dsn: "https://a0243a7c58774a038d7e0fda3604e484@o4505441706901504.ingest.sentry.io/4505441708539904",
//   integrations: [
//     new Sentry.BrowserTracing({
//       routingInstrumentation: Sentry.vueRouterInstrumentation(router),
//     }),
//     new Sentry.Replay(),
//   ],
//   // Performance Monitoring
//   tracesSampleRate: 1.0, // Capture 100% of the transactions, reduce in production!
//   // Session Replay
//   replaysSessionSampleRate: 0.1, // This sets the sample rate at 10%. You may want to change it to 100% while in development and then sample at a lower rate in production.
//   replaysOnErrorSampleRate: 1.0, // If you're not already sampling the entire session, change the sample rate to 100% when sampling sessions where errors occur.
// });

//router and mount
app
  .use(router)
  .use(
    createAuth0({
      // tech@suntech.gi auth0
      domain: "dev-55zgp5t8ytxc6vx4.us.auth0.com",
      clientId: "KpTwp4t0PGvpbQR0UiX75I5aOkWkroZq", // staging
      // clientId: "0d8tU2PUQiAE0i1fn1qv55KGRNgcMVWk",    // live
      authorizationParams: {
        redirect_uri: LANDING_URL,
        scope: "openid profile email",
      },
    }),
  )
  .mount("#app");

//register Non-Async components
//this is automatically generate in common.d.ts
//however, listed here are the failure of the tool to generate these below.
import KendoDialogComponent from "./components/KendoDialog.vue";
import { useAlert } from "@/composables/useAlert";

app.component("KendoDialogComponent", KendoDialogComponent);
