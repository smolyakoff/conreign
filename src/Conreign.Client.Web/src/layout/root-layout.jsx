import React, { PropTypes } from 'react';
import { connect } from 'react-redux';

import { PanelContainer, Panel } from './../theme';
import PageLoadingIndicator from './page-loading-indicator';
import Footer from './footer';
import { selectLayoutProps } from './layout';
import './root-layout.scss';

function RootLayout({ children, isPageLoading }) {
  const content = isPageLoading
    ? (
      <div className="u-absolute-center">
        <PageLoadingIndicator />
      </div>
    )
    : children;
  return (
    <PanelContainer className="u-full-height">
      <Panel className="o-root-view">
        {content}
      </Panel>
      <Footer position="bottom" />
    </PanelContainer>
  );
}

RootLayout.propTypes = {
  children: PropTypes.node,
  isPageLoading: PropTypes.bool,
};

export default connect(
  (state, ownProps) => ({
    ...ownProps,
    ...selectLayoutProps(state),
  }),
)(RootLayout);
