import React from 'react';

import { PLAYER_JOINED, PLAYER_UPDATED } from './../../api';
import { P } from './../../theme';
import { PLAYER_SHAPE } from './../room-schemas';
import { Nickname } from './../text';
import { WELCOME_MESSAGE_RECEIVED, WelcomeMessageEvent } from './welcome-message-event';


function PlayerJoined({ player }) {
  return (
    <P>
      <Nickname {...player} /> joined the game.
    </P>
  );
}

PlayerJoined.propTypes = {
  player: PLAYER_SHAPE.isRequired,
};

function PlayerUpdated({ currentPlayer, previousPlayer }) {
  return (
    <P>
      <Nickname {...previousPlayer} /> has changed the name to <Nickname {...currentPlayer} />
    </P>
  );
}

PlayerUpdated.propTypes = {
  currentPlayer: PLAYER_SHAPE.isRequired,
  previousPlayer: PLAYER_SHAPE.isRequired,
};

export default {
  [PLAYER_JOINED]: PlayerJoined,
  [PLAYER_UPDATED]: PlayerUpdated,
  [WELCOME_MESSAGE_RECEIVED]: WelcomeMessageEvent,
};
