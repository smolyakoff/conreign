import { PropTypes } from 'react';
import { values } from 'lodash';

import { PresenceStatus, GameEventType } from './../core';

export const PLAYER = {
  userId: PropTypes.string.isRequired,
  color: PropTypes.string.isRequired,
  nickname: PropTypes.string,
  status: PropTypes.oneOf(values(PresenceStatus)).isRequired,
};

export const PLAYER_SHAPE = PropTypes.shape(PLAYER);

export const PLANET_SHAPE = PropTypes.shape({
  name: PropTypes.string.isRequired,
  productionRate: PropTypes.number.isRequired,
  power: PropTypes.number.isRequired,
  ships: PropTypes.number.isRequired,
  ownerId: PropTypes.string,
});

export const GAME_EVENT_SHAPE = PropTypes.shape({
  type: PropTypes.oneOf(values(GameEventType)).isRequired,
  payload: PropTypes.any.isRequired,
});
