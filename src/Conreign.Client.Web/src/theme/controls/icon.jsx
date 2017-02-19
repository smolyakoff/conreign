import React, { PropTypes } from 'react';
import block from 'bem-cn';

import { decorate, withThemeSizes } from './decorators';

const css = block('o-svg-icon');

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
)(Icon);
