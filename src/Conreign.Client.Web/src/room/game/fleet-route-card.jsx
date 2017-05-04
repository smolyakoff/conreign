import React, { PropTypes } from 'react';

import {
  Box,
  Grid,
  GridCell,
  P,
  GridMode,
  ThemeSize,
  ThemeColor,
} from './../../theme';
import PlanetCard from './../planet-card';
import { PLANET_SHAPE, PLAYER_SHAPE } from './../room-schemas';
import FleetRouteLabel,
{
  PlanetOwnerType,
  FleetRoutePointType,
}
from './fleet-route-label';

function selectOwnerType(planet, currentUserId) {
  if (!planet.ownerId) {
    return PlanetOwnerType.Neutral;
  }
  return planet.ownerId === currentUserId ? PlanetOwnerType.You : PlanetOwnerType.Enemy;
}

function FleetRouteCard({
  sourcePlanet,
  sourcePlanetOwner,
  destinationPlanet,
  destinationPlanetOwner,
  currentUserId,
}) {
  const destinationOwnerType = destinationPlanet
    ? selectOwnerType(destinationPlanet, currentUserId)
    : null;
  return (
    <Grid
      responsiveness={{
        [ThemeSize.Small]: GridMode.Full,
        [ThemeSize.Medium]: GridMode.Full,
        [ThemeSize.Large]: GridMode.Full,
        [ThemeSize.XLarge]: GridMode.Full,
      }}
    >
      <GridCell>
        <Box themeSize={ThemeSize.Small}>
          <FleetRouteLabel
            planetOwnerType={PlanetOwnerType.You}
            pointType={FleetRoutePointType.Source}
          />
        </Box>
        <PlanetCard
          {...sourcePlanet}
          ships={sourcePlanet.ships}
          owner={sourcePlanetOwner}
        />
      </GridCell>
      <GridCell>
        {
          destinationPlanet
            ? (
              <div>
                <Box themeSize={ThemeSize.Small}>
                  <FleetRouteLabel
                    planetOwnerType={destinationOwnerType}
                    pointType={FleetRoutePointType.Destination}
                  />
                </Box>
                <PlanetCard
                  {...destinationPlanet}
                  owner={destinationPlanetOwner}
                />
              </div>
            )
            : (
              <P themeColor={ThemeColor.Info}>
                Select destination planet on the map
              </P>
            )
        }
      </GridCell>
    </Grid>
  );
}

FleetRouteCard.defaultProps = {
  destinationPlanet: null,
  sourcePlanetOwner: null,
  destinationPlanetOwner: null,
};

FleetRouteCard.propTypes = {
  sourcePlanet: PLANET_SHAPE.isRequired,
  sourcePlanetOwner: PLAYER_SHAPE.isRequired,
  destinationPlanet: PLANET_SHAPE,
  destinationPlanetOwner: PLAYER_SHAPE,
  currentUserId: PropTypes.string.isRequired,
};

export default FleetRouteCard;
