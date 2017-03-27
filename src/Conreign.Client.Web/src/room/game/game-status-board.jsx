import React, { PropTypes } from 'react';

import { Grid, GridCell } from './../../theme';

function formatTurnTime(seconds) {
  return `00:${seconds}`;
}

function GameStatusBoard({
  turn,
  turnSeconds,
}) {
  return (
    <Grid>
      <GridCell>
        Turn {turn + 1}
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
};

export default GameStatusBoard;
