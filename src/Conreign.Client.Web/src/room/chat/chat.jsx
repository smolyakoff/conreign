import React, { PropTypes } from 'react';
import { keyBy } from 'lodash';

import './chat.scss';
import ChatControlPanel from './chat-control-panel';
import ChatMessageArea from './chat-message-area';
import PlayerList from './player-list';
import { PLAYER_SHAPE, GAME_EVENT_SHAPE } from './../schemas';

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
  events,
  renderers,
  settingsChildren,
  settingsOpen,
  onSettingsToggle,
  onMessageSend,
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
          <Grid className="u-full-height u-small">
            <GridCell>
              <ChatMessageArea
                events={events}
                players={keyBy(players, x => x.userId)}
                renderers={renderers}
              />
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
            onMessageSend={onMessageSend}
          />
        </Box>
      </DeckItem>
    </Deck>
  );
}

Chat.propTypes = {
  players: PropTypes.arrayOf(PLAYER_SHAPE),
  events: PropTypes.arrayOf(GAME_EVENT_SHAPE),
  renderers: PropTypes.objectOf(PropTypes.func),
  settingsChildren: PropTypes.node,
  settingsOpen: PropTypes.bool,
  onSettingsToggle: PropTypes.func.isRequired,
  onMessageSend: PropTypes.func.isRequired,
};

Chat.defaultProps = {
  events: [],
  players: [],
  settingsChildren: null,
  settingsOpen: false,
  renderers: {},
};