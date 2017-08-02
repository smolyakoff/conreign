import React, { PropTypes } from 'react';
import { isNumber } from 'lodash';

import {
  Grid,
  GridCell,
  ThemeSize,
  Box,
  BoxType,
  Text,
  TextEmphasize,
  VerticalAlignment,
} from './../../theme';
import PlayerIcon from './player-icon';
import { TurnStatus } from './../../api';
import { PLAYER_WITH_OPTIONAL_STATS } from './player-schema';

export default function PlayerListItem({
  nickname,
  status,
  color,
  turnStatus,
  isCurrent,
  planetsCount,
}) {
  const className = turnStatus === TurnStatus.Ended ? 'u-bg-success-light' : null;
  return (
    <Box
      type={BoxType.Window}
      className={className}
      themeSize={ThemeSize.Small}
    >
      <Grid
        gutter={ThemeSize.Small}
        verticalAlignment={VerticalAlignment.Center}
      >
        <GridCell gutter={false} fixedWidth>
          <PlayerIcon
            color={color}
            status={status}
          />
        </GridCell>
        <GridCell>
          <Text emphasize={isCurrent ? TextEmphasize.Loud : null}>
            {nickname}
          </Text>
        </GridCell>
        {
          isNumber(planetsCount) && (
            <GridCell fixedWidth>
              {planetsCount}
            </GridCell>
          )
        }
      </Grid>
    </Box>
  );
}

PlayerListItem.propTypes = {
  ...PLAYER_WITH_OPTIONAL_STATS,
  isCurrent: PropTypes.bool.isRequired,
};

PlayerListItem.defaultProps = {
  turnStatus: null,
  planetsCount: null,
};
