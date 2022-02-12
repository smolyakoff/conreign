import PropTypes from 'prop-types';

export const GAME_OPTIONS_SHAPE = PropTypes.shape({
  mapWidth: PropTypes.number.isRequired,
  mapHeight: PropTypes.number.isRequired,
  neutralPlanetsCount: PropTypes.number.isRequired,
});

export const PLAYER_OPTIONS_SHAPE = PropTypes.shape({
  nickname: PropTypes.string.isRequired,
  color: PropTypes.string.isRequired,
});
