import React, { PropTypes } from 'react';
import { pure } from 'recompose';

import Planet from './planet';

const PLANET_SHAPE = PropTypes.shape({
  name: PropTypes.string.isRequired,
  productionRate: PropTypes.number.isRequired,
  power: PropTypes.number.isRequired,
  ships: PropTypes.number.isRequired,
});

const OWNER_SHAPE = PropTypes.shape({
  color: PropTypes.string.isRequired,
});

function PlanetCell({ planet, owner }) {
  const color = owner ? owner.color : null;
  return <Planet {...planet} color={color} />;
}
PlanetCell.propTypes = {
  planet: PLANET_SHAPE.isRequired,
  owner: OWNER_SHAPE,
};
PlanetCell.defaultProps = {
  owner: null,
};

export default pure(PlanetCell);
