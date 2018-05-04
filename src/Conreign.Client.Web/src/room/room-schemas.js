import PropTypes from 'prop-types';
import { values } from 'lodash';

import { PresenceStatus, PlayerType } from './../api';

export const PLAYER = {
  userId: PropTypes.string.isRequired,
  color: PropTypes.string.isRequired,
  nickname: PropTypes.string,
  status: PropTypes.oneOf(values(PresenceStatus)).isRequired,
  type: PropTypes.oneOf(values(PlayerType)).isRequired,
};

export const PLAYER_SHAPE = PropTypes.shape(PLAYER);

export const PLANET = {
  name: PropTypes.string.isRequired,
  productionRate: PropTypes.number.isRequired,
  power: PropTypes.number.isRequired,
  ships: PropTypes.number.isRequired,
  ownerId: PropTypes.string,
};

export const PLANET_SHAPE = PropTypes.shape(PLANET);

export const GAME_EVENT_SHAPE = PropTypes.shape({
  type: PropTypes.string.isRequired,
  payload: PropTypes.shape({
    timestamp: PropTypes.string.isRequired,
    senderId: PropTypes.string,
  }).isRequired,
});
