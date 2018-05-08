import React from 'react';
import PropTypes from 'prop-types';
import { isNumber } from 'lodash';
import bem from 'bem-cn';

import {
  Grid,
  GridCell,
  ThemeSize,
  Box,
  BoxType,
  Text,
  TextEmphasize,
  VerticalAlignment,
  mapEnumValueToModifierValue,
} from './../../theme';
import PlayerIcon from './player-icon';
import { TurnStatus, PlayerType } from './../../api';
import { PLAYER_WITH_OPTIONAL_STATS } from './player-schema';
import './player-list-item.scss';

const block = bem('c-player-list-item');
const nicknameElement = block('nickname');

export default function PlayerListItem({
  nickname,
  status,
  type,
  color,
  turnStatus,
  isDead,
  isCurrent,
  planetsCount,
}) {
  const modifiers = {
    'player-type': mapEnumValueToModifierValue(type, PlayerType),
    'turn-status': mapEnumValueToModifierValue(turnStatus, TurnStatus),
    dead: isDead,
  };
  return (
    <Box
      className={block(modifiers)()}
      type={BoxType.Window}
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
            type={type}
          />
        </GridCell>
        <GridCell>
          <Text
            className={nicknameElement()}
            emphasize={isCurrent ? TextEmphasize.Loud : null}
          >
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
  isDead: PropTypes.bool,
  isCurrent: PropTypes.bool.isRequired,
};

PlayerListItem.defaultProps = {
  isDead: false,
  turnStatus: null,
  planetsCount: null,
};
