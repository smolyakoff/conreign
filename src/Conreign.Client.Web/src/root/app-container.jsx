import React from 'react';
import PropTypes from 'prop-types';
import { Provider } from 'react-redux';
import { Router } from 'react-router-dom';
import { AppContainer as HotContainer } from 'react-hot-loader';

import RouterContainer from './router-container';

export default function AppContainer({ history, store }) {
  return (
    <HotContainer>
      <Provider store={store}>
        <Router history={history}>
          <RouterContainer />
        </Router>
      </Provider>
    </HotContainer>
  );
}

AppContainer.propTypes = {
  /* eslint-disable react/forbid-prop-types */
  history: PropTypes.object.isRequired,
  store: PropTypes.object.isRequired,
  /* eslint-enable react/forbid-prop-types */
};
