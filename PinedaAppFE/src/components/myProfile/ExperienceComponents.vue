<template>
  <div>
    <div class="section-checkpoint"></div>
    <div class="section-content">
      <div class="section-content-date">
        {{ this.startDate }} - {{ this.endDate }}
      </div>
      <div class="section-content-location">
        {{ this.content.companyName }} | {{ this.content.position }}
      </div>
      <div class="">{{ this.content.shortDesc }}</div>
      <div
        class="section-content-detail p-3"
        v-for="(project, index) in this.projects"
        :key="index"
      >
        <div class="font-weight-bold">{{ project.projectName }}</div>
        <div class="">{{ project.projectDescription }}</div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: {
    content: Object,
  },

  methods: {
    getDateExperience(dateParam) {
      let date = new Date(dateParam);
      let year = date.getFullYear();
      let month = date.toLocaleDateString("default", { month: "long" });

      return year + " " + month;
    },
  },

  computed: {
    startDate() {
      let startDate = new Date(this.content.startDate);
      return this.getDateExperience(startDate);
    },

    endDate() {
      let endDate = this.content.endDate;
      if (endDate === null) return "Until Now";
      return this.getDateExperience(endDate);
    },
  },
};
</script>
<style scoped>
.section-checkpoint {
  position: relative;
  top: calc(50% - 25px);
  left: -10px;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 1px solid black;
}

.section-content {
  padding-left: 10px;
  border-left: 1px solid black;
  margin-bottom: 25px;
}

.section-content:last-of-type {
  margin-bottom: 0px;
}

.section-content-date {
  font-weight: bold;
  font-size: 18px;
}

.section-content-location {
  font-size: 24px;
  font-weight: bold;
}
</style>
