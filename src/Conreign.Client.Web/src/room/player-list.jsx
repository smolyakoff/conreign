import React, { PropTypes } from 'react';

import { PresenceStatus } from './../core';
import { PLAYER_SHAPE, PLAYER } from './schemas';
import {
  Table,
  TableBody,
  TableRow,
  TableCell,
} from './../theme';
import { Circle } from './icons';

const StatusColor = {
  [PresenceStatus.Online]: '#4CAF50',
  [PresenceStatus.Offline]: '#F44336',
};

function Player({
  nickname,
  status,
  color,
}) {
  return (
    <TableRow>
      <TableCell>
        <span style={{ color }}>{nickname}</span>
      </TableCell>
      <TableCell fixedWidth>
        {status === PresenceStatus.Online ? 'Online' : 'Offline'}
      </TableCell>
      <TableCell fixedWidth>
        <Circle
          className="u-xsmall"
          color={StatusColor[status]}
        />
      </TableCell>
    </TableRow>
  );
}

Player.propTypes = PLAYER;

export default function PlayerList({
  players,
}) {
  const rows = players.map(
    (p, i) => <Player key={p.userId} index={i} {...p} />,
  );
  return (
    <Table>
      <TableBody>
        {rows}
      </TableBody>
    </Table>
  );
}

PlayerList.propTypes = {
  players: PropTypes.arrayOf(PLAYER_SHAPE),
};

PlayerList.defaultProps = {
  players: [],
};
