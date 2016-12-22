import React from 'react';
import { Provider } from 'react-redux';
import { Router } from 'react-router';

export class Root extends React.Component {
    static propTypes = {
        history: React.PropTypes.object.isRequired,
        store: React.PropTypes.object.isRequired,
        routes: React.PropTypes.element.isRequired,
        devTools: React.PropTypes.element
    };
    render() {
        return (
            <Provider store={this.props.store}>
                <div id="app">
                    <Router history={this.props.history}>
                        {this.props.routes}
                    </Router>
                    {this.props.devTools}
                </div>
            </Provider>
        );
    }
}

export default Root;