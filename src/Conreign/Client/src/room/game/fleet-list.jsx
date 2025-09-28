import React from 'react';
import PropTypes from 'prop-types';
import { pure } from 'recompose';
import { noop } from 'lodash';

import {
  FlexTable as FTable,
  FlexTableHead as FTHead,
  FlexTableBody as FTBody,
  FlexTableRow as Row,
  FlexTableCell as Cell,
  HorizontalAlignment,
} from './../../theme';
import { FLEET_SHAPE } from './game-schemas';

function FleetListItem({
  fleet,
  ...others
}) {
  return (
    <Row {...others}>
      <Cell>
        {fleet.from.name} ➔ {fleet.to.name}
      </Cell>
      <Cell
        horizontalAlignment={HorizontalAlignment.Right}
      >
        {fleet.distance}
      </Cell>
      <Cell
        horizontalAlignment={HorizontalAlignment.Right}
      >
        {fleet.ships}
      </Cell>
    </Row>
  );
}

FleetListItem.propTypes = {
  fleet: FLEET_SHAPE.isRequired,
};

function FleetList({
  items,
  activeItemIndex,
  onItemClick,
}) {
  return (
    <FTable clickable>
      <FTHead>
        <Row heading>
          <Cell>Route</Cell>
          <Cell
            horizontalAlignment={HorizontalAlignment.Right}
          >
            Distance
          </Cell>
          <Cell
            horizontalAlignment={HorizontalAlignment.Right}
          >
            Ships
          </Cell>
        </Row>
      </FTHead>
      <FTBody>
        {
          items.map((fleet, index) => (
            <FleetListItem
              key={fleet.id || index}
              fleet={fleet}
              active={index === activeItemIndex}
              onClick={e => onItemClick({ fleet, index }, e)}
            />
          ))
        }
      </FTBody>
    </FTable>
  );
}

FleetList.propTypes = {
  items: PropTypes.arrayOf(FLEET_SHAPE).isRequired,
  activeItemIndex: PropTypes.number,
  onItemClick: PropTypes.func,
};

FleetList.defaultProps = {
  activeItemIndex: null,
  onItemClick: noop,
};

export default pure(FleetList);
