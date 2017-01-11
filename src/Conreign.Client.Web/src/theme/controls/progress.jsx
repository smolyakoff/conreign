import React, { PropTypes } from 'react';
import block from 'bem-cn';

import { decorate, withThemeSizes, withThemeColors } from './decorators';

const progress = block('c-progress');
const bar = progress('bar');

function ProgressBar({ value, className, children }) {
  const percent = `${value}%`;
  return (
    <div className={bar.mix(className)} style={{ width: percent }}>
      {children}
    </div>
  );
}

ProgressBar.propTypes = {
  value: PropTypes.number,
  className: PropTypes.string,
  children: PropTypes.node,
};

const DecoratedProgressBar = decorate(
  withThemeColors(bar()),
)(ProgressBar);

function Progress({ className, rounded, ...otherProps }) {
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
  className: PropTypes.string,
};

Progress.defaultProps = {
  rounded: false,
};

export default decorate(
  withThemeSizes(),
)(Progress);
