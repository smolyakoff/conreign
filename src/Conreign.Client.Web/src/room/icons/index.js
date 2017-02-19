import fp from 'lodash/fp';

import earth from './earth.svg';
import mars from './mars.svg';
import moon from './moon.svg';

export const ICON = {
  earth,
  mars,
  moon,
};

export const PLANET_ICON_LIMITS = {
  0: 'moon',
  50: 'mars',
  80: 'earth',
};

export function choosePlanetIcon(power, limits = PLANET_ICON_LIMITS) {
  const name = fp.flow(
    fp.toPairs,
    fp.map(([limit, iconName]) => [parseInt(limit, 10) / 100, iconName]),
    fp.orderBy(([limit]) => limit, 'desc'),
    fp.find(([limit]) => power >= limit),
    fp.last,
  )(limits);
  return {
    name,
    id: ICON[name],
  };
}

export { default as Circle } from './circle';
