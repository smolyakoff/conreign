'use strict';
import React from 'react';
import CSSModules from 'react-css-modules';

import styles from './header.scss';
import logo from './logo.png';

export class HeaderBase extends React.Component {
    render() {
        return (
            <header styleName="navigation-centered" role="banner">
                <div styleName="navigation-centered-wrapper">
                    <a href="javascript:void(0)" styleName="mobile-logo">
                        <img src={logo} alt="Logo"/>
                    </a>
                    <a href="javascript:void(0)" id="js-navigation-centered-mobile-menu" styleName="navigation-centered-mobile-menu">Menu</a>
                    <nav role="navigation">
                        <ul id="js-navigation-centered-menu" styleName="navigation-centered-menu show">
                            <li styleName="nav-link"><a>Stats</a></li>
                            <li styleName="nav-link logo">
                                <a href="javascript:void(0)" styleName="logo">
                                    <img src={logo} alt="Logo"/>
                                </a>
                            </li>
                            <li styleName="nav-link"><a>Rules</a></li>
                        </ul>
                    </nav>
                </div>
            </header>
        );
    }
}

export const Header = CSSModules(HeaderBase, styles, {allowMultiple: true});

export default Header;