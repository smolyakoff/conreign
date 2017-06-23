import React, { PropTypes } from 'react';
import { isNumber } from 'lodash';

import { PLANET_SHAPE, PLAYER_SHAPE } from './../room-schemas';
import { FLEET_SHAPE } from './game-schemas';
import { Deck, DeckItem, Button, Orientation, ThemeColor } from './../../theme';
import FleetRouteCard from './fleet-route-card';
import FleetForm from './fleet-form';
import FleetList from './fleet-list';

function CommandCenter({
  canLaunchFleets,
  currentPlayer,
  sourcePlanet,
  destinationPlanet,
  destinationPlanetOwner,
  routeDistance,
  waitingFleets,
  fleetShips,
  selectedFleetIndex,
  onFleetClick,
  onFleetFormSubmit,
  onFleetFormChange,
  onCancelFleetClick,
  onEndTurnClick,
}) {
  return (
    <Deck>
      <DeckItem>
        <FleetRouteCard
          sourcePlanet={sourcePlanet}
          sourcePlanetOwner={currentPlayer}
          destinationPlanet={destinationPlanet}
          destinationPlanetOwner={destinationPlanetOwner}
          currentUserId={currentPlayer.userId}
        />
      </DeckItem>
      {
        destinationPlanet && canLaunchFleets && (
          <DeckItem>
            <FleetForm
              ships={fleetShips}
              maxShips={sourcePlanet.ships}
              distance={routeDistance}
              onSubmit={onFleetFormSubmit}
              onChange={onFleetFormChange}
            />
          </DeckItem>
        )
      }
      {
        waitingFleets.length > 0 && (
          <DeckItem>
            <FleetList
              items={waitingFleets}
              activeItemIndex={selectedFleetIndex}
              onItemClick={onFleetClick}
            />
          </DeckItem>
        )
      }
      {
        canLaunchFleets && (
          <DeckItem>
            <Deck orientation={Orientation.Horizontal}>
              <DeckItem stretch>
                <Button
                  fullWidth
                  disabled={!isNumber(selectedFleetIndex)}
                  onClick={onCancelFleetClick}
                >
                  Cancel Fleet
                </Button>
              </DeckItem>
              <DeckItem stretch>
                <Button
                  fullWidth
                  themeColor={ThemeColor.Brand}
                  onClick={onEndTurnClick}
                >
                  End Turn
                </Button>
              </DeckItem>
            </Deck>
          </DeckItem>
        )
      }
    </Deck>
  );
}

CommandCenter.propTypes = {
  canLaunchFleets: PropTypes.bool.isRequired,
  currentPlayer: PLAYER_SHAPE.isRequired,
  sourcePlanet: PLANET_SHAPE.isRequired,
  destinationPlanet: PLANET_SHAPE,
  destinationPlanetOwner: PLAYER_SHAPE,
  routeDistance: PropTypes.number,
  fleetShips: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  selectedFleetIndex: PropTypes.number,
  waitingFleets: PropTypes.arrayOf(FLEET_SHAPE).isRequired,
  onFleetClick: PropTypes.func.isRequired,
  onFleetFormSubmit: PropTypes.func.isRequired,
  onFleetFormChange: PropTypes.func.isRequired,
  onCancelFleetClick: PropTypes.func.isRequired,
  onEndTurnClick: PropTypes.func.isRequired,
};

CommandCenter.defaultProps = {
  destinationPlanet: null,
  destinationPlanetOwner: null,
  routeDistance: null,
  selectedFleetIndex: null,
};

export default CommandCenter;
