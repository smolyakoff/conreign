import React from 'react';
/* eslint-disable import/no-extraneous-dependencies */
import { createDevTools } from 'redux-devtools';
import DockMonitor from 'redux-devtools-dock-monitor';
import Inspector from 'redux-devtools-inspector';

export function createDevMonitor() {
  return createDevTools(
    <DockMonitor
      toggleVisibilityKey="ctrl-h"
      changePositionKey="ctrl-q"
      defaultPosition="bottom"
      defaultIsVisible={false}
    >
      <Inspector />
    </DockMonitor>,
  );
}

export default createDevMonitor;
