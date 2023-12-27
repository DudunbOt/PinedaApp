<template>
  <div class="center-container">
    <div class="w-25 form-login">
      <h1 class="text-center box-underline">Pineda App</h1>
      <b-form @submit.prevent="login">
        <b-form-group
          id="input-group-1"
          label="Username"
          label-for="input-1"
          required
        >
          <b-form-input
            id="input-1"
            v-model="form.Username"
            type="text"
            required
          ></b-form-input>
        </b-form-group>

        <b-form-group id="input-group-2" label="Password" label-for="input-2">
          <b-form-input
            id="input-2"
            v-model="form.Password"
            type="password"
            required
          ></b-form-input>
        </b-form-group>

        <b-button type="submit" variant="primary" class="w-100">
          Submit
        </b-button>
      </b-form>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      form: {
        Username: "",
        Password: "",
      },
    };
  },
  methods: {
    async login() {
      try {
        const response = await this.$axios.post("/user/login", {
          username: this.form.Username,
          password: this.form.Password,
        });

        const token = response.data.token;
        localStorage.setItem("token", token);

        this.$router.push("/home");
      } catch (error) {
        console.error("login failed");
      }
    },
  },
};
</script>

<style scoped>
.center-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
}

.form-login {
  padding: 10px;
}

.box-underline {
  border-bottom: 1px solid black;
  padding-bottom: 5px;
  margin-bottom: 10px;
}
</style>
