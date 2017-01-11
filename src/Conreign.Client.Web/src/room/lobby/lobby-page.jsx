import React, { PropTypes } from 'react';

import { Grid, GridCell, ThemeSize, GridMode } from './../../theme';
import { Map, Planet } from '../map';

function LobbyPage({ map, players, viewDimensions }) {
  function Cell({ cellIndex }) {
    const planet = map.planets[cellIndex];
    if (planet) {
      const owner = players[planet.ownerId];
      const color = owner ? owner.color : null;
      return <Planet {...planet} color={color} />;
    }
    return null;
  }
  Cell.propTypes = {
    cellIndex: PropTypes.number.isRequired,
  };
  const mapSize = Math.min(viewDimensions.width / 2, viewDimensions.height);
  return (
    <Grid
      responsiveness={{
        [ThemeSize.Small]: GridMode.Full,
      }}
    >
      <GridCell fixedWidth width={mapSize}>
        <div className="u-window-box--small">
          <Map {...map} cellRenderer={Cell} />
        </div>
      </GridCell>
      <GridCell>
        <div className="u-window-box--small">
          Content
        </div>
      </GridCell>
    </Grid>
  );
}

LobbyPage.propTypes = {
  viewDimensions: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
  }).isRequired,
  map: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
    planets: PropTypes.objectOf(PropTypes.shape({
      name: PropTypes.string.isRequired,
      productionRate: PropTypes.number.isRequired,
      power: PropTypes.number.isRequired,
      ships: PropTypes.number.isRequired,
      ownerId: PropTypes.string,
    })).isRequired,
  }),
  players: PropTypes.objectOf(PropTypes.shape({
    userId: PropTypes.string.isRequired,
    username: PropTypes.string,
    color: PropTypes.string.isRequired,
  })),
};

export default LobbyPage;
