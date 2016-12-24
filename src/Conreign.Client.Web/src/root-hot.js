import React, { Component } from 'react';
import { AppContainer } from 'react-hot-loader';

import Root from './root';

export class HotRoot extends Component {
  render() {
    return (
      <AppContainer>
        <Root {...this.props}/>
      </AppContainer>
    );
  }
}

export default HotRoot;
