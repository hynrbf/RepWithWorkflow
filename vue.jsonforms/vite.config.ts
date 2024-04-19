import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import path from "path";
import Components from "unplugin-vue-components/vite";
import kendoUnpluginResolver from "./kendoUnpluginResolver";
import ckeditor5 from "@ckeditor/vite-plugin-ckeditor5";
import { createRequire } from "node:module";

const require = createRequire(import.meta.url);

// https://vitejs.dev/config/
// https://vitejs.dev/config/server-options.html
export default defineConfig({
  plugins: [
    vue(),
    Components({
      dirs: ["src/components"],
      resolvers: [kendoUnpluginResolver],
      // Empty array to make work on .html
      include: [
        // @TODO Find another way to make vue-unplugin-components works on .html
        // [/\.vue$/, /\.vue\?vue/],
      ],
    }),
    ckeditor5({ theme: require.resolve("@ckeditor/ckeditor5-theme-lark") }),
  ],
  resolve: {
    alias: [
      {
        find: /^~(.*)$/,
        replacement: "node_modules/$1",
      },
      {
        find: "@",
        replacement: path.resolve(__dirname, "./src"),
      },
    ],
  },
  // // cors, https://www.youtube.com/watch?v=hbcnZ_GokBA
  // // using proxy like this in cors cannot work in deployed version
  // // cors problem will exist in browser so the solution in to put calling into api
  // // see "useproxy.png"
  // server: {
  //     proxy: {
  //         '/companies-house': {
  //             target: 'https://api.company-information.service.gov.uk',
  //             changeOrigin: true,
  //             rewrite: (path) => path.replace(/^\/companies-house/, '')
  //         },
  //         '/fca': {
  //             target: 'https://register.fca.org.uk',
  //             changeOrigin: true,
  //             rewrite: (path) => path.replace(/^\/fca/, '')
  //         }
  //     }
  // }
});
