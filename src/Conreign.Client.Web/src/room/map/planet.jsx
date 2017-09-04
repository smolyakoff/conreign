import React from 'react';
import PropTypes from 'prop-types';
import { pure } from 'recompose';
import block from 'bem-cn';
import Color from 'color';
import { values, kebabCase } from 'lodash';
import {
  Progress,
  PropertyTable,
  ThemeSize,
  ThemeColor,
  Icon,
} from './../../theme';
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
  fleetShips,
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
        rounded
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
    <div className={planet(propModifiers)()} style={style}>
      <span className={planet('name')()}>{name}</span>
      {
        displayMode === PlanetDisplayMode.Game && fleetShips > 0 && (
          <div className={planet('port')()}>
            ▲{fleetShips}
          </div>
        )
      }
      <div className={planet('icon')()}>
        <Icon name={icon.id} />
      </div>
      <div className={planet('footer')()}>
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
  fleetShips: PropTypes.number,
  icons: PropTypes.objectOf(PropTypes.string),
  color: PropTypes.string,
  displayMode: PropTypes.oneOf(values(PlanetDisplayMode)),
};

Planet.defaultProps = {
  color: 'transparent',
  fleetShips: 0,
  icons: {
    0: 'moon',
    50: 'mars',
    80: 'earth',
  },
  displayMode: PlanetDisplayMode.Lobby,
};

export default pure(Planet);
