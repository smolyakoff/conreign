import { PropTypes } from 'react';

export const GAME_SETTINGS_SHAPE = PropTypes.shape({
  mapWidth: PropTypes.number.isRequired,
  mapHeight: PropTypes.number.isRequired,
  neutralPlanetsCount: PropTypes.number.isRequired,
});

function reducer(state) {
  return state;
}

export default {
  reducer,
};
