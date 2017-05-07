import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { values, mapValues } from 'lodash';
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
import { Map, Planet, MAP_SELECTION_SHAPE } from './../map';
import {
  updateGameOptions,
  changeGameOptions,
  updatePlayerOptions,
  changePlayerOptions,
  setPlayerOptionsVisibility,
  startGame,
} from './lobby';
import { GAME_OPTIONS_SHAPE, PLAYER_OPTIONS_SHAPE } from './lobby-schemas';
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

function LobbyMapPlanetCell({
  cellIndex,
  planets,
  players,
}) {
  const planet = planets[cellIndex];
  const owner = players[planet.ownerId];
  const color = owner ? owner.color : null;
  return <Planet {...planet} color={color} />;
}
LobbyMapPlanetCell.propTypes = {
  cellIndex: PropTypes.number.isRequired,
  planets: PropTypes.objectOf(PLANET_SHAPE).isRequired,
  players: PropTypes.objectOf(PLAYER_SHAPE).isRequired,
};

function LobbyPage({
  playerOptions,
  playerOptionsOpen,
  gameOptions,
  map,
  mapSelection,
  mapCells,
  players,
  events,
  eventRenderers,
  leaderUserId,
  currentUser,
  onMessageSend,
  onSetPlayerOptionsVisibility,
  onMapCellClick,
  onGameOptionsUpdate,
  onGameOptionsChange,
  onPlayerOptionsChange,
  onPlayerOptionsUpdate,
  onGameStart,
}) {
  const selectedPlanet = map.planets[mapSelection.start];
  const isLeader = leaderUserId === currentUser.id;
  playerOptions = playerOptions || players[currentUser.id];

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
                    cells={mapCells}
                    selection={mapSelection}
                    onCellClick={onMapCellClick}
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
                        settingsOpen={playerOptionsOpen}
                        settingsChildren={
                          <PlayerSettingsForm
                            values={playerOptions}
                            previousValues={players[currentUser.id]}
                            usedNicknames={usedNicknames}
                            onSubmit={onPlayerOptionsUpdate}
                            onChange={onPlayerOptionsChange}
                          />
                        }
                        onSettingsToggle={onSetPlayerOptionsVisibility}
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
                            values={gameOptions}
                            onSubmit={onGameOptionsUpdate}
                            onChange={onGameOptionsChange}
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
  gameOptions: GAME_OPTIONS_SHAPE.isRequired,
  playerOptions: PLAYER_OPTIONS_SHAPE,
  map: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
    planets: PropTypes.objectOf(PLANET_SHAPE).isRequired,
  }).isRequired,
  mapSelection: MAP_SELECTION_SHAPE,
  mapCells: PropTypes.objectOf(PropTypes.node).isRequired,
  players: PropTypes.objectOf(PLAYER_SHAPE).isRequired,
  events: PropTypes.arrayOf(GAME_EVENT_SHAPE).isRequired,
  eventRenderers: PropTypes.objectOf(PropTypes.func).isRequired,
  leaderUserId: PropTypes.string.isRequired,
  currentUser: PropTypes.shape({
    id: PropTypes.string.isRequired,
  }).isRequired,
  playerOptionsOpen: PropTypes.bool,
  onMessageSend: PropTypes.func.isRequired,
  onSetPlayerOptionsVisibility: PropTypes.func.isRequired,
  onMapCellClick: PropTypes.func.isRequired,
  onGameOptionsUpdate: PropTypes.func.isRequired,
  onGameOptionsChange: PropTypes.func.isRequired,
  onGameStart: PropTypes.func.isRequired,
  onPlayerOptionsChange: PropTypes.func.isRequired,
  onPlayerOptionsUpdate: PropTypes.func.isRequired,
};

LobbyPage.defaultProps = {
  mapSelection: {
    start: null,
    end: null,
  },
  playerOptions: null,
  playerOptionsOpen: false,
};

function generateMapCells({ planets, players }) {
  return mapValues(planets, (planet) => {
    const owner = planet.ownerId ? players[planet.ownerId] : null;
    const color = owner ? owner.color : null;
    return <Planet {...planet} color={color} />;
  });
}

const onMapCellClick =
  ({ onMapSelectionChange, mapSelection }) =>
  ({ cellIndex }) => onMapSelectionChange({ ...mapSelection, start: cellIndex });

const enhance = compose(
  connect(
    null,
    {
      onGameOptionsUpdate: updateGameOptions,
      onGameOptionsChange: changeGameOptions,
      onPlayerOptionsUpdate: updatePlayerOptions,
      onPlayerOptionsChange: changePlayerOptions,
      onSetPlayerOptionsVisibility: setPlayerOptionsVisibility,
      onGameStart: startGame,
    },
  ),
  withPropsOnChange(['map', 'players'], ({ map, players }) => ({
    mapCells: generateMapCells({
      players,
      planets: map.planets,
    }),
  })),
  withPropsOnChange(['eventRenderers'], ({ eventRenderers }) => ({
    eventRenderers: {
      ...eventRenderers,
      ...lobbyEventRenderers,
    },
  })),
  withHandlers({
    onGameOptionsUpdate: ({ onGameOptionsUpdate, roomId }) =>
      options => onGameOptionsUpdate({ roomId, options }),
    onGameOptionsChange: ({ onGameOptionsChange, roomId }) =>
      options => onGameOptionsChange({ roomId, options }),
    onPlayerOptionsChange: ({ onPlayerOptionsChange, roomId }) =>
      options => onPlayerOptionsChange({ roomId, options }),
    onPlayerOptionsUpdate: ({ onPlayerOptionsUpdate, roomId }) =>
      options => onPlayerOptionsUpdate({ roomId, options }),
    onGameStart: ({ onGameStart, roomId }) => () => onGameStart({ roomId }),
    onMessageSend: ({ onMessageSend, roomId }) => text => onMessageSend({ roomId, text }),
    onMapCellClick,
  }),
);

export default enhance(LobbyPage);
