import React, { PropTypes } from 'react';
import { connect } from 'react-redux';

import { selectPendingActionCount } from './../state';
import { PanelContainer, Panel } from './../theme';
import NavigationMenu from './navigation-menu';
import LoadingBar from './loading-bar';
import './navigation-menu-layout.scss';

function NavigationMenuLayout({
  children,
  isLoading,
}) {
  return (
    <PanelContainer className="u-full-height">
      <NavigationMenu className="u-centered" />
      <LoadingBar isHidden={!isLoading} />
      <Panel className="o-nav-view">
        { children }
      </Panel>
    </PanelContainer>
  );
}

NavigationMenuLayout.propTypes = {
  children: PropTypes.node,
  isLoading: PropTypes.bool,
};

NavigationMenuLayout.defaultProps = {
  children: null,
  isLoading: false,
};

const enhance = connect(state => ({
  isLoading: selectPendingActionCount(state) > 0,
}));

export default enhance(NavigationMenuLayout);
