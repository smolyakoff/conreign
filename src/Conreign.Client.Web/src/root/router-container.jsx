/* eslint-disable react/forbid-prop-types */
import React from 'react';
import PropTypes from 'prop-types';
import { Router, Route, IndexRoute } from 'react-router';
import { isFunction, flatMap, isObject } from 'lodash';

import './../theme';
import { executeRouteTransition } from './../framework';
import { login } from './../auth';
import { AppLayout, NavigationMenuLayout } from './../layout';
import { ErrorPage, ERROR_PAGE_PATH } from './../errors';
import { HomePage } from './../home';
import { RoomPage } from './../room';
import RulesPage from './../rules';

export default function RouterContainer({ history }, { store }) {
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

  function dispatchRouteTransaction(nextState, authRequired) {
    const actions = collectRouteActions(nextState);
    const stages = [];
    if (authRequired) {
      stages.push(login());
    }
    if (actions.length > 0) {
      stages.push(actions);
    }
    store.dispatch(executeRouteTransition(stages));
  }

  function onRouteChange(prevState, nextState) {
    dispatchRouteTransaction(nextState, false);
  }

  function onRouteEnter(nextState) {
    dispatchRouteTransaction(nextState, true);
  }

  return (
    <div className="u-full-height">
      <Router history={history}>
        <Route
          path="/"
          component={AppLayout}
        >
          <Route
            component={NavigationMenuLayout}
            onChange={onRouteChange}
            onEnter={onRouteEnter}
          >
            <IndexRoute component={HomePage} />
            <Route path="/$/rules" component={RulesPage} />
            <Route path="/:roomId" component={RoomPage} />
          </Route>
          <Route path={ERROR_PAGE_PATH} component={ErrorPage} />
        </Route>
      </Router>
    </div>
  );
}

RouterContainer.propTypes = {
  history: PropTypes.object.isRequired,

};

RouterContainer.contextTypes = {
  store: PropTypes.object.isRequired,
};
