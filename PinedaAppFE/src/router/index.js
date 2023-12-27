import Vue from "vue";
import VueRouter from "vue-router";
import HomeView from "../views/HomeView.vue";
import AboutView from "../views/AboutView.vue";
import LoginView from "../views/LoginView.vue";
// import ProfileView from "../views/ProfileView.vue";

Vue.use(VueRouter);

const router = new VueRouter({
  mode: "history",
  base: import.meta.env.BASE_URL,
  routes: [
    {
      path: "/",
      name: "Login",
      component: LoginView,
      meta: {
        title: "Welcome",
      },
      beforeEnter: (to, from, next) => {
        const token = localStorage.getItem("token");
        if (token) {
          next("/home");
        } else {
          next();
        }
      },
    },
    {
      path: "/home",
      name: "Home",
      component: HomeView,
      meta: {
        title: "HomeView",
      },
      beforeEnter: (to, from, next) => {
        const token = localStorage.getItem("token");
        if (!token) {
          next("/");
        } else {
          next();
        }
      },
    },
    {
      path: "/about",
      name: "About",
      component: AboutView,
      meta: {
        title: "About Me",
      },
      beforeEnter: (to, from, next) => {
        const token = localStorage.getItem("token");
        if (!token) {
          next("/");
        } else {
          next();
        }
      },
    },
    {
      path: "/profile",
      name: "Profile",
      component: () => import("../views/ProfileView.vue"),
      meta: {
        title: "Profile",
      },
    },
  ],
});

export default router;
