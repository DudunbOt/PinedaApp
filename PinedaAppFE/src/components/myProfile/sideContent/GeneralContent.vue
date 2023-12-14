<template>
  <div class="side-content">
    <div class="side-content-main">{{ title }}</div>
    <div class="side-content-sub">{{ content }}</div>
  </div>
</template>

<script>
export default {
  props: {
    title: String,
    content: String,
  },
};
</script>
<style scoped>
.side-content {
  margin-top: 20px;
}
.side-content-main {
  font-weight: bold;
}
</style>
