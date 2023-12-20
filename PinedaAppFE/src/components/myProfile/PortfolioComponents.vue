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
const baseUrl = import.meta.env.VITE_BASE_URL;
import imagePath from "../../assets/stock_image.jpeg";
export default {
  props: {
    content: Object,
  },
  computed: {
    imgUrl() {
      if (this.content.imageFilePath == null) return imagePath;
      return baseUrl + "/Assets/Portfolio/" + this.content.imageFilePath;
    },
  },
};
</script>
