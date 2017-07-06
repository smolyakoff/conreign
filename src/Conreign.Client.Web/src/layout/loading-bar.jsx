import React from 'react';

import { Progress } from './../theme';

export default function LoadingBar() {
  return (
    <Progress value={50} rounded={false} />
  );
}
