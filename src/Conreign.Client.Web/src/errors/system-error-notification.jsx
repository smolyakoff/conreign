import React from 'react';
import PropTypes from 'prop-types';

import ErrorNotification from './error-notification';
import { H2 } from './../theme';

export default function SystemErrorNotification({
  name,
  message,
  ...other
}) {
  return (
    <ErrorNotification {...other}>
      <H2 className="u-window-box--none">
        {name}
      </H2>
      <p>
        {message}
      </p>
    </ErrorNotification>
  );
}

SystemErrorNotification.propTypes = {
  name: PropTypes.string.isRequired,
  message: PropTypes.string.isRequired,
};

// Display name is used in notification area to select corresponding component
SystemErrorNotification.displayName = 'SystemErrorNotification';
