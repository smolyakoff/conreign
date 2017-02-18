import React from 'react';
import settingsIconSrc from 'evil-icons/assets/icons/ei-gear.svg';

import { PanelContainer, Panel, Grid, GridCell, Button } from './../../theme';

function ChatControlPanel() {
  return (
    <Grid>
      <GridCell fixedWidth>
        <Button>
          <img src={settingsIconSrc} alt="" />
        </Button>
      </GridCell>
      <GridCell>
        <input type="text" />
      </GridCell>
      <GridCell fixedWidth>
        <Button>Send</Button>
      </GridCell>
    </Grid>
  );
}

export default function Chat() {
  return (
    <div>
      <PanelContainer>
        <Panel>
          Text
        </Panel>
      </PanelContainer>
      <ChatControlPanel />
    </div>
  );
}
