/* eslint-disable react/forbid-prop-types */
import React, { PropTypes } from 'react';
import { Provider } from 'react-redux';
import { Router, Route, IndexRoute } from 'react-router';
import { isFunction, flatMap, isObject } from 'lodash';
import Rx from 'rxjs';

import './../theme';
import {
  executeRouteActions,
  beginRouteTransaction,
  endRouteTransaction,
} from './../root';
import { login, AUTH_REDUCER_KEY } from './../auth';
import { RootLayout, NavigationMenuLayout } from './../layout';
import { ErrorPage, ERROR_PAGE_PATH } from './../errors';
import { HomePage } from './../home';
import { RoomPage } from './../room';


export default function RouterContainer({ store, history }) {
  function ensureIsAuthenticated() {
    const state = store.getState();
    if (state[AUTH_REDUCER_KEY].user) {
      return Rx.Observable.of(state[AUTH_REDUCER_KEY].user);
    }
    store.dispatch(executeRouteActions([login()]));
    return Rx.Observable.from(store)
      .first(currentState => isObject(currentState[AUTH_REDUCER_KEY].user));
  }

  function collectRouteActions(nextState) {
    const storeState = store.getState();
    const components = nextState.routes.map(route => route.component);
    const initializers = components
      .map(x => x.init)
      .filter(isFunction);
    const actions = flatMap(initializers, init => init(nextState, storeState))
      .filter(isObject);
    return actions;
  }

  function dispatchRouteActions(nextState) {
    const actions = collectRouteActions(nextState);
    if (actions.length === 0) {
      return;
    }
    store.dispatch(executeRouteActions(actions));
  }

  function onRouteChange(prevState, nextState) {
    dispatchRouteActions(nextState);
  }

  function onRouteEnter(nextState) {
    store.dispatch(beginRouteTransaction());
    ensureIsAuthenticated()
      .do(() => dispatchRouteActions(nextState))
      .finally(() => store.dispatch(endRouteTransaction()))
      .subscribe();
  }

  return (
    <Provider store={store}>
      <div className="u-full-height">
        <Router history={history}>
          <Route
            path="/"
            component={RootLayout}
            onChange={onRouteChange}
            onEnter={onRouteEnter}
          >
            <Route component={NavigationMenuLayout}>
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
