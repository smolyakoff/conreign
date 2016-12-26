import React, { PropTypes } from 'react';

import { PanelContainer, Panel } from './../theme';
import NavigationMenu from './navigation-menu';
import Footer from './footer';
import './layout-container.scss';

export function LayoutContainer({ children }) {
  return (
    <PanelContainer className="u-full-height">
      <NavigationMenu className="u-centered" />
      <Panel className="view-container">
        {children}
      </Panel>
      <Footer position="bottom" />
    </PanelContainer>
  );
}

LayoutContainer.propTypes = {
  children: PropTypes.node,
};

export default LayoutContainer;
