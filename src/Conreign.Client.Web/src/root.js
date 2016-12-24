import React, { PropTypes } from 'react';
import { Provider } from 'react-redux';
import { Router, Route, IndexRoute } from 'react-router';
import { syncHistoryWithStore } from 'react-router-redux'

import { createApplicationStore } from './store';
import { LayoutContainer } from './layout';
import { HomePage } from './home';

const DevTools = process.env.NODE_ENV === 'development'
  ? require('./dev-tools').createDevMonitor()
  : null;

export function Root({ history, state = {} }) {
  const store = createApplicationStore({ state, DevTools });
  const reduxHistory = syncHistoryWithStore(history, store);
  return (
    <Provider store={store}>
      <div>
        <Router history={reduxHistory}>
          <Route path="/" component={LayoutContainer}>
            <IndexRoute component={HomePage}/>
          </Route>
        </Router>
        {DevTools ? <DevTools /> : null}
      </div>
    </Provider>
  );
}
Root.propTypes = {
  history: PropTypes.object,
  state: PropTypes.object,
};

export default Root;
