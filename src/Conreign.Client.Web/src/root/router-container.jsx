/* eslint-disable react/forbid-prop-types */
import React, { PropTypes } from 'react';
import { Router, Route, IndexRoute } from 'react-router';
import { isFunction, flatMap, isObject } from 'lodash';

import './../theme';
import {
  beginRouteTransaction,
} from './../root';
import { login } from './../auth';
import { FooterLayout, NavigationMenuLayout } from './../layout';
import { ErrorPage, ERROR_PAGE_PATH, ErrorNotification } from './../errors';
import { HomePage } from './../home';
import { RoomPage } from './../room';
import { NotificationArea } from './../notifications';

const NOTIFICATION_RENDERERS = {
  ErrorNotification,
};

function AppLayout({ children }) {
  return (
    <FooterLayout view={children}>
      <NotificationArea renderers={NOTIFICATION_RENDERERS} />
    </FooterLayout>
  );
}

AppLayout.propTypes = {
  children: PropTypes.node,
};

AppLayout.defaultProps = {
  children: null,
};

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
    stages.push(actions);
    store.dispatch(beginRouteTransaction(stages));
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
