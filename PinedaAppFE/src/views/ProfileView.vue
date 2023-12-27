<script setup></script>
<template>
  <div>
    <b-container>
      <b-row class="mt-2">
        <b-col cols="12" md="3">
          <b-row class="w-100 h-100 bg-dark text-white" align-content="start">
            <b-col cols="12" class="mb-4 center-image">
              <b-img
                class="mt-2"
                :src="UserProfile.profilePicture"
                width="200"
                rounded="circle"
                fluid
                block
              ></b-img>
            </b-col>
            <b-col cols="12" class="pl-4 pr-0 mb-2">
              <div class="side-title">
                <p class="mb-0 mr-3">Contacts</p>
              </div>
              <SideGeneralContent
                :title="capitalize(getKey.phone)"
                :content="UserProfile.phone"
              />
              <SideGeneralContent
                :title="capitalize(getKey.email)"
                :content="UserProfile.email"
              />
              <SideGeneralContent
                :title="capitalize(getKey.address)"
                :content="UserProfile.address"
              />
            </b-col>
            <b-col cols="12" class="pl-4 pr-0 mb-2">
              <div class="side-title">
                <p class="mb-0 mr-3">{{ capitalize(getKey.academics) }}</p>
              </div>
              <div
                v-for="(academic, index) in UserProfile.academics"
                :key="index"
              >
                <SideContentWithDate :content="academic" />
              </div>
            </b-col>
            <b-col cols="12" class="pl-4 pr-0">
              <div class="side-title">
                <p class="mb-0 mr-3">Expertise</p>
              </div>
              <SideContentList :dataList="UserProfile.expertises" />
            </b-col>
            <b-col class="d-flex justify-content-center">
              <SideButton />
            </b-col>
          </b-row>
        </b-col>
        <b-col cols="12" md="9">
          <div class="w-100 p-2" id="">
            <p class="user-name">
              <b>{{ UserProfile.firstname }}</b> {{ UserProfile.lastname }}
            </p>
            <p class="job-title">{{ UserProfile.occupation }}</p>
            <p>{{ UserProfile.bio }}</p>
          </div>
          <div class="w-100 p-2" id="">
            <div class="section-title">
              {{ capitalize(getKey.experiences) }}
            </div>
            <div
              v-for="(experience, index) in UserProfile.experiences"
              :key="index"
              class="section-container"
            >
              <ExperienceComponent :content="experience" />
            </div>
          </div>
          <div class="w-100 p-2">
            <div class="section-title">{{ capitalize(getKey.portfolios) }}</div>
            <p>Some of the work i done in the past.</p>
            <b-row class="ml-1 mr-1" align-h="between">
              <b-col
                v-for="(portfolio, index) in UserProfile.portfolios"
                :key="index"
                cols="4"
              >
                <PortfolioComponent :content="portfolio" />
              </b-col>
            </b-row>
          </div>
        </b-col>
      </b-row>
    </b-container>
  </div>
</template>
<script>
import SideGeneralContent from "../components/myProfile/sideContent/GeneralContent.vue";
import SideContentWithDate from "../components/myProfile/sideContent/ContentWithDate.vue";
import SideContentList from "../components/myProfile/sideContent/ContentList.vue";
import SideButton from "../components/myProfile/sideContent/SideButton.vue";
import ExperienceComponent from "../components/myProfile/ExperienceComponents.vue";
import PortfolioComponent from "../components/myProfile/PortfolioComponents.vue";
import imagePath from "../assets/stock_image_pfp.png";

export default {
  methods: {
    async GetMyProfile() {
      this.$axios
        .get("/user/1")
        .then((response) => {
          this.bindData(response.data);
        })
        .catch((error) => {
          console.log(error);
        });
    },

    GetProfilePicture(profilePicture) {
      if (profilePicture == null) return imagePath;

      let fullpath =
        import.meta.env.VITE_BASE_URL +
        "/Users/ProfilePicture/" +
        profilePicture;

      return fullpath;
    },

    bindData(UserProfile) {
      this.UserProfile.firstname = UserProfile.firstName;
      this.UserProfile.lastname = UserProfile.lastName;
      this.UserProfile.email = UserProfile.email;
      this.UserProfile.phone = UserProfile.phone;
      this.UserProfile.address = UserProfile.address;
      this.UserProfile.academics = UserProfile.academics;
      this.UserProfile.experiences = UserProfile.experiences;
      this.UserProfile.portfolios = UserProfile.portfolios;
      this.UserProfile.occupation = UserProfile.occupation;
      this.UserProfile.bio = UserProfile.bio;
      this.UserProfile.profilePicture = this.GetProfilePicture(
        UserProfile.profilePicture
      );
      this.UserProfile.expertises = UserProfile.expertises;
    },

    capitalize(text) {
      return text.charAt(0).toUpperCase() + text.slice(1);
    },
  },
  data() {
    return {
      imagePath,
      UserProfile: {
        UserProfile: {},
        firstname: "",
        lastname: "",
        email: "",
        phone: "",
        address: "",
        academics: [],
        experiences: [],
        portfolios: [],
      },
    };
  },
  computed: {
    getKey() {
      let title = Object.keys(this.UserProfile);
      let newObj = {};

      title.forEach((key) => {
        newObj[key] = key;
      });

      return newObj;
    },
  },
  async mounted() {
    await this.GetMyProfile();
  },
};
</script>
<style lang="css" scoped>
.user-name {
  font-size: 60px;
  margin-bottom: -5px;
}
.job-title {
  font-size: 38px;
  letter-spacing: 0.25em;
}

.section-title {
  border-bottom: 1px solid black;
  font-size: 34px;
  font-weight: bold;
  margin-bottom: 20px;
}

.side-content {
  margin-top: 20px;
}

.side-title {
  font-size: 28px;
  border-bottom: 2px solid white;
  font-weight: bold;
}

.center-image {
  margin-top: 10px;
  display: flex;
  justify-content: center;
}
</style>
