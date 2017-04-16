import React from 'react';

import { PLANET_SHAPE, PLAYER_SHAPE } from './../room-schemas';
import { Deck, DeckItem } from './../../theme';
import FleetRouteCard from './fleet-route-card';

function CommandCenter({
  currentPlayer,
  sourcePlanet,
  destinationPlanet,
  destinationPlanetOwner,
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
    </Deck>
  );
}

CommandCenter.propTypes = {
  currentPlayer: PLAYER_SHAPE.isRequired,
  sourcePlanet: PLANET_SHAPE.isRequired,
  destinationPlanet: PLANET_SHAPE,
  destinationPlanetOwner: PLAYER_SHAPE,
};

CommandCenter.defaultProps = {
  destinationPlanet: null,
  destinationPlanetOwner: null,
};

export default CommandCenter;
