import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import Measure from 'react-measure';
import { compose, withPropsOnChange, withHandlers, withProps } from 'recompose';
import { mapValues, isNumber, isNil, sumBy, orderBy } from 'lodash';

import {
  Grid,
  GridCell,
  GridMode,
  Deck,
  DeckItem,
  Box,
  Widget,
  ThemeSize,
} from './../../theme';

import { TurnStatus } from './../../api';
import {
  Map,
  Planet,
  PlanetDisplayMode,
  Fleet,
  updateMapSelection,
  getDistance,
  MAP_SELECTION_SHAPE,
} from './../map';
import Chat from './../chat';
import roomEventRenderers from './../room-event-renderers';
import gameEventRenderers from './game-event-renderers';
import { PLANET_SHAPE, PLAYER_SHAPE, GAME_EVENT_SHAPE } from './../room-schemas';
import { FLEET_SHAPE } from './game-schemas';
import {
  launchFleet,
  changeFleet,
  selectFleet,
  cancelFleet,
  endTurn,
  selectGame,
} from './game';
import GameStatusBoard from './game-status-board';
import CommandCenter from './command-center';

const WIDGET_HEADER_HEIGHT = 35;
const STATUS_BOARD_HEIGHT = 78;
const eventRenderers = {
  ...roomEventRenderers,
  ...gameEventRenderers,
};

function calculateMapViewDimensions(viewDimensions) {
  const {
    width: fullVw,
    height: fullVh,
  } = viewDimensions;
  const vw = fullVw / 2;
  const vh = fullVh - WIDGET_HEADER_HEIGHT - STATUS_BOARD_HEIGHT;
  return { width: vw, height: vh };
}

function GameField({
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
  events,
  onMapCellClick,
  onFleetSelect,
  onFleetFormSubmit,
  onFleetFormChange,
  onCancelFleetClick,
  onEndTurnClick,
  onMessageSend,
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
  const sortedPlayers = orderBy(players, ['planetsCount'], ['desc']);
  const isTurnEnded = players[currentUser.id].turnStatus === TurnStatus.Ended;
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
              <Deck>
                <DeckItem>
                  <Widget
                    header="Command Center"
                    className="u-higher"
                  >
                    <CommandCenter
                      canLaunchFleets={!isTurnEnded}
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
                </DeckItem>
                <DeckItem stretch>
                  <Widget
                    header="Chat"
                    className="u-higher u-full-height"
                    bodyClassName="u-window-box--none"
                  >
                    <Chat
                      currentUserId={currentUser.id}
                      players={sortedPlayers}
                      events={events}
                      renderers={eventRenderers}
                      onMessageSend={onMessageSend}
                    />
                  </Widget>

                </DeckItem>
              </Deck>

            </GridCell>
          </Grid>
        );
      }}
    </Measure>
  );
}

GameField.propTypes = {
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
  events: PropTypes.arrayOf(GAME_EVENT_SHAPE).isRequired,
  onMapCellClick: PropTypes.func.isRequired,
  onFleetSelect: PropTypes.func.isRequired,
  onFleetFormSubmit: PropTypes.func.isRequired,
  onFleetFormChange: PropTypes.func.isRequired,
  onCancelFleetClick: PropTypes.func.isRequired,
  onEndTurnClick: PropTypes.func.isRequired,
  onMessageSend: PropTypes.func.isRequired,
};

GameField.defaultProps = {
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

const onMessageSend = ({ onMessageSend: emit, roomId }) =>
  text => emit({ roomId, text });

const enhance = compose(
  connect(null, {
    onFleetSelect: selectFleet,
    onFleetLaunch: launchFleet,
    onFleetCancel: cancelFleet,
    onFleetFormChange: changeFleet,
    onEndTurnClick: endTurn,
  }),
  withProps(selectGame),
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
    onMessageSend,
  }),
);

export default enhance(GameField);
