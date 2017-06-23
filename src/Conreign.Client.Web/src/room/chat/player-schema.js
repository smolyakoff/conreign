import { values } from 'lodash';
import { PropTypes } from 'react';

import { TurnStatus } from './../../api';
import { PLAYER } from './../room-schemas';

export const PLAYER_WITH_OPTIONAL_STATS = {
  ...PLAYER,
  turnStatus: PropTypes.oneOf(values(TurnStatus)),
  planetsCount: PropTypes.number,
};

export const PLAYER_WITH_OPTIONAL_STATS_SHAPE = PropTypes.shape(PLAYER_WITH_OPTIONAL_STATS);
