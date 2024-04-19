//set values in .env or ,env-development files
export const REMOTE_API: string = import.meta.env.VITE_REMOTE_API;
export const AUTH0_TOKEN_URL: string = import.meta.env.VITE_AUTH0_TOKEN_URL;
export const USE_REMOTE_DB: boolean = JSON.parse(
  import.meta.env.VITE_USE_REMOTE_DB,
);
export const FCA_URL: string = import.meta.env.VITE_FCA_URL;
export const CH_URL: string = import.meta.env.VITE_COMPANY_HOUSE_URL;
export const TINY_MCE_API_KEY: string = import.meta.env.VITE_TINYMCE_API_KEY;
export const APP_CACHE_ENABLE: boolean = JSON.parse(
  import.meta.env.VITE_APP_CACHE_ENABLE,
);
export const SECURE_APP_ENABLE: boolean = JSON.parse(
  import.meta.env.VITE_SECURE_ENABLE,
);
export const LANDING_URL: string = import.meta.env.VITE_LANDING_URL;
export const APP_VERSION: string = import.meta.env.VITE_APP_VERSION;
export const SIGNUP_USER: string = import.meta.env.VITE_SIGNUP_USER;
export const CKEDITOR_LICENSE_KEY: string = import.meta.env
  .VITE_CKEDITOR_LICENSE_KEY;
export const AUTOSAVE_ENABLED: boolean = JSON.parse(
  import.meta.env.VITE_AUTOSAVE_ENABLED,
);
