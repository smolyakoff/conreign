import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import Measure from 'react-measure';
import { compose, withPropsOnChange, withHandlers, withProps } from 'recompose';
import { mapValues, isNumber, isNil, sumBy } from 'lodash';

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
  Fleet,
  updateMapSelection,
  getDistance,
  MAP_SELECTION_SHAPE,
} from './../map';
import { PLANET_SHAPE, PLAYER_SHAPE } from './../room-schemas';
import { FLEET_SHAPE } from './game-schemas';
import {
  launchFleet,
  changeFleet,
  selectFleet,
  cancelFleet,
  endTurn,
  selectWaitingFleetsWithDetails,
  selectMovingFleetsByPosition,
} from './game';
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
  fleetShips,
  waitingFleets,
  selectedFleetIndex,
  players,
  currentUser,
  onMapCellClick,
  onFleetSelect,
  onFleetFormSubmit,
  onFleetFormChange,
  onCancelFleetClick,
  onEndTurnClick,
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
  const finalFleetShips = isNil(fleetShips) ? sourcePlanet.ships : fleetShips;
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
                  onCellClick={onMapCellClick}
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
                  fleetShips={finalFleetShips}
                  waitingFleets={waitingFleets}
                  selectedFleetIndex={selectedFleetIndex}
                  onFleetClick={onFleetSelect}
                  onFleetFormSubmit={onFleetFormSubmit}
                  onFleetFormChange={onFleetFormChange}
                  onCancelFleetClick={onCancelFleetClick}
                  onEndTurnClick={onEndTurnClick}
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
  fleetShips: PropTypes.oneOfType([
    PropTypes.number,
    PropTypes.string,
  ]),
  selectedFleetIndex: PropTypes.number,
  waitingFleets: PropTypes.arrayOf(FLEET_SHAPE).isRequired,
  onMapCellClick: PropTypes.func.isRequired,
  onFleetSelect: PropTypes.func.isRequired,
  onFleetFormSubmit: PropTypes.func.isRequired,
  onFleetFormChange: PropTypes.func.isRequired,
  onCancelFleetClick: PropTypes.func.isRequired,
  onEndTurnClick: PropTypes.func.isRequired,
};

GamePage.defaultProps = {
  turnSeconds: null,
  fleetShips: null,
  selectedFleetIndex: null,
};

function generateMapCells(planets, players, movingFleets, waitingFleets) {
  const planetCells = mapValues(planets, (planet, position) => {
    const owner = planet.ownerId ? players[planet.ownerId] : null;
    const color = owner ? owner.color : null;
    const cellFleets = [
      ...movingFleets[position],
      ...waitingFleets.filter(x => x.from.name === planet.name),
    ].filter(x => x);
    const fleetShips = sumBy(cellFleets, fleet => fleet.ships);
    return (
      <Planet
        {...planet}
        fleetShips={fleetShips}
        color={color}
        displayMode={PlanetDisplayMode.Game}
      />
    );
  });
  const fleetCells = mapValues(movingFleets, (fleets) => {
    const totalShips = sumBy(fleets, fleet => fleet.ships);
    return (
      <Fleet ships={totalShips} />
    );
  });
  return {
    ...fleetCells,
    ...planetCells,
  };
}

const onMapCellClick =
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

const onFleetFormSubmit = ({ onFleetLaunch, mapSelection, roomId }) => ({ ships }) => {
  onFleetLaunch({
    roomId,
    fleet: {
      from: mapSelection.start,
      to: mapSelection.end,
      ships,
    },
  });
};

const onCancelFleetClick = ({ selectedFleetIndex, onFleetCancel, roomId }) => () => {
  onFleetCancel({
    index: selectedFleetIndex,
    roomId,
  });
};

const onEndTurnClick = ({ onEndTurnClick: emit, roomId }) => () => {
  emit({ roomId });
};

const enhance = compose(
  connect(null, {
    onFleetSelect: selectFleet,
    onFleetLaunch: launchFleet,
    onFleetCancel: cancelFleet,
    onFleetFormChange: changeFleet,
    onEndTurnClick: endTurn,
  }),
  withProps(props => ({
    waitingFleets: selectWaitingFleetsWithDetails(props),
    movingFleets: selectMovingFleetsByPosition(props),
  })),
  withPropsOnChange(
    ['map', 'players', 'movingFleets', 'waitingFleets'],
    ({ map, players, movingFleets, waitingFleets }) => ({
      mapCells: generateMapCells(
        map.planets,
        players,
        movingFleets,
        waitingFleets,
      ),
    }),
  ),
  withHandlers({
    onMapCellClick,
    onEndTurnClick,
    onCancelFleetClick,
    onFleetFormSubmit,
  }),
);

export default enhance(GamePage);
