/* eslint-disable react/forbid-prop-types */
import React, { PropTypes } from 'react';
import { Provider } from 'react-redux';
import { Router, Route, IndexRoute } from 'react-router';
import { isFunction, flatMap, isObject } from 'lodash';
import Rx from 'rxjs';

import './../theme';
import { executeRouteActions, selectPendingRouteOperations } from './../root';
import { LayoutContainer } from './../layout';
import { HomePage } from './../home';
import { RoomPage } from './../room';


export default function Root({ store, history, DevTools }) {
  function onRouteChange(prevState, nextState, callback) {
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
          <Route path="/" component={LayoutContainer} onChange={onRouteChange}>
            <IndexRoute component={HomePage} />
            <Route path="/:roomId" component={RoomPage} />
          </Route>
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
