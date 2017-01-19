import React, { PropTypes } from 'react';
import block from 'bem-cn';
import Color from 'color';
import fp from 'lodash/fp';

import { Progress, PropertyTable, ThemeSize, ThemeColor } from './../../theme';
import './planet.scss';
import earth from './icons/earth.svg';
import mars from './icons/mars.svg';
import moon from './icons/moon.svg';

const ICON = {
  earth,
  mars,
  moon,
};

export default function Planet({
  name,
  power,
  productionRate,
  icons,
  color,
}) {
  const planet = block('c-planet');
  const style = {};
  if (color) {
    style.backgroundColor = Color(color).alpha(0.8).string();
  }
  const chosenIconName = fp.flow(
    fp.toPairs,
    fp.map(([limit, iconName]) => [parseInt(limit, 10) / 100, iconName]),
    fp.orderBy(([limit]) => limit, 'desc'),
    fp.find(([limit]) => power >= limit),
    fp.last,
  )(icons);

  const planetProperties = [{
    name: 'P',
    value: productionRate,
  }, {
    name: 'E',
    value: (
      <Progress
        value={power * 100}
        themeSize={ThemeSize.Small}
        themeColor={ThemeColor.Success}
      />
    ),
  }];
  return (
    <div className={planet()} style={style}>
      <span className={planet('name')()}>{name}</span>
      <img
        src={ICON[chosenIconName]}
        className={planet('icon')()}
        alt={chosenIconName}
      />
      <div className={planet('planet-properties')()}>
        <PropertyTable
          properties={planetProperties}
        />
      </div>
    </div>
  );
}

Planet.propTypes = {
  name: PropTypes.string.isRequired,
  power: PropTypes.number.isRequired,
  productionRate: PropTypes.number.isRequired,
  icons: PropTypes.objectOf(PropTypes.string),
  color: PropTypes.string,
};

Planet.defaultProps = {
  color: 'transparent',
  icons: {
    0: 'moon',
    50: 'mars',
    80: 'earth',
  },
};
