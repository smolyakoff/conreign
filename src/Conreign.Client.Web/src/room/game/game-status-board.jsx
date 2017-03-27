import React, { PropTypes } from 'react';

import {
  Grid,
  GridCell,
  Text,
  VerticalAlignment,
  TextEmphasize,
} from './../../theme';
import { PLAYER_SHAPE } from './../room-schemas';

function formatTurnTime(seconds) {
  return `00:${seconds}`;
}

function GameStatusBoard({
  turn,
  turnSeconds,
  player,
}) {
  return (
    <Grid
      className="u-centered u-large"
      verticalAlignment={VerticalAlignment.Center}
    >
      <GridCell>
        <div>Turn</div>
        <div>{turn}</div>
      </GridCell>
      <GridCell>
        <Text
          emphasize={TextEmphasize.Loud}
          style={{ color: player.color }}
        >
          {player.nickname}
        </Text>
      </GridCell>
      <GridCell>
        {formatTurnTime(turnSeconds)}
      </GridCell>
    </Grid>
  );
}

GameStatusBoard.propTypes = {
  turn: PropTypes.number.isRequired,
  turnSeconds: PropTypes.number.isRequired,
  player: PLAYER_SHAPE.isRequired,
};

export default GameStatusBoard;
