const React = require('react');
const Provider = require('react-redux').Provider;
const RouterContext = require('react-router').RouterContext;

class Root extends React.Component {
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

module.exports = Root;