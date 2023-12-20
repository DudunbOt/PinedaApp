<template>
  <div class="side-content">
    <span
      class="badge badge-info"
      v-for="(data, index) in dataList"
      :key="index"
    >
      {{ data }}
    </span>
    <!-- <ul>
      <li v-for="(data, index) in dataList" :key="index">
        {{ data }}
      </li>
    </ul> -->
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
  margin-bottom: 10px;
}
.badge {
  font-size: 16px;
  margin: 2px;
}
</style>
