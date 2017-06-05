import React, { PropTypes } from 'react';
import { values } from 'lodash';

import { ATTACK_HAPPENED, AttackOutcome } from './../../api';
import { P } from './../../theme';
import { PlanetTitle, Nickname } from './../text';
import { PLAYER_SHAPE, PLANET_SHAPE } from './../room-schemas';

function AttackHappened({
  attacker,
  planet,
  outcome,
}) {
  if (outcome === AttackOutcome.Win) {
    return (
      <P>
        <PlanetTitle {...planet} capitalize />
        {' '}
        was conquered by
        {' '}
        <Nickname {...attacker} />!
      </P>
    );
  }
  return (
    <P>
      <PlanetTitle {...planet} capitalize />
      {' '}
      held an attack from
      {' '}
      <Nickname {...attacker} />.
    </P>
  );
}

AttackHappened.propTypes = {
  attacker: PLAYER_SHAPE.isRequired,
  planet: PLANET_SHAPE.isRequired,
  outcome: PropTypes.oneOf(values(AttackOutcome)).isRequired,
};

export default {
  [ATTACK_HAPPENED]: AttackHappened,
};
