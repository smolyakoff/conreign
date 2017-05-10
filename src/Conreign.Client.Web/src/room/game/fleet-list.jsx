import React, { PropTypes } from 'react';
import { pure } from 'recompose';
import { noop } from 'lodash';

import {
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  HorizontalAlignment,
} from './../../theme';
import { FLEET_SHAPE } from './game-schemas';

function FleetListItem({
  fleet,
  ...others
}) {
  return (
    <TableRow {...others}>
      <TableCell>
        {fleet.from.name} ➔ {fleet.to.name}
      </TableCell>
      <TableCell
        horizontalAlignment={HorizontalAlignment.Right}
      >
        {fleet.distance}
      </TableCell>
      <TableCell
        horizontalAlignment={HorizontalAlignment.Right}
      >
        {fleet.ships}
      </TableCell>
    </TableRow>
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
    <Table clickable>
      <TableHead>
        <TableRow heading>
          <TableCell>Route</TableCell>
          <TableCell
            horizontalAlignment={HorizontalAlignment.Right}
          >
            Distance
          </TableCell>
          <TableCell
            horizontalAlignment={HorizontalAlignment.Right}
          >
            Ships
          </TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
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
      </TableBody>
    </Table>
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
