import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';

import { decorate, withThemeSizes, withThemeColors } from './decorators';

const css = bem('o-svg-icon');

function Icon({ className, name, ...others }) {
  return (
    <svg className={css.mix(className)()} viewBox="0 0 64 64" {...others}>
      <use xlinkHref={name} />
    </svg>
  );
}

Icon.propTypes = {
  name: PropTypes.string.isRequired,
  className: PropTypes.string,
};

Icon.defaultProps = {
  className: null,
};

export default decorate(
  withThemeSizes(css()),
  withThemeColors(css()),
)(Icon);
