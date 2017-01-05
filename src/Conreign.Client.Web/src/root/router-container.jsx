/* eslint-disable react/forbid-prop-types */
import React, { PropTypes } from 'react';
import { Provider } from 'react-redux';
import { Router, Route, IndexRoute } from 'react-router';
import { isFunction, flatMap, isObject } from 'lodash';
import Rx from 'rxjs';

import './../theme';
import { executeRouteActions, selectPendingRouteOperations } from './../root';
import { RootLayout, NavigationMenuLayout } from './../layout';
import { ErrorPage, ERROR_PAGE_PATH } from './../errors';
import { HomePage } from './../home';
import { RoomPage } from './../room';


export default function RouterContainer({ store, history }) {
  function onRouteChange(prevState, nextState, replace, callback) {
    const storeState = store.getState();
    const components = nextState.routes.map(route => route.component);
    const initializers = components
      .map(x => x.init)
      .filter(isFunction);
    const actions = flatMap(initializers, init => init(nextState, storeState))
      .filter(isObject);
    store.dispatch(executeRouteActions(actions));
    Rx.Observable.from(store)
      .map(() => store.getState())
      .map(selectPendingRouteOperations)
      .first(ops => ops === 0)
      .delay(0)
      .subscribe(() => callback(), e => callback(e));
  }
  return (
    <Provider store={store}>
      <div className="u-full-height">
        <Router history={history}>
          <Route component={RootLayout} onChange={onRouteChange}>
            <Route path="/" component={NavigationMenuLayout}>
              <IndexRoute component={HomePage} />
              <Route path="/:roomId" component={RoomPage} />
            </Route>
            <Route path={ERROR_PAGE_PATH} component={ErrorPage} />
          </Route>
        </Router>
      </div>
    </Provider>
  );
}

RouterContainer.propTypes = {
  history: PropTypes.object.isRequired,
  store: PropTypes.object.isRequired,
};
