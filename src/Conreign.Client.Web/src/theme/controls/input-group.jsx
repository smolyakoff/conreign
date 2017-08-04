import React, { PropTypes } from 'react';
import bem from 'bem-cn';

import { decorate, withThemeSizes } from './decorators';

const bemClass = bem('c-input-group');

function InputGroup({
  className,
  children,
  rounded,
  stacked,
  ...props
}) {
  const modifiers = {
    rounded,
    stacked,
  };
  return (
    <span
      className={bemClass(modifiers).mix(className)()}
      {...props}
    >
      {children}
    </span>
  );
}

InputGroup.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  rounded: PropTypes.bool,
  stacked: PropTypes.bool,
};

InputGroup.defaultProps = {
  className: null,
  children: null,
  rounded: false,
  stacked: false,
};

export default decorate(
  withThemeSizes(),
)(InputGroup);
