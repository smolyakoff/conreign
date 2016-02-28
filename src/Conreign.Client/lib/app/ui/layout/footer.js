'use strict';
import React from 'react';
import CSSModules from 'react-css-modules';

import styles from './footer.scss';
import logo from './logo.png';

export class FooterBase extends React.Component {
    render() {
        const date = new Date();
        return (
            <footer styleName="footer" role="contentinfo">
                <div styleName="footer-logo">
                    <img src={logo} alt="Logo image"/> Conreign
                </div>
                <div styleName="footer-legal">
                    {date.getFullYear()}, All Rights Reserved
                </div>
            </footer>
        );
    }
}

export const Footer = CSSModules(FooterBase, styles, {allowMultiple: true});

export default Footer;