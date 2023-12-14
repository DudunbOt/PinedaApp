import Vue from "vue";
import { BootstrapVue, IconsPlugin } from "bootstrap-vue";

import App from "./App.vue";
import router from "./router";
import axiosInstance from "./services/api";

import "./assets/main.css";
import "bootstrap/dist/css/bootstrap.css";
import "bootstrap-vue/dist/bootstrap-vue.css";

Vue.use(BootstrapVue);
Vue.use(IconsPlugin);

router.beforeEach((to, from, next) => {
  // to and from are both route objects. must call `next`.
  document.title = to.meta.title || "Pineda App";
  next();
});

new Vue({
  router,
  render: (h) => h(App),
}).$mount("#app");

Vue.prototype.$axios = axiosInstance;
