import React, { Component, PropTypes } from 'react';
import { Provider } from 'react-redux';
import { Router } from 'react-router';

import rootRoute from './routes';

export class Root extends Component {
  render() {
    const { store, history, DevTools } = this.props;
    return (
      <Provider store={store}>
        <div className="u-full-height">
          <Router history={history}>
            {rootRoute}
          </Router>
          {DevTools ? <DevTools /> : null}
        </div>
      </Provider>
    );
  }
}

Root.propTypes = {
  history: PropTypes.object,
  store: PropTypes.object,
};

export default Root;
