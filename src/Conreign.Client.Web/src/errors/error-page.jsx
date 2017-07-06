import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import block from 'bem-cn';

import { H1, P, Box, ThemeSize } from './../theme';
import { selectErrorPage } from './errors';
import './error-page.scss';

const css = block('c-error-box');

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
