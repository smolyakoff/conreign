import React from 'react';
// eslint-disable-next-line import/no-extraneous-dependencies
import { AppContainer } from 'react-hot-loader';

import Root from './root';

export default function HotRoot(props) {
  return (
    <AppContainer>
      <Root {...props} />
    </AppContainer>
  );
}
