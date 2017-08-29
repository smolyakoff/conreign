import React, { PropTypes } from 'react';
import { omit } from 'lodash';

import ErrorNotification from './error-notification';
import { H2 } from './../theme';

const VALIDATION_ERROR_SHAPE = PropTypes.shape({
  message: PropTypes.string.isRequired,
});

export default function ValidationErrorNotification({
  details,
  ...other
}) {
  const otherWithoutStack = omit(other, ['stack']);
  return (
    <ErrorNotification {...otherWithoutStack} isWarning>
      <H2 className="u-window-box--none">
        Something went wrong :(
      </H2>
      <p>Please check the following and retry once again!</p>
      <ul>
        {
          details.failures.map(error => (
            <li key={error.message}>{error.message}</li>
          ))
        }
      </ul>
    </ErrorNotification>
  );
}

ValidationErrorNotification.propTypes = {
  details: PropTypes.shape({
    failures: PropTypes.arrayOf(VALIDATION_ERROR_SHAPE).isRequired,
  }).isRequired,
};
