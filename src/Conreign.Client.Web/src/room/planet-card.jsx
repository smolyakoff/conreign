import React, { PropTypes } from 'react';
import { pure } from 'recompose';
import block from 'bem-cn';

import {
  H1,
  Grid,
  Icon,
  GridCell,
  PropertyTable,
  VerticalAlignment,
  ThemeSize,
} from './../theme';

import { choosePlanetIcon, Circle } from './icons';
import './planet-card.scss';

function PlanetCard({
  className,
  name,
  productionRate,
  power,
  owner,
  ships,
}) {
  const color = owner ? owner.color : 'white';
  const ownerValue = (
    <div>
      <Circle color={color} /> {owner ? owner.nickname : 'Neutral'}
    </div>
  );
  const planetProperties = [
    {
      name: 'Owner',
      value: ownerValue,
    },
    {
      name: 'Production Rate',
      value: productionRate,
    },
    {
      name: 'Power',
      value: `${Math.ceil(power * 100)}%`,
    },
    {
      name: 'Ships',
      value: ships,
    },
  ];

  const icon = choosePlanetIcon(power);
  const css = block('c-planet-details');

  return (
    <Grid
      className={className}
      gutter={ThemeSize.Medium}
      verticalAlignment={VerticalAlignment.Center}
    >
      <GridCell
        className={css('icon-container')()}
        fixedWidth
      >
        <Icon name={icon.id} />
        <H1>{name}</H1>
      </GridCell>
      <GridCell>
        <PropertyTable properties={planetProperties} />
      </GridCell>
    </Grid>
  );
}

PlanetCard.propTypes = {
  className: PropTypes.string,
  name: PropTypes.string.isRequired,
  productionRate: PropTypes.number.isRequired,
  power: PropTypes.number.isRequired,
  owner: PropTypes.shape({
    nickname: PropTypes.string.isRequired,
    color: PropTypes.string.isRequired,
  }),
  ships: PropTypes.number.isRequired,
};

PlanetCard.defaultProps = {
  className: null,
  owner: null,
};

export default pure(PlanetCard);
