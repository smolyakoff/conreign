import React, { PropTypes } from 'react';
import block from 'bem-cn';

import {
  withThemeColors,
  withThemeSizes,
  decorate,
} from './decorators';

const INPUT_CLASS = 'c-field';

function InputBase({
  tagName,
  className,
  ...others
}) {
  const Tag = tagName;
  const css = block(INPUT_CLASS);
  return (
    <Tag className={css.mix(className)} {...others} />
  );
}

InputBase.displayName = 'Input';
InputBase.propTypes = {
  tagName: PropTypes.oneOf(['input', 'textarea', 'select']),
  className: PropTypes.string,
};
InputBase.defaultProps = {
  className: null,
  tagName: 'input',
  type: 'text',
};

export const Input = decorate([
  withThemeSizes(),
  withThemeColors(INPUT_CLASS),
])(InputBase);

const INPUT_FIELD_CLASS = 'o-field';

function InputContainerBase({
  className,
  children,
  iconLeft,
  iconRight,
}) {
  const css = block(INPUT_FIELD_CLASS);
  const modifiers = {
    'icon-left': iconLeft,
    'icon-right': iconRight,
  };
  return (
    <div className={css(modifiers).mix(className)}>
      {children}
    </div>
  );
}

InputContainerBase.displayName = 'InputContainer';
InputContainerBase.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  iconLeft: PropTypes.bool,
  iconRight: PropTypes.bool,
};
InputContainerBase.defaultProps = {
  className: null,
  children: false,
  iconLeft: false,
  iconRight: false,
};

export const InputContainer = decorate(withThemeSizes())(InputContainerBase);
