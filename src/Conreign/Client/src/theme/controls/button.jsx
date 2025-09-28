import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';

import {
  withThemeColors,
  withActiveState,
  withThemeSizes,
  decorate,
} from './decorators';

const CSS_CLASS = 'c-button';
const css = bem(CSS_CLASS);

function Button({
  className,
  children,
  component,
  fullWidth,
  ghost,
  rounded,
  close,
  ...others
}) {
  const modifiers = {
    block: fullWidth,
    ghost,
    rounded,
    close,
  };
  const Tag = component;
  return (
    <Tag className={css(modifiers).mix(className)()} {...others}>
      {children}
    </Tag>
  );
}

export default decorate([
  withActiveState(CSS_CLASS),
  withThemeColors(
    CSS_CLASS,
    {
      getClassName: props => props.ghost
        ? `${CSS_CLASS}--ghost-${props.themeColor}`
        : `${CSS_CLASS}--${props.themeColor}`,
    },
  ),
  withThemeSizes(),
])(Button);


Button.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  fullWidth: PropTypes.bool,
  ghost: PropTypes.bool,
  rounded: PropTypes.bool,
  close: PropTypes.bool,
  component: PropTypes.oneOfType([
    PropTypes.func,
    PropTypes.string,
  ]),
};

Button.defaultProps = {
  className: null,
  children: null,
  fullWidth: false,
  ghost: false,
  rounded: false,
  close: false,
  component: 'button',
};
