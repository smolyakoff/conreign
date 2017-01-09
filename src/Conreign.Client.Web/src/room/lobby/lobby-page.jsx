import React, { PropTypes } from 'react';

import { Grid, GridCell, ThemeSize, GridMode } from './../../theme';
import Map from '../map/map';

function LobbyPage({ map }) {
  return (
    <Grid
      className="u-full-height"
      responsiveness={{
        [ThemeSize.Small]: GridMode.Full,
      }}
    >
      <GridCell>
        <Map {...map} />
      </GridCell>
      <GridCell>
        Content
      </GridCell>
    </Grid>
  );
}

LobbyPage.propTypes = {
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
};

export default LobbyPage;
