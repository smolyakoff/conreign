import React from 'react';
import { Provider } from 'react-redux';
import { Router } from 'react-router';

import {createRoutes} from './routes';

export class Window extends React.Component {
    static propTypes = {
        history: React.PropTypes.object.isRequired,
        store: React.PropTypes.object.isRequired
    };
    render() {
        return (
            <Provider store={this.props.store}>
                <Router history={this.props.history}>
                    {createRoutes(this.props.store)}
                </Router>
            </Provider>
        );
    }
}

export default Window;