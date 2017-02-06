import React, { PropTypes } from 'react';
import { noop } from 'lodash';

import { Alert, ThemeColor, H2, Code } from './../theme';

export default function ErrorNotification({
  id,
  name,
  message,
  stack,
  isWarning,
  onClose,
}) {
  const errorNotification = {
    id,
    name,
    message,
    stack,
    isWarning,
  };
  const themeColor = isWarning
    ? ThemeColor.Warning
    : ThemeColor.Error;
  return (
    <Alert
      className="u-window-box--medium"
      hasCloseButton
      themeColor={themeColor}
      onClick={e => onClose(errorNotification, e)}
      onCloseClick={e => onClose(errorNotification, e)}
    >
      <H2 className="u-window-box--none">
        {name}
      </H2>
      <p>
        {message}
      </p>
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
  id: PropTypes.string.isRequired,
  name: PropTypes.string,
  message: PropTypes.string.isRequired,
  stack: PropTypes.string,
  isWarning: PropTypes.bool,
  onClose: PropTypes.func,
};

ErrorNotification.defaultProps = {
  name: 'Unexpected Error',
  stack: null,
  isWarning: false,
  onClose: noop,
};
