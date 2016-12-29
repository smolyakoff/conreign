/* eslint-disable react/forbid-prop-types */

import React, { PropTypes } from 'react';
import { Provider } from 'react-redux';
import { Router } from 'react-router';
import './../theme';

import routes from './routes';

export default function Root({ store, history, DevTools }) {
  return (
    <Provider store={store}>
      <div className="u-full-height">
        <Router history={history}>
          {routes}
        </Router>
        {DevTools ? <DevTools /> : null}
      </div>
    </Provider>
  );
}

Root.propTypes = {
  history: PropTypes.object.isRequired,
  store: PropTypes.object.isRequired,
  DevTools: PropTypes.func,
};
