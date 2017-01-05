import React from 'react';
// eslint-disable-next-line import/no-extraneous-dependencies
import { AppContainer } from 'react-hot-loader';

import RouterContainer from './router-container';

export default function HotRoot(props) {
  return (
    <AppContainer>
      <RouterContainer {...props} />
    </AppContainer>
  );
}
