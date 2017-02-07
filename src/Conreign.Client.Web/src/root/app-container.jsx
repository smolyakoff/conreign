/* eslint-disable react/forbid-prop-types */
import React, { Component, PropTypes } from 'react';
import { Provider } from 'react-redux';

import RouterContainer from './router-container';
import { reportRenderingError } from './root';

export default class AppContainer extends Component {
  static propTypes = {
    history: PropTypes.object.isRequired,
    store: PropTypes.object.isRequired,
  };
  // eslint-disable-next-line
  unstable_handleError(e) {
    // TODO: This is not working
    // eslint-disable-next-line
    console.error(e);
    const { store } = this.props;
    store.dispatch(reportRenderingError(e));
  }
  render() {
    const { store, history } = this.props;
    return (
      <Provider store={store}>
        <RouterContainer
          history={history}
          store={store}
        />
      </Provider>
    );
  }
}
