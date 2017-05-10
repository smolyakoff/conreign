import { PropTypes } from 'react';

import { PLANET_SHAPE } from './../room-schemas';

export const FLEET = {
  from: PLANET_SHAPE.isRequired,
  to: PLANET_SHAPE.isRequired,
  ships: PropTypes.number.isRequired,
  distance: PropTypes.number.isRequired,
};

export const FLEET_SHAPE = PropTypes.shape(FLEET);
