import { PropTypes } from 'react';

export const GAME_SETTINGS_SHAPE = PropTypes.shape({
  mapWidth: PropTypes.number.isRequired,
  mapHeight: PropTypes.number.isRequired,
  neutralPlanetsCount: PropTypes.number.isRequired,
});

export const PLAYER_SETTINGS_SHAPE = PropTypes.shape({
  nickname: PropTypes.string.isRequired,
  color: PropTypes.string.isRequired,
});
