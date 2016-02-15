import React from 'react';
import { Provider } from 'react-redux';
import { Router } from 'react-router';

export class Layout extends React.Component {
    render() {
        return (
            <div>
                <header>Header</header>
                {this.props.children}
            </div>
        );
    }
}

export default Layout;