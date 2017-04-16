import React, { PropTypes } from 'react';
import { values } from 'lodash';
import bem from 'bem-cn';

import { Deck, DeckItem, Orientation } from './../../theme';
import './fleet-route-label.scss';

export const PlanetOwnerType = {
  You: 'you',
  Neutral: 'neutral',
  Enemy: 'enemy',
};

export const FleetRoutePointType = {
  Source: 'source',
  Destination: 'destination',
};

const TEXT_BY_OWNER_TYPE = {
  [PlanetOwnerType.You]: 'You',
  [PlanetOwnerType.Neutral]: 'Neutral',
  [PlanetOwnerType.Enemy]: 'Enemy',
};

const block = bem('c-fleet-route-label');

function FleetRouteLabel({
  planetOwnerType,
  pointType,
}) {
  const modifiers = {
    owner: planetOwnerType,
  };
  return (
    <Deck
      className={block(modifiers)()}
      orientation={Orientation.Horizontal}
    >
      <DeckItem>
        {pointType === FleetRoutePointType.Source
          ? '➔'
          : '⚐'
        }
      </DeckItem>
      <DeckItem>
        {TEXT_BY_OWNER_TYPE[planetOwnerType]}
      </DeckItem>
    </Deck>
  );
}

FleetRouteLabel.propTypes = {
  planetOwnerType: PropTypes.oneOf(values(PlanetOwnerType)).isRequired,
  pointType: PropTypes.oneOf(values(FleetRoutePointType)).isRequired,
};

export default FleetRouteLabel;
