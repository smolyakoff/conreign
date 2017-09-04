import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';

import { decorate, withThemeSizes, withThemeColors } from './decorators';

const progress = bem('c-progress');
const bar = progress('bar');

function ProgressBar({
  value,
  animated,
  striped,
  className,
  children,
}) {
  const modifiers = { animated, striped };
  const percentValue = `${value}%`;
  return (
    <div
      className={bar(modifiers).mix(className)()}
      style={{ width: percentValue }}
    >
      {children}
    </div>
  );
}

ProgressBar.propTypes = {
  value: PropTypes.number,
  animated: PropTypes.bool,
  striped: PropTypes.bool,
  className: PropTypes.string,
  children: PropTypes.node,
};

ProgressBar.defaultProps = {
  value: 0,
  animated: false,
  striped: false,
  className: null,
  children: null,
};

const DecoratedProgressBar = decorate(
  withThemeColors(bar()),
)(ProgressBar);

function Progress({
  className,
  rounded,
  ...otherProps
}) {
  const modifiers = {
    rounded,
  };
  return (
    <div className={progress(modifiers).mix(className)()}>
      <DecoratedProgressBar {...otherProps} />
    </div>
  );
}

Progress.propTypes = {
  value: PropTypes.number,
  rounded: PropTypes.bool,
  animated: PropTypes.bool,
  striped: PropTypes.bool,
  className: PropTypes.string,
};

Progress.defaultProps = {
  className: null,
  value: 0,
  rounded: false,
  animated: false,
  striped: false,
};

export default decorate(
  withThemeSizes(),
)(Progress);
