<template>
  <div class="side-content">
    <div class="side-content-date">{{ content.endDate.substring(0, 4) }}</div>
    <div class="side-content-main">{{ content.degree }}</div>
    <div class="side-content-sub">{{ content.schoolName }}</div>
  </div>
</template>
<script>
export default {
  props: {
    content: Object,
  },
};
</script>
<style scoped>
.side-content {
  margin-top: 20px;
}
</style>
