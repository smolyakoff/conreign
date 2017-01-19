import React, { PropTypes } from 'react';
import { connect } from 'react-redux';

import { Grid, GridCell, ThemeSize, GridMode } from './../../theme';
import { Map, PlanetCell, MAP_SELECTION_SHAPE } from '../map';
import { setMapSelection } from './../room';


function LobbyPage({
  roomId,
  map,
  players,
  viewDimensions,
  mapSelection,
  onMapSelectionChanged,
}) {
  const mapSize = Math.min(viewDimensions.width / 2, viewDimensions.height);

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

  return (
    <Grid
      responsiveness={{
        [ThemeSize.Small]: GridMode.Full,
      }}
    >
      <GridCell fixedWidth width={mapSize}>
        <div className="u-window-box--small">
          <Map
            {...map}
            cellRenderer={LobbyCell}
            selection={mapSelection}
            onSelectionChanged={selection => onMapSelectionChanged({ selection, roomId })}
          />
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

const PLANET_SHAPE = PropTypes.shape({
  name: PropTypes.string.isRequired,
  productionRate: PropTypes.number.isRequired,
  power: PropTypes.number.isRequired,
  ships: PropTypes.number.isRequired,
  ownerId: PropTypes.string,
});

const PLAYER_SHAPE = PropTypes.shape({
  userId: PropTypes.string.isRequired,
  color: PropTypes.string.isRequired,
  nickname: PropTypes.string,
});

LobbyPage.propTypes = {
  viewDimensions: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
  }).isRequired,
  roomId: PropTypes.string.isRequired,
  map: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
    planets: PropTypes.objectOf(PLANET_SHAPE).isRequired,
  }).isRequired,
  mapSelection: MAP_SELECTION_SHAPE,
  onMapSelectionChanged: PropTypes.func,
  players: PropTypes.objectOf(PLAYER_SHAPE).isRequired,
};

LobbyPage.defaultProps = {
  mapSelection: {
    start: null,
    end: null,
  },
  onMapSelectionChanged: null,
};

export default connect(
  null,
  {
    onMapSelectionChanged: setMapSelection,
  },
)(LobbyPage);
