import React, { PropTypes } from 'react';
import { connect } from 'react-redux';

import { PanelContainer, Panel } from './../theme';
import PageLoadingIndicator from './page-loading-indicator';
import Footer from './footer';
import { selectLayoutProps } from './layout';
import './footer-layout.scss';

function FooterLayout({ view, children, isPageLoading }) {
  const content = isPageLoading
    ? (
      <div className="u-absolute-center">
        <PageLoadingIndicator />
      </div>
    )
    : view;
  return (
    <PanelContainer className="u-full-height">
      {children}
      <Panel className="o-root-view">
        {content}
      </Panel>
      <Footer position="bottom" />
    </PanelContainer>
  );
}

FooterLayout.propTypes = {
  children: PropTypes.node,
  view: PropTypes.node,
  isPageLoading: PropTypes.bool,
};

FooterLayout.defaultProps = {
  children: null,
  view: null,
  isPageLoading: false,
};

export default connect(
  (state, ownProps) => ({
    ...ownProps,
    ...selectLayoutProps(state),
  }),
)(FooterLayout);
