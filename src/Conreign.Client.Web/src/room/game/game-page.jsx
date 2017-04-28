import React, { PropTypes } from 'react';
import Measure from 'react-measure';
import { compose, withPropsOnChange, withHandlers } from 'recompose';
import { mapValues, isNumber } from 'lodash';

import {
  Grid,
  GridCell,
  GridMode,
  Box,
  Widget,
  ThemeSize,
} from './../../theme';

import {
  Map,
  Planet,
  PlanetDisplayMode,
  updateMapSelection,
  getDistance,
  MAP_SELECTION_SHAPE,
} from './../map';
import { PLANET_SHAPE, PLAYER_SHAPE } from './../room-schemas';
import GameStatusBoard from './game-status-board';
import CommandCenter from './command-center';

const WIDGET_HEADER_HEIGHT = 35;
const STATUS_BOARD_HEIGHT = 78;

function calculateMapViewDimensions(viewDimensions) {
  const {
    width: fullVw,
    height: fullVh,
  } = viewDimensions;
  const vw = fullVw / 2;
  const vh = fullVh - WIDGET_HEADER_HEIGHT - STATUS_BOARD_HEIGHT;
  return { width: vw, height: vh };
}

function GamePage({
  map,
  mapSelection,
  mapCells,
  turn,
  turnSeconds,
  players,
  currentUser,
  onMapCellFocus,
}) {
  const currentPlayer = players[currentUser.id];
  const sourcePlanet = map.planets[mapSelection.start];
  const destinationPlanet = isNumber(mapSelection.end)
    ? map.planets[mapSelection.end]
    : null;
  const destinationPlanetOwner = destinationPlanet && destinationPlanet.ownerId
    ? players[destinationPlanet.ownerId]
    : null;
  const routeDistance = destinationPlanet
    ? getDistance(mapSelection.start, mapSelection.end, map.width)
    : null;
  return (
    <Measure>
      {(dimensions) => {
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
                header="Map"
                className="u-higher"
                bodyClassName="u-window-box--none"
              >
                <Box themeSize={ThemeSize.Medium}>
                  <GameStatusBoard
                    turn={turn}
                    turnSeconds={turnSeconds}
                    player={currentPlayer}
                  />
                </Box>
                <Map
                  {...map}
                  selection={mapSelection}
                  cells={mapCells}
                  viewWidth={mapViewWidth}
                  viewHeight={mapViewHeight}
                  onCellFocus={onMapCellFocus}
                />
              </Widget>
            </GridCell>
            <GridCell>
              <Widget
                header="Command Center"
                className="u-higher"
              >
                <CommandCenter
                  currentPlayer={currentPlayer}
                  sourcePlanet={sourcePlanet}
                  destinationPlanet={destinationPlanet}
                  destinationPlanetOwner={destinationPlanetOwner}
                  routeDistance={routeDistance}
                />
              </Widget>
            </GridCell>
          </Grid>
        );
      }}
    </Measure>
  );
}

GamePage.propTypes = {
  map: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
    planets: PropTypes.objectOf(PLANET_SHAPE).isRequired,
  }).isRequired,
  mapSelection: MAP_SELECTION_SHAPE.isRequired,
  mapCells: PropTypes.objectOf(PropTypes.node).isRequired,
  players: PropTypes.objectOf(PLAYER_SHAPE).isRequired,
  currentUser: PropTypes.shape({
    id: PropTypes.string.isRequired,
  }).isRequired,
  turn: PropTypes.number.isRequired,
  turnSeconds: PropTypes.number,
  onMapCellFocus: PropTypes.func.isRequired,
};

GamePage.defaultProps = {
  turnSeconds: null,
};

function generateMapCells({ planets, players }) {
  const planetCells = mapValues(planets, (planet) => {
    const owner = planet.ownerId ? players[planet.ownerId] : null;
    const color = owner ? owner.color : null;
    return (
      <Planet
        {...planet}
        color={color}
        displayMode={PlanetDisplayMode.Game}
      />
    );
  });
  return planetCells;
}

const onMapCellFocus =
  ({ onMapSelectionChange, map, mapSelection, currentUser }) =>
  ({ cellIndex }) => {
    const updatedSelection = updateMapSelection({
      cellIndex,
      mapSelection,
      planets: map.planets,
      currentUserId: currentUser.id,
    });
    if (updatedSelection !== mapSelection) {
      onMapSelectionChange(updatedSelection);
    }
  };

const enhance = compose(
  withPropsOnChange(['map', 'players'], ({ map, players }) => ({
    mapCells: generateMapCells({
      players,
      planets: map.planets,
    }),
  })),
  withHandlers({
    onMapCellFocus,
  }),
);

export default enhance(GamePage);
