import React from 'react';

import TopMenu from './top-menu';
import Footer from './footer';
import "./layout-container.scss";

export function LayoutContainer({ children }) {
  return (
    <div className="o-panel-container u-full-height">
      <TopMenu className="c-nav--top u-centered"/>
      <main className="o-panel view-container o-panel--nav-top">
        {children}
      </main>
      <Footer className="c-nav--bottom"/>
    </div>
  );
}

export default LayoutContainer;
