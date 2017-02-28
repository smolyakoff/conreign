import React, { PropTypes } from 'react';

import './chat.scss';
import ChatControlPanel from './chat-control-panel';
import ChatMessageArea from './chat-message-area';
import PlayerList from './player-list';
import { PLAYER_SHAPE } from './../schemas';

import {
  Deck,
  DeckItem,
  DrawerContainer,
  Position,
  Box,
  BoxType,
  ThemeSize,
  Grid,
  GridCell,
} from './../../theme';

export default function Chat({
  players,
  settingsChildren,
  settingsOpen,
  onSettingsToggle,
}) {
  const showSettings = !!settingsChildren;

  return (
    <Deck themeSpacing={ThemeSize.Small}>
      <DeckItem stretch>
        <DrawerContainer
          className="u-full-height"
          drawerPosition={Position.Bottom}
          drawerVisible={settingsOpen}
          drawerChildren={
            <Box themeSize={ThemeSize.Small}>
              {settingsChildren}
            </Box>
          }
          onOverlayClick={e => onSettingsToggle(false, e)}
        >
          <Grid className="u-full-height">
            <GridCell>
              <ChatMessageArea />
            </GridCell>
            <GridCell className="chat-sidebar" fixedWidth>
              <PlayerList players={players} />
            </GridCell>
          </Grid>
        </DrawerContainer>
      </DeckItem>
      <DeckItem>
        <Box type={BoxType.Letter} themeSize={ThemeSize.Small}>
          <ChatControlPanel
            showSettings={showSettings}
            onSettingsClick={e => onSettingsToggle(!settingsOpen, e)}
          />
        </Box>
      </DeckItem>
    </Deck>
  );
}

Chat.propTypes = {
  players: PropTypes.arrayOf(PLAYER_SHAPE).isRequired,
  settingsChildren: PropTypes.node,
  settingsOpen: PropTypes.bool,
  onSettingsToggle: PropTypes.func.isRequired,
};

Chat.defaultProps = {
  settingsChildren: null,
  settingsOpen: false,
};
