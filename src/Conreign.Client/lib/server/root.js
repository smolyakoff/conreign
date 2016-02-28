import React from 'react';
import {Provider} from 'react-redux';
import {RouterContext} from 'react-router';

export class Root extends React.Component {
    static propTypes = {
        renderProps: React.PropTypes.object.isRequired,
        store: React.PropTypes.object.isRequired
    };
    render() {
        return (
            <Provider store={this.props.store}>
                <div id="app">
                    <RouterContext {...this.props.renderProps}/>
                </div>
            </Provider>
        );
    }
}

export default Root;