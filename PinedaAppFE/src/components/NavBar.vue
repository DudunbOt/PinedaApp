<template>
  <div>
    <b-navbar toggleable="sm" type="dark" variant="dark" class="p-1">
      <b-navbar-brand to="/">PinedaApp</b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
      <b-collapse id="nav-collapse" is-nav>
        <b-navbar-nav>
          <b-nav-item to="/home">Home</b-nav-item>
          <b-nav-item to="/about">About</b-nav-item>
        </b-navbar-nav>

        <!-- Right Aligned Item -->
        <b-navbar-nav class="ml-auto">
          <b-nav-item-dropdown text="User" right>
            <b-dropdown-item to="/profile">Profile</b-dropdown-item>
            <b-dropdown-item @click="logout">Logout</b-dropdown-item>
          </b-nav-item-dropdown>
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>
  </div>
</template>
<script>
export default {
  methods: {
    logout() {
      localStorage.removeItem("token");
      location.reload();
    },
  },
};
</script>
<style lang=""></style>
