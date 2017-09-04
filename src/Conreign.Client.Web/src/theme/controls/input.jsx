import React from 'react';
import PropTypes from 'prop-types';
import { values, isString } from 'lodash';
import bem from 'bem-cn';

import {
  withThemeColors,
  withThemeSizes,
  decorate,
} from './decorators';

const FIELD_CLASS = 'c-field';
const fieldBlock = bem(FIELD_CLASS);

export const ValidationState = {
  Success: 'success',
  Error: 'error',
};

function InputBase({
  tagName,
  className,
  validationState,
  ...others
}) {
  const Tag = tagName;
  const modifiers = {
    [validationState]: isString(validationState),
  };
  return (
    <Tag
      className={fieldBlock(modifiers).mix(className)()}
      {...others}
    />
  );
}

InputBase.displayName = 'Input';
InputBase.propTypes = {
  tagName: PropTypes.oneOf(['input', 'textarea', 'select']),
  className: PropTypes.string,
  validationState: PropTypes.oneOf(values(ValidationState)),
};
InputBase.defaultProps = {
  className: null,
  tagName: 'input',
  type: 'text',
  validationState: null,
};

export const Input = decorate([
  withThemeSizes(),
  withThemeColors(FIELD_CLASS),
])(InputBase);

const INPUT_FIELD_CLASS = 'o-field';

function InputContainerBase({
  className,
  children,
  iconLeft,
  iconRight,
}) {
  const css = bem(INPUT_FIELD_CLASS);
  const modifiers = {
    'icon-left': iconLeft,
    'icon-right': iconRight,
  };
  return (
    <div className={css(modifiers).mix(className)()}>
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

const hint = bem('c-hint');

function HintBase({
  className,
  children,
  isStatic,
  validationState,
}) {
  const modifiers = {
    static: isStatic,
    [validationState]: isString(validationState),
  };
  return (
    <div className={hint(modifiers).mix(className)()}>
      {children}
    </div>
  );
}

HintBase.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  isStatic: PropTypes.bool,
  validationState: PropTypes.oneOf(values(ValidationState)),
};

HintBase.defaultProps = {
  className: null,
  children: null,
  isStatic: false,
  validationState: null,
};

export const Hint = decorate(
  withThemeColors(hint()),
)(HintBase);
