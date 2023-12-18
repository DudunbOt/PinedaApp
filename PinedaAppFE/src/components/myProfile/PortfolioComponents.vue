<template>
  <b-card
    :title="this.content.name"
    :img-src="this.imgUrl"
    style="max-width: 15rem"
    img-top
    class="mb-2"
    :sub-title="this.content.description"
    align="center"
  >
  </b-card>
</template>

<script>
const baseUrl = import.meta.env.VITE_MEDIA_URL;
export default {
  props: {
    content: Object,
  },
  computed: {
    imgUrl() {
      return baseUrl + "/Portfolio/" + this.content.imageFilePath;
    }
  }
};
</script>
