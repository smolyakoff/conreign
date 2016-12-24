import "./theme/theme.scss";
import React from 'react';
import ReactDOM from 'react-dom';
import { browserHistory } from 'react-router';

const state = {};

if (COMPILATION_MODE === 'debug') {
  window.React = React;
}

function render(RootComponent) {
  ReactDOM.render(
    <RootComponent history={browserHistory} state={state}/>,
    document.getElementById('root'),
  )
}

const App = TASK === 'run' ? require('./root-hot').default : require('./root').default;
render(App);

if (TASK === 'run') {
  if (module.hot) {
    module.hot.accept('./root-hot', () => {
      const UpdatedApp = require('./root-hot').default;
      render(UpdatedApp)
    });
  }
}
