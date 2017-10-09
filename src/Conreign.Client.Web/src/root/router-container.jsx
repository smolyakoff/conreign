import React, { PureComponent } from 'react';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router-dom';
import { renderRoutes } from 'react-router-config';
import { compose } from 'recompose';
import { connect } from 'react-redux';

import './../theme';
import { executeRouteTransition } from './../framework';
import { AppLayout, NavigationMenuLayout } from './../layout';
import { ErrorPage, ERROR_PAGE_PATH } from './../errors';
import { HomePage } from './../home';
import { RoomPage } from './../room';
import RulesPage from './../rules';

const routes = [{
  component: AppLayout,
  routes: [
    {
      path: ERROR_PAGE_PATH,
      exact: true,
      component: ErrorPage,
    },
    {
      path: '/',
      component: NavigationMenuLayout,
      routes: [
        {
          path: '/',
          exact: true,
          component: HomePage,
        },
        {
          path: '/$/rules',
          exact: true,
          component: RulesPage,
        },
        {
          path: '/:roomId',
          exact: true,
          component: RoomPage,
        },
      ],
    },
  ],
}];

class RouterContainer extends PureComponent {
  componentWillMount() {
    const { location, onRouteChange, state } = this.props;
    onRouteChange(routes, state, location);
  }
  componentWillReceiveProps({ location: nextLocation }) {
    const {
      location: previousLocation,
      onRouteChange,
      state,
    } = this.props;
    const locationChanged = nextLocation !== previousLocation;
    if (!locationChanged) {
      return;
    }
    onRouteChange(
      routes,
      state,
      nextLocation,
      previousLocation,
    );
  }
  render() {
    return (
      <div className="u-full-height">
        {renderRoutes(routes)}
      </div>
    );
  }
}

RouterContainer.propTypes = {
  /* eslint-disable react/forbid-prop-types */
  state: PropTypes.object.isRequired,
  location: PropTypes.object.isRequired,
  /* eslint-enable react/forbid-prop-types */
  onRouteChange: PropTypes.func.isRequired,
};

const enhance = compose(
  withRouter,
  connect(
    state => ({ state }),
    { onRouteChange: executeRouteTransition },
  ),
);

export default enhance(RouterContainer);
