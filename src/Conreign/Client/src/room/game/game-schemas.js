import PropTypes from 'prop-types';

import { PLANET_SHAPE } from './../room-schemas';

export const FLEET = {
  from: PLANET_SHAPE.isRequired,
  to: PLANET_SHAPE.isRequired,
  ships: PropTypes.number.isRequired,
  distance: PropTypes.number.isRequired,
};

export const FLEET_SHAPE = PropTypes.shape(FLEET);

export const GAME_STATISTICS = {
  deathTurn: PropTypes.number,
  shipsProduced: PropTypes.number.isRequired,
  battlesWon: PropTypes.number.isRequired,
  battlesLost: PropTypes.number.isRequired,
  shipsLost: PropTypes.number.isRequired,
  shipsDestroyed: PropTypes.number.isRequired,
};

export const GAME_STATISTICS_SHAPE = PropTypes.shape(GAME_STATISTICS);
