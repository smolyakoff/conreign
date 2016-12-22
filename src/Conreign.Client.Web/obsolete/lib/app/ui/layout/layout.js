import React from 'react';
import { Provider } from 'react-redux';
import { Router } from 'react-router';
import CSSModules from 'react-css-modules';

import styles from './layout.scss';
import {Header} from './header';
import {Footer} from './footer';

export class LayoutBase extends React.Component {
    render() {
        return (
            <div styleName="layout">
                <div styleName="layout-container">
                    <Header/>
                    {this.props.children}
                </div>
                <Footer/>
            </div>
        );
    }
}

export const Layout = CSSModules(LayoutBase, styles);

export default Layout;