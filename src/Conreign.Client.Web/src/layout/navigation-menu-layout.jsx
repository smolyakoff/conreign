import React, { PropTypes } from 'react';

import { PanelContainer, Panel } from './../theme';
import NavigationMenu from './navigation-menu';
import LoadingBar from './loading-bar';
import './navigation-menu-layout.scss';

export default function NavigationMenuLayout({ children }) {
  return (
    <PanelContainer className="u-full-height">
      <NavigationMenu className="u-centered" />
      <LoadingBar />
      <Panel className="o-nav-view">
        { children }
      </Panel>
    </PanelContainer>
  );
}

NavigationMenuLayout.propTypes = {
  children: PropTypes.node,
};

NavigationMenuLayout.defaultProps = {
  children: null,
};
