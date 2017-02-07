import React from 'react';
// eslint-disable-next-line import/no-extraneous-dependencies
import { AppContainer as HotLoader } from 'react-hot-loader';

import AppContainer from './app-container';

export default function HotAppContainer(props) {
  return (
    <HotLoader>
      <AppContainer {...props} />
    </HotLoader>
  );
}
