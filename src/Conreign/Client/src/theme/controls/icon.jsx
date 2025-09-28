import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';

import { decorate, withThemeSizes, withThemeColors } from './decorators';

const css = bem('o-svg-icon');

function Icon({ className, name, viewBox, ...others }) {
  const finalViewBox = viewBox || '0 0 64 64';
  return (
    <svg className={css.mix(className)()} viewBox={finalViewBox} {...others}>
      <use xlinkHref={`#${name}`} />
    </svg>
  );
}

Icon.propTypes = {
  name: PropTypes.string.isRequired,
  viewBox: PropTypes.string,
  className: PropTypes.string,
};

Icon.defaultProps = {
  viewBox: null,
  className: null,
};

export default decorate(
  withThemeSizes(css()),
  withThemeColors(css()),
)(Icon);
