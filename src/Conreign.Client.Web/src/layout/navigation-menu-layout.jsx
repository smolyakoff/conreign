import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { renderRoutes } from 'react-router-config';

import { selectPendingActionCount } from './../state';
import { ROUTE_SHAPE } from './../framework';
import { PanelContainer, Panel } from './../theme';
import { login } from './../auth';
import NavigationMenu from './navigation-menu';
import LoadingBar from './loading-bar';
import './navigation-menu-layout.scss';

function NavigationMenuLayout({
  route,
  isLoading,
}) {
  return (
    <PanelContainer className="u-full-height">
      <NavigationMenu className="u-centered" />
      <LoadingBar isHidden={!isLoading} />
      <Panel className="o-nav-view">
        { renderRoutes(route.routes) }
      </Panel>
    </PanelContainer>
  );
}

NavigationMenuLayout.init = login;

NavigationMenuLayout.propTypes = {
  route: ROUTE_SHAPE.isRequired,
  isLoading: PropTypes.bool,
};

NavigationMenuLayout.defaultProps = {
  isLoading: false,
};

const enhance = connect(state => ({
  isLoading: selectPendingActionCount(state) > 0,
}));

export default enhance(NavigationMenuLayout);
