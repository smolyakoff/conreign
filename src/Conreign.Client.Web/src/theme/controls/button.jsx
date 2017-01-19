import React, { PropTypes } from 'react';
import block from 'bem-cn';

import {
  withThemeColors,
  withActiveState,
  withThemeSizes,
  decorate,
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

export default decorate([
  withActiveState(CSS_CLASS),
  withThemeColors(CSS_CLASS),
  withThemeSizes(),
])(Button);


Button.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  fullWidth: PropTypes.bool,
  ghost: PropTypes.bool,
  rounded: PropTypes.bool,
};

Button.defaultProps = {
  className: null,
  children: null,
  fullWidth: false,
  ghost: false,
  rounded: false,
};
