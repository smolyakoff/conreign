import React from 'react';

import { Spinner, H2 } from './../theme';

export default function PageLoadingIndicator() {
  const size = '10vmin';
  return (
    <div>
      <H2>Loading...</H2>
      <div className="u-center-block" style={{ height: size }}>
        <div className="u-center-block__content">
          <Spinner size={size} />
        </div>
      </div>
    </div>
  );
}
