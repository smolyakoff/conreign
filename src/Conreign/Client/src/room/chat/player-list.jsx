import React from 'react';
import PropTypes from 'prop-types';

import PlayerListItem from './player-list-item';
import { PLAYER_WITH_OPTIONAL_STATS_SHAPE } from './player-schema';

export default function PlayerList({
  players,
  currentUserId,
}) {
  const items = players.map(
    p => (
      <PlayerListItem
        key={p.userId}
        {...p}
        isCurrent={p.userId === currentUserId}
      />
    ),
  );
  return (
    <div>{items}</div>
  );
}

PlayerList.propTypes = {
  players: PropTypes.arrayOf(PLAYER_WITH_OPTIONAL_STATS_SHAPE),
  currentUserId: PropTypes.string.isRequired,
};

PlayerList.defaultProps = {
  players: [],
};
