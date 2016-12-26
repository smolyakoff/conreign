import React, { PropTypes } from 'react';
import block from 'bem-cn';
import { flow } from 'lodash';

import {
  supportsThemeColors,
  supportsThemeSizes,
  supportsActiveState,
} from './decorators';

const CSS_CLASS = 'c-button';
const css = block(CSS_CLASS);

function Button({
  className,
  children,
  fullWidth,
  ghost,
  rounded,
  ...others
}) {
  const modifiers = {
    block: fullWidth,
    ghost,
    rounded,
  };
  return (
    <button className={css(modifiers).mix(className)()} {...others}>
      {children}
    </button>
  );
}

export default flow([
  supportsThemeSizes(),
  supportsActiveState(CSS_CLASS),
  supportsThemeColors(CSS_CLASS),
])(Button);

Button.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  fullWidth: PropTypes.bool,
  ghost: PropTypes.bool,
  rounded: PropTypes.bool,
};

Button.defaultProps = {
  fullWidth: false,
  ghost: false,
  rounded: false,
};
