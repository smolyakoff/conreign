import React, { PropTypes } from 'react';
import { pure } from 'recompose';
import block from 'bem-cn';
import Color from 'color';
import { values, kebabCase } from 'lodash';
import { Progress, PropertyTable, ThemeSize, ThemeColor, Icon } from './../../theme';
import './planet.scss';
import { choosePlanetIcon } from './../icons';

export const PlanetDisplayMode = {
  Lobby: 'Lobby',
  Game: 'Game',
};

function Planet({
  name,
  power,
  productionRate,
  icons,
  color,
  ships,
  displayMode,
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
  const properties = displayMode === PlanetDisplayMode.Lobby
    ? (
      <PropertyTable
        themeSpacing={ThemeSize.XSmall}
        properties={planetProperties}
      />
    )
    : ships;
  const propModifiers = {
    mode: kebabCase(displayMode),
  };
  return (
    <div className={planet()} style={style}>
      <span className={planet('name')()}>{name}</span>
      <div className={planet('icon')()}>
        <Icon name={icon.id} />
      </div>
      <div className={planet('planet-properties')(propModifiers)()}>
        {properties}
      </div>
    </div>
  );
}

Planet.propTypes = {
  name: PropTypes.string.isRequired,
  power: PropTypes.number.isRequired,
  productionRate: PropTypes.number.isRequired,
  ships: PropTypes.number.isRequired,
  icons: PropTypes.objectOf(PropTypes.string),
  color: PropTypes.string,
  displayMode: PropTypes.oneOf(values(PlanetDisplayMode)),
};

Planet.defaultProps = {
  color: 'transparent',
  icons: {
    0: 'moon',
    50: 'mars',
    80: 'earth',
  },
  displayMode: PlanetDisplayMode.Lobby,
};

export default pure(Planet);
