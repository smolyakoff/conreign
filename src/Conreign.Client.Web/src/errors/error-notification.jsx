import React, { PropTypes } from 'react';
import { noop } from 'lodash';

import { Alert, ThemeColor, Code } from './../theme';

export default function ErrorNotification({
  children,
  stack,
  isWarning,
  onClose,
}) {
  const themeColor = isWarning
    ? ThemeColor.Warning
    : ThemeColor.Error;
  return (
    <Alert
      className="u-window-box--medium"
      hasCloseButton
      themeColor={themeColor}
      onClick={onClose}
      onCloseClick={onClose}
    >
      {children}
      {
        stack && (
          <Code multiLine>
            {stack}
          </Code>
        )
      }
    </Alert>
  );
}

ErrorNotification.propTypes = {
  children: PropTypes.node.isRequired,
  stack: PropTypes.string,
  isWarning: PropTypes.bool,
  onClose: PropTypes.func,
};

ErrorNotification.defaultProps = {
  stack: null,
  isWarning: false,
  onClose: noop,
};
