import React, { PropTypes } from 'react';
import { omit } from 'lodash';

import { GameEventType } from './../../core';
import { Text, P } from './../../theme';
import { PLAYER_SHAPE, PLAYER } from './../room-schemas';

function Nick({ player }) {
  const style = { color: player.color };
  return <Text style={style}>{player.nickname}</Text>;
}

Nick.propTypes = {
  player: PropTypes.shape(omit(PLAYER, 'status')).isRequired,
};

function PlayerJoined({ player }) {
  return (
    <P>
      <Nick player={player} /> joined the game.
    </P>
  );
}

PlayerJoined.propTypes = {
  player: PLAYER_SHAPE.isRequired,
};

function PlayerUpdated({ currentPlayer, previousPlayer }) {
  return (
    <P>
      <Nick player={previousPlayer} /> has changed the name to <Nick player={currentPlayer} />
    </P>
  );
}

PlayerUpdated.propTypes = {
  currentPlayer: PLAYER_SHAPE.isRequired,
  previousPlayer: PLAYER_SHAPE.isRequired,
};

export default {
  [GameEventType.PlayerJoined]: PlayerJoined,
  [GameEventType.PlayerUpdated]: PlayerUpdated,
};
