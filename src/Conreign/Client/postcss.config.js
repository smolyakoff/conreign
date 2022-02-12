const { constants } = require('./tools');

module.exports = {
  plugins: [
    require('autoprefixer')({
      browsers: constants.SUPPORTED_BROWSERS,
    }),
  ]
};
