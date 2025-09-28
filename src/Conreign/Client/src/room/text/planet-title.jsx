import React from 'react';
import PropTypes from 'prop-types';
import { pick, capitalize as doCapitalize, identity } from 'lodash';
import { Text, TextEmphasize, ThemeColor } from './../../theme';

import { PLANET } from './../room-schemas';

export default function PlanetTitle({
  name,
  ownerId,
  capitalize,
}) {
  const transform = capitalize ? doCapitalize : identity;
  const prefix = transform(ownerId ? 'planet' : 'neutral planet');
  return (
    <Text>
      {prefix}
      {' '}
      <Text
        emphasize={TextEmphasize.Loud}
        themeColor={ThemeColor.Brand}
      >
        {name}
      </Text>
    </Text>
  );
}

PlanetTitle.propTypes = {
  ...pick(PLANET, ['name', 'ownerId']),
  capitalize: PropTypes.bool,
};

PlanetTitle.defaultProps = {
  capitalize: false,
};
