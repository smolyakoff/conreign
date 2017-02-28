import React, { PropTypes } from 'react';

import { PLAYER_SHAPE, PLAYER } from '../schemas';
import {
  Table,
  TableBody,
  TableRow,
  TableCell,
} from './../../theme';
import PlayerIcon from './player-icon';

function PlayerRow({
  nickname,
  status,
  color,
}) {
  return (
    <TableRow>
      <TableCell fixedWidth>
        <PlayerIcon color={color} status={status} />
      </TableCell>
      <TableCell>
        {nickname}
      </TableCell>
    </TableRow>
  );
}

PlayerRow.propTypes = PLAYER;

export default function PlayerList({
  players,
}) {
  const rows = players.map(
    (p, i) => <PlayerRow key={p.userId} index={i} {...p} />,
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
