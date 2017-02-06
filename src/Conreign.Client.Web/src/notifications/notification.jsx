import React, { PropTypes } from 'react';
import { noop } from 'lodash';

import { Alert } from './../theme';

export default function Notification({
  id,
  themeColor,
  message,
  onClose,
}) {
  const notification = {
    id,
    themeColor,
    message,
  };
  return (
    <Alert
      hasCloseButton
      themeColor={themeColor}
      onClick={e => onClose(notification, e)}
      onCloseClick={e => onClose(notification, e)}
    >
      {message}
    </Alert>
  );
}

Notification.propTypes = {
  id: PropTypes.string,
  themeColor: PropTypes.string,
  message: PropTypes.string,
  onClose: PropTypes.func,
};

Notification.defaultProps = {
  id: null,
  themeColor: null,
  message: null,
  onClose: noop,
};
