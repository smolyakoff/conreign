import React from 'react';
import gear from 'evil-icons/assets/icons/ei-gear.svg';

import { PanelContainer, Panel, Grid, GridCell, Button, Icon, ThemeSize } from './../../theme';

function ChatControlPanel() {
  return (
    <Grid>
      <GridCell fixedWidth>
        <Button>
          <Icon name={gear} themeSize={ThemeSize.Medium} />
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
