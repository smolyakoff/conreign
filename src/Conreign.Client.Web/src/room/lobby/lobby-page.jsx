import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { findKey, parseInt, isNumber, values } from 'lodash';

import { PLAYER_SHAPE, PLANET_SHAPE } from './../schemas';
import { Grid, GridCell, ThemeSize, GridMode, Box, Widget } from './../../theme';
import { Map, PlanetCell, MAP_SELECTION_SHAPE } from '../map';
import { submitGameSettings, changeGameSettings, GAME_SETTINGS_SHAPE } from './lobby';
import PlanetCard from './../planet-card';
import Chat from './../chat';
import GameSettingsForm from './game-settings-form';

const WIDGET_HEADER_HEIGHT = 40;

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
  gameSettings,
  map,
  players,
  leaderUserId,
  viewDimensions,
  mapSelection,
  currentUser,
  onMapSelectionChange,
  onGameSettingsSubmit,
  onGameSettingsChange,
}) {
  const mapSize = calculateMapSize(viewDimensions, map);

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

  return (
    <Grid
      responsiveness={{
        [ThemeSize.Small]: GridMode.Full,
        [ThemeSize.Medium]: GridMode.Full,
      }}
    >
      <GridCell
        fixedWidth
        width={mapSize}
      >
        <Box themeSize={ThemeSize.Small}>
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
        </Box>
      </GridCell>
      <GridCell>
        <Box themeSize={ThemeSize.Small}>
          <Widget
            header="Planet Details"
            className="u-higher"
          >
            <PlanetCard {...selectedPlanet} />
          </Widget>
          {
            isLeader && (
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
            )
          }
          <Widget
            header="Chat"
            className="u-higher"
          >
            <Chat />
          </Widget>
        </Box>
      </GridCell>
    </Grid>
  );
}

LobbyPage.propTypes = {
  viewDimensions: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
  }).isRequired,
  roomId: PropTypes.string.isRequired,
  gameSettings: GAME_SETTINGS_SHAPE,
  map: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
    planets: PropTypes.objectOf(PLANET_SHAPE).isRequired,
  }).isRequired,
  mapSelection: MAP_SELECTION_SHAPE,
  players: PropTypes.objectOf(PLAYER_SHAPE).isRequired,
  leaderUserId: PropTypes.string.isRequired,
  currentUser: PropTypes.shape({
    id: PropTypes.string.isRequired,
  }).isRequired,
  onMapSelectionChange: PropTypes.func.isRequired,
  onGameSettingsSubmit: PropTypes.func.isRequired,
  onGameSettingsChange: PropTypes.func.isRequired,
};

LobbyPage.defaultProps = {
  mapSelection: {
    start: null,
    end: null,
  },
  gameSettings: null,
};

export default connect(
  null,
  {
    onGameSettingsSubmit: submitGameSettings,
    onGameSettingsChange: changeGameSettings,
  },
)(LobbyPage);
