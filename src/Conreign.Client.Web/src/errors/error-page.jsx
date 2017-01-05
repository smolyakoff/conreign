import React, { PropTypes } from 'react';
import { connect } from 'react-redux';

import { H1, P } from './../theme';
import { selectErrorPageProps } from './errors';

function ErrorPage({ error, statusCode, showStack }) {
  return (
    <div className="u-absolute-center">
      <div className="u-centered">
        <H1 className="u-color-error-dark">Error {statusCode}</H1>
        <P>{error.message}</P>
      </div>
      {
        showStack ? (
          <div className="u-letter-box--medium">
            <code className="c-code c-code--multiline">
              {error.stack}
            </code>
          </div>

        ) : null
      }
    </div>
  );
}

ErrorPage.propTypes = {
  error: PropTypes.shape({
    message: PropTypes.string.isRequired,
  }).isRequired,
  statusCode: PropTypes.number.isRequired,
  showStack: PropTypes.bool,
};

ErrorPage.defaultProps = {
  showStack: false,
};

export default connect(
  selectErrorPageProps,
)(ErrorPage);
