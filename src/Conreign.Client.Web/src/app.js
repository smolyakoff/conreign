import "./theme/theme.scss";
import React from 'react';
import ReactDOM from 'react-dom';
import { browserHistory } from 'react-router';
import { syncHistoryWithStore } from 'react-router-redux'

import { createApplicationStore } from './store';

if (COMPILATION_MODE === 'debug') {
  window.React = React;
}

const state = {};
const DevTools = process.env.NODE_ENV === 'development'
  ? require('./dev-tools').createDevMonitor()
  : null;
const store = createApplicationStore({ state, DevTools });
const history = syncHistoryWithStore(browserHistory, store);

function render(RootComponent) {
  ReactDOM.render(
    <RootComponent
      history={history}
      store={store}
      DevTools={DevTools}
    />,
    document.getElementById('root'),
  )
}

const App = TASK === 'run' ? require('./root-hot').default : require('./root').default;
render(App);

if (TASK === 'run') {
  if (module.hot) {
    // HACK: react-router v3 is incompatible with hot reload, so need to hide some odd messages
    // https://github.com/gaearon/react-hot-loader/issues/298
    const _ = require('lodash');
    const originalConsoleError = console.error.bind(console);
    console.error = (message) => {
      if (_.isString(message) && message.indexOf('You cannot change <Router routes>;') >= 0) {
        return;
      }
      originalConsoleError(message);
    };

    module.hot.accept('./root-hot', () => {
      const UpdatedApp = require('./root-hot').default;
      render(UpdatedApp)
    });
  }
}
