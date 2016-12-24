import React from 'react';
import { AppContainer } from 'react-hot-loader';

import Root from './root';

export function HotRoot(props) {
  return (
    <AppContainer>
      <Root {...props}/>
    </AppContainer>
  );
}

export default HotRoot;
