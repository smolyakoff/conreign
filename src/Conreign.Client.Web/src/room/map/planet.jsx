import React, { PropTypes } from 'react';
import block from 'bem-cn';
import Color from 'color';
import { Progress, PropertyTable, ThemeSize, ThemeColor, Icon } from './../../theme';
import './planet.scss';
import { choosePlanetIcon } from './../icons';

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
  const icon = choosePlanetIcon(power, icons);

  const planetProperties = [{
    name: 'R',
    value: productionRate,
  }, {
    name: 'P',
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
      <div className={planet('icon')()}>
        <Icon name={icon.id} />
      </div>
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
