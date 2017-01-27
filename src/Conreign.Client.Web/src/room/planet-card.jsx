import React, { PropTypes } from 'react';
import block from 'bem-cn';

import {
  H1,
  Card,
  CardItem,
  CardBody,
  Grid,
  Image,
  GridCell,
  PropertyTable,
  ThemeColor,
  VerticalAlignment,
} from './../theme';

import { choosePlanetIcon, Circle } from './icons';
import './planet-card.scss';

export default function PlanetCard({
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
      <Circle color={color} /> {owner ? owner.nickname : 'Neutral'},
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
    <Card className={className}>
      <CardItem themeColor={ThemeColor.Info}>
        Planet Details
      </CardItem>
      <CardBody className={css()}>
        <Grid gutter verticalAlignment={VerticalAlignment.Center}>
          <GridCell
            className={css('icon-container')()}
            fixedWidth
          >
            <Image src={icon.src} alt={icon.name} />
            <H1>{name}</H1>
          </GridCell>
          <GridCell>
            <PropertyTable properties={planetProperties} />
          </GridCell>
        </Grid>
      </CardBody>
    </Card>
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
