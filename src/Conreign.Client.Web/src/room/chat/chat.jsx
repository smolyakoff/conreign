import React from 'react';
import gear from 'evil-icons/assets/icons/ei-gear.svg';
import comment from 'evil-icons/assets/icons/ei-comment.svg';

import {
  PanelContainer,
  Panel,
  IconButton,
  Deck,
  DeckItem,
  Orientation,
} from './../../theme';

function ChatControlPanel() {
  return (
    <Deck orientation={Orientation.Horizontal}>
      <DeckItem>
        <IconButton iconName={gear} />
      </DeckItem>
      <DeckItem stretch>
        <input type="text" />
      </DeckItem>
      <DeckItem>
        <IconButton iconName={comment}>Send</IconButton>
      </DeckItem>
    </Deck>
  );
}

export default function Chat() {
  return (
    <Deck>
      <DeckItem stretch>
        <PanelContainer className="u-full-height">
          <Panel>
            Text
          </Panel>
        </PanelContainer>
      </DeckItem>
      <DeckItem>
        <ChatControlPanel />
      </DeckItem>
    </Deck>
  );
}
