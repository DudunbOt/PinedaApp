<template>
  <div class="side-content">
    <ul>
      <li v-for="(data, index) in dataList" :key="index">
        {{ data }}
      </li>
    </ul>
  </div>
</template>
<script>
export default {
  props: {
    dataList: Array,
  },
};
</script>
<style scoped>
.side-content {
  margin-top: 20px;
}
</style>
