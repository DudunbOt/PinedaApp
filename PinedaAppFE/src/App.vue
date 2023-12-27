<script setup>
import NavBar from "./components/NavBar.vue";
</script>

<template>
  <div id="app">
    <NavBar v-if="this.currentRoutePath" />
    <router-view />
  </div>
</template>
<script>
export default {
  components: {
    NavBar,
  },
  computed: {
    currentRoutePath() {
      let routePath = this.$route.path;
      let viewWithNavbar = ["/", "/profile"];
      return !viewWithNavbar.includes(routePath);
    },
  },
};
</script>
<style scoped></style>
