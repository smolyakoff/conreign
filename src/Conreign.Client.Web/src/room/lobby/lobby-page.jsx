import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { values } from 'lodash';
import Measure from 'react-measure';
import { compose, withHandlers, withPropsOnChange } from 'recompose';

import {
  PLAYER_SHAPE,
  PLANET_SHAPE,
  GAME_EVENT_SHAPE,
} from '../room-schemas';
import {
  Grid,
  GridCell,
  ThemeSize,
  GridMode,
  Widget,
  Deck,
  DeckItem,
} from './../../theme';
import { Map, PlanetCell, MAP_SELECTION_SHAPE } from './../map';
import {
  submitGameSettings,
  changeGameSettings,
  submitPlayerSettings,
  changePlayerSettings,
  setPlayerSettingsVisibility,
  startGame,
} from './lobby';
import { GAME_SETTINGS_SHAPE, PLAYER_SETTINGS_SHAPE } from './lobby-schemas';
import PlanetCard from './../planet-card';
import Chat from './../chat';
import lobbyEventRenderers from './lobby-event-renderers';
import GameSettingsForm from './game-settings-form';
import PlayerSettingsForm from './player-settings-form';

const WIDGET_HEADER_HEIGHT = 35;

function calculateMapViewDimensions(viewDimensions) {
  const {
    width: fullVw,
    height: fullVh,
  } = viewDimensions;
  const vw = fullVw / 2;
  const vh = fullVh - WIDGET_HEADER_HEIGHT;
  return { width: vw, height: vh };
}

function LobbyCell({
  cellIndex,
  planets,
  players,
}) {
  const planet = planets[cellIndex];
  if (!planet) {
    return null;
  }
  const owner = players[planet.ownerId];
  return <PlanetCell planet={planet} owner={owner} />;
}
LobbyCell.propTypes = {
  cellIndex: PropTypes.number.isRequired,
  planets: PropTypes.objectOf(PLANET_SHAPE).isRequired,
  players: PropTypes.objectOf(PLAYER_SHAPE).isRequired,
};

function LobbyPage({
  playerSettings,
  gameSettings,
  map,
  players,
  events,
  cellRenderer,
  eventRenderers,
  leaderUserId,
  mapSelection,
  currentUser,
  playerSettingsOpen,
  onMessageSend,
  onPlayerSettingsSetVisibility,
  onMapSelectionChange,
  onGameSettingsSubmit,
  onGameSettingsChange,
  onPlayerSettingsChange,
  onPlayerSettingsSubmit,
  onGameStart,
}) {
  const selectedPlanet = map.planets[mapSelection.start];
  const isLeader = leaderUserId === currentUser.id;
  playerSettings = playerSettings || players[currentUser.id];

  const usedNicknames = values(players)
    .filter(p => p.userId !== currentUser.id)
    .map(u => u.nickname);

  return (
    <Measure whitelist={['width', 'height']}>
      {
        (dimensions) => {
          const {
            width: mapViewWidth,
            height: mapViewHeight,
          } = calculateMapViewDimensions(dimensions);
          return (
            <Grid
              className="u-full-height"
              gutter={ThemeSize.Small}
              responsiveness={{
                [ThemeSize.Small]: GridMode.Full,
                [ThemeSize.Medium]: GridMode.Full,
              }}
            >
              <GridCell fixedWidth>
                <Widget
                  className="u-higher"
                  bodyClassName="u-window-box--none"
                  header="Map"
                >
                  <Map
                    {...map}
                    viewWidth={mapViewWidth}
                    viewHeight={mapViewHeight}
                    cellRenderer={cellRenderer}
                    selection={mapSelection}
                    onSelectionChanged={onMapSelectionChange}
                  />
                </Widget>
              </GridCell>
              <GridCell>
                <Deck>
                  <DeckItem>
                    <Widget
                      header="Planet Details"
                      className="u-higher"
                    >
                      <PlanetCard
                        {...selectedPlanet}
                        owner={players[selectedPlanet.ownerId]}
                      />
                    </Widget>
                  </DeckItem>
                  <DeckItem stretch>
                    <Widget
                      header="Chat"
                      className="u-higher u-full-height"
                      bodyClassName="u-window-box--none"
                    >
                      <Chat
                        currentUserId={currentUser.id}
                        players={values(players)}
                        events={events}
                        renderers={eventRenderers}
                        settingsOpen={playerSettingsOpen}
                        settingsChildren={
                          <PlayerSettingsForm
                            values={playerSettings}
                            previousValues={players[currentUser.id]}
                            usedNicknames={usedNicknames}
                            onSubmit={onPlayerSettingsSubmit}
                            onChange={onPlayerSettingsChange}
                          />
                        }
                        onSettingsToggle={onPlayerSettingsSetVisibility}
                        onMessageSend={onMessageSend}
                      />
                    </Widget>
                  </DeckItem>
                  {
                    isLeader && (
                      <DeckItem>
                        <Widget
                          header="Game Settings"
                          className="u-higher"
                        >
                          <GameSettingsForm
                            values={gameSettings}
                            onSubmit={onGameSettingsSubmit}
                            onChange={onGameSettingsChange}
                            onStart={onGameStart}
                          />
                        </Widget>
                      </DeckItem>
                    )
                  }
                </Deck>
              </GridCell>
            </Grid>
          );
        }
    }
    </Measure>
  );
}

LobbyPage.propTypes = {
  gameSettings: GAME_SETTINGS_SHAPE,
  playerSettings: PLAYER_SETTINGS_SHAPE,
  map: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
    planets: PropTypes.objectOf(PLANET_SHAPE).isRequired,
  }).isRequired,
  mapSelection: MAP_SELECTION_SHAPE,
  cellRenderer: PropTypes.func.isRequired,
  players: PropTypes.objectOf(PLAYER_SHAPE).isRequired,
  events: PropTypes.arrayOf(GAME_EVENT_SHAPE).isRequired,
  eventRenderers: PropTypes.objectOf(PropTypes.func).isRequired,
  leaderUserId: PropTypes.string.isRequired,
  currentUser: PropTypes.shape({
    id: PropTypes.string.isRequired,
  }).isRequired,
  playerSettingsOpen: PropTypes.bool,
  onMessageSend: PropTypes.func.isRequired,
  onPlayerSettingsSetVisibility: PropTypes.func.isRequired,
  onMapSelectionChange: PropTypes.func.isRequired,
  onGameSettingsSubmit: PropTypes.func.isRequired,
  onGameSettingsChange: PropTypes.func.isRequired,
  onGameStart: PropTypes.func.isRequired,
  onPlayerSettingsChange: PropTypes.func.isRequired,
  onPlayerSettingsSubmit: PropTypes.func.isRequired,
};

LobbyPage.defaultProps = {
  mapSelection: {
    start: null,
    end: null,
  },
  gameSettings: null,
  playerSettings: null,
  playerSettingsOpen: false,
};

export default compose(
  connect(
    null,
    {
      onGameSettingsSubmit: submitGameSettings,
      onGameSettingsChange: changeGameSettings,
      onGameStart: startGame,
      onPlayerSettingsSubmit: submitPlayerSettings,
      onPlayerSettingsChange: changePlayerSettings,
      onPlayerSettingsSetVisibility: setPlayerSettingsVisibility,
    },
  ),
  withPropsOnChange(['map', 'players'], ({ map, players }) => ({
    // eslint-disable-next-line react/prop-types
    cellRenderer: ({ cellIndex }) =>
      <LobbyCell
        cellIndex={cellIndex}
        planets={map.planets}
        players={players}
      />,
  })),
  withPropsOnChange(['eventRenderers'], ({ eventRenderers }) => ({
    eventRenderers: {
      ...eventRenderers,
      ...lobbyEventRenderers,
    },
  })),
  withHandlers({
    onGameSettingsSubmit: ({ onGameSettingsSubmit, roomId }) =>
      settings => onGameSettingsSubmit({ roomId, settings }),
    onGameSettingsChange: ({ onGameSettingsChange, roomId }) =>
      settings => onGameSettingsChange({ roomId, settings }),
    onPlayerSettingsChange: ({ onPlayerSettingsChange, roomId }) =>
      settings => onPlayerSettingsChange({ roomId, settings }),
    onPlayerSettingsSubmit: ({ onPlayerSettingsSubmit, roomId }) =>
      settings => onPlayerSettingsSubmit({ roomId, settings }),
    onGameStart: ({ onGameStart, roomId }) => () => onGameStart({ roomId }),
    onMessageSend: ({ onMessageSend, roomId }) => text => onMessageSend({ roomId, text }),
  }),
)(LobbyPage);
