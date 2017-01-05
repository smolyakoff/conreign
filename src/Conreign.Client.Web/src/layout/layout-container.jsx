import React, { PropTypes } from 'react';
import { connect } from 'react-redux';

import { PanelContainer, Panel } from './../theme';
import NavigationMenu from './navigation-menu';
import PageLoadingIndicator from './page-loading-indicator';
import Footer from './footer';
import { selectLayoutProps } from './layout';
import './layout-container.scss';

function LayoutContainer({ children, isPageLoading }) {
  const content = isPageLoading
    ? (
      <div className="u-absolute-center">
        <PageLoadingIndicator />
      </div>
    )
    : children;
  return (
    <PanelContainer className="u-full-height">
      <NavigationMenu className="u-centered" />
      <Panel className="view-container">
        {content}
      </Panel>
      <Footer position="bottom" />
    </PanelContainer>
  );
}

LayoutContainer.propTypes = {
  children: PropTypes.node,
  isPageLoading: PropTypes.bool,
};

export default connect(
  (state, ownProps) => ({
    ...ownProps,
    ...selectLayoutProps(state),
  }),
)(LayoutContainer);
