import Axios from "axios";

const baseURL =
  process.env.NODE_ENV === "production"
    ? import.meta.env.VITE_BASE_URL
    : import.meta.env.VITE_BASE_URL_DEV;

const axiosInstance = Axios.create({
  baseURL,
});

// const setAuthToken = (token) => {
//   if (token) {
//     axiosInstance.defaults.headers.common["Authorization"] = `Bearer ${token}`;
//   } else {
//     delete axiosInstance.defaults.headers.common["Authorization"];
//   }
// };

export default axiosInstance;
