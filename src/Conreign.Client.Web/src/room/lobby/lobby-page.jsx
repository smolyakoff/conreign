import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { findKey, parseInt, isNumber, values } from 'lodash';
import Measure from 'react-measure';

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
  Box,
  BoxType,
} from './../../theme';
import { Map, PlanetCell, MAP_SELECTION_SHAPE } from './../map';
import {
  submitGameSettings,
  changeGameSettings,
  submitPlayerSettings,
  changePlayerSettings,
  setPlayerSettingsVisibility,
} from './lobby';
import { GAME_SETTINGS_SHAPE, PLAYER_SETTINGS_SHAPE } from './lobby-schemas';
import PlanetCard from './../planet-card';
import Chat from './../chat';
import lobbyEventRenderers from './lobby-event-renderers';
import GameSettingsForm from './game-settings-form';
import PlayerSettingsForm from './player-settings-form';

const WIDGET_HEADER_HEIGHT = 21;

function calculateMapSize(viewDimensions, mapDimensions) {
  const {
    width: fullVw,
    height: fullVh,
  } = viewDimensions;
  const vw = fullVw / 2;
  const vh = fullVh - WIDGET_HEADER_HEIGHT;
  const {
    width: mw,
    height: mh,
  } = mapDimensions;
  const wCellSize = vw / mw;
  const hCellSize = vh / mh;
  const size = Math.min(wCellSize, hCellSize) * mw;
  return size;
}

function adjustMapSelection(map, mapSelection, currentUser) {
  if (isNumber(mapSelection.start)) {
    return mapSelection;
  }
  const currentUserPlanetPosition = findKey(
    map.planets,
    p => p.ownerId === currentUser.id,
  );
  return { start: parseInt(currentUserPlanetPosition) };
}

function LobbyPage({
  roomId,
  playerSettings,
  gameSettings,
  map,
  players,
  events,
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
}) {
  function LobbyCell({ cellIndex }) {
    const planet = map.planets[cellIndex];
    if (!planet) {
      return null;
    }
    const owner = players[planet.ownerId];
    return <PlanetCell planet={planet} owner={owner} />;
  }
  LobbyCell.propTypes = {
    cellIndex: PropTypes.number.isRequired,
  };
  const chosenMapSelection = adjustMapSelection(
    map,
    mapSelection,
    currentUser,
  );
  let selectedPlanet = map.planets[chosenMapSelection.start];
  selectedPlanet = {
    ...selectedPlanet,
    owner: players[selectedPlanet.ownerId],
  };
  const isLeader = leaderUserId === currentUser.id;
  gameSettings = gameSettings || {
    mapWidth: map.width,
    mapHeight: map.height,
    neutralPlanetsCount: values(map.planets)
      .filter(planet => !planet.ownerId)
      .length,
  };
  playerSettings = playerSettings || players[currentUser.id];

  const usedNicknames = values(players)
    .filter(p => p.userId !== currentUser.id)
    .map(u => u.nickname);

  return (
    <Box
      className="u-full-height"
      type={BoxType.Letter}
      themeSize={ThemeSize.Small}
    >
      <Measure whitelist={['width', 'height']}>
        {
          dimensions => (
            <Grid
              className="u-full-height"
              gutter={ThemeSize.Small}
              responsiveness={{
                [ThemeSize.Small]: GridMode.Full,
                [ThemeSize.Medium]: GridMode.Full,
              }}
            >
              <GridCell
                fixedWidth
                width={calculateMapSize(dimensions, map)}
              >
                <Widget
                  className="u-higher"
                  bodyClassName="u-window-box--none"
                  header="Map"
                >
                  <Map
                    {...map}
                    cellRenderer={LobbyCell}
                    selection={chosenMapSelection}
                    onSelectionChanged={selection => onMapSelectionChange({ selection, roomId })}
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
                      <PlanetCard {...selectedPlanet} />
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
                            onSubmit={settings => onGameSettingsSubmit({ settings, roomId })}
                            onChange={settings => onGameSettingsChange({ settings, roomId })}
                          />
                        </Widget>
                      </DeckItem>
                    )
                  }
                  <DeckItem stretch>
                    <Widget
                      header="Chat"
                      className="u-higher u-full-height"
                      bodyClassName="u-window-box--none"
                    >
                      <Chat
                        players={values(players)}
                        events={events}
                        renderers={{
                          ...eventRenderers,
                          ...lobbyEventRenderers,
                        }}
                        settingsOpen={playerSettingsOpen}
                        settingsChildren={
                          <PlayerSettingsForm
                            values={playerSettings}
                            previousValues={players[currentUser.id]}
                            usedNicknames={usedNicknames}
                            onSubmit={settings => onPlayerSettingsSubmit({ settings, roomId })}
                            onChange={settings => onPlayerSettingsChange({ settings, roomId })}
                          />
                        }
                        onSettingsToggle={
                          visible => onPlayerSettingsSetVisibility({ roomId, visible })
                        }
                        onMessageSend={text => onMessageSend({ roomId, text })}
                      />
                    </Widget>
                  </DeckItem>
                </Deck>
              </GridCell>
            </Grid>
          )
        }
      </Measure>
    </Box>
  );
}

LobbyPage.propTypes = {
  roomId: PropTypes.string.isRequired,
  gameSettings: GAME_SETTINGS_SHAPE,
  playerSettings: PLAYER_SETTINGS_SHAPE,
  map: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
    planets: PropTypes.objectOf(PLANET_SHAPE).isRequired,
  }).isRequired,
  mapSelection: MAP_SELECTION_SHAPE,
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

export default connect(
  null,
  {
    onGameSettingsSubmit: submitGameSettings,
    onGameSettingsChange: changeGameSettings,
    onPlayerSettingsSubmit: submitPlayerSettings,
    onPlayerSettingsChange: changePlayerSettings,
    onPlayerSettingsSetVisibility: setPlayerSettingsVisibility,
  },
)(LobbyPage);
