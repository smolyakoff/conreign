import React, { PropTypes } from 'react';
import { padStart, isNumber } from 'lodash';

import {
  Grid,
  GridCell,
  Text,
  VerticalAlignment,
  TextEmphasize,
} from './../../theme';
import { PLAYER_SHAPE } from './../room-schemas';

function formatTurnTime(seconds) {
  if (!isNumber(seconds)) {
    return 'Loading...';
  }
  return `00:${padStart(seconds.toString(), 2, '0')}`;
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
        <div>{turn + 1}</div>
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
  turnSeconds: PropTypes.number,
  player: PLAYER_SHAPE.isRequired,
};

GameStatusBoard.defaultProps = {
  turnSeconds: null,
};

export default GameStatusBoard;
