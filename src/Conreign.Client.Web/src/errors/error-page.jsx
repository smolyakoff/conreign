import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { parseInt, flow } from 'lodash';
import bem from 'bem-cn';

import { selectErrors } from './../state';
import { H1, P, Box, ThemeSize } from './../theme';
import { selectRoutingError } from './errors';
import './error-page.scss';

const css = bem('c-error-box');

function ErrorPage({ error, statusCode, showStack }) {
  return (
    <div className={css.mix('u-absolute-center')()}>
      <div className="u-centered">
        <H1 className="u-color-error-dark">Error {statusCode}</H1>
        <P className={css('message')()}>{error.message}</P>
      </div>
      {
        showStack ? (
          <Box themeSize={ThemeSize.Medium}>
            <code
              className="c-code c-code--multiline"
              style={{ fontSize: '0.8em' }}
            >
              {error.stack}
            </code>
          </Box>
        ) : null
      }
    </div>
  );
}

const DUMMY_ERROR = new Error('Nothing to do here 😉');

function selectErrorPage(state, { params }) {
  const routingError = flow(selectErrors, selectRoutingError)(state);
  const statusCode = parseInt(params.statusCode);
  return {
    statusCode,
    showStack: COMPILATION_MODE === 'debug',
    error: routingError || DUMMY_ERROR,
  };
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
  selectErrorPage,
)(ErrorPage);
