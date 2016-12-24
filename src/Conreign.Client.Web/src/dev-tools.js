import React from 'react';
import { createDevTools } from 'redux-devtools';

export function createDevMonitor() {
  const createDevTools = require('redux-devtools').createDevTools;
  const DockMonitor = require('redux-devtools-dock-monitor').default;
  const Inspector = require('redux-devtools-inspector').default;
  return createDevTools(
    <DockMonitor toggleVisibilityKey="ctrl-h"
                 changePositionKey="ctrl-q"
                 defaultPosition="bottom"
                 defaultIsVisible={false}>
      <Inspector />
    </DockMonitor>
  );
}

export default createDevMonitor;
