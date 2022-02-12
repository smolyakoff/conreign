import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';
import { noop } from 'lodash';

import Button from './button';
import { decorate, withThemeColors } from './decorators';

const css = bem('c-alert');

function Alert({
  className,
  children,
  hasCloseButton,
  onCloseClick,
  ...others
}) {
  return (
    <div className={css.mix(className)()} {...others}>
      {hasCloseButton && <Button close onClick={onCloseClick}>×</Button>}
      {children}
    </div>
  );
}

Alert.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  hasCloseButton: PropTypes.bool,
  onCloseClick: PropTypes.func,
};

Alert.defaultProps = {
  className: null,
  children: null,
  hasCloseButton: false,
  onCloseClick: noop,
};

export default decorate(
  withThemeColors(css()),
)(Alert);
