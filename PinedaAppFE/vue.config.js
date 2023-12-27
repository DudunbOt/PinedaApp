const path = require("path");
require("dotenv").config({ path: path.resolve(__dirname, ".env.development") });

module.exports = {
  // other configuration options...
  configureWebpack: {
    devtool: "source-map",
  },
};
