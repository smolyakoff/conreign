import React, { PropTypes } from 'react';

import formatDate from 'date-fns/format';
import {
  Grid,
  GridCell,
  Icon,
  ThemeSize,
  Deck,
  DeckItem,
  Orientation,
  Text,
  ThemeColor,
  TextEmphasize,
} from './../../theme';
import { PLAYER_SHAPE } from './../schemas';
import PlayerIcon from './player-icon';
import vader from './vader.svg';

const TIME_FORMAT = 'HH:MM';

export default function ChatMessage({
  sender,
  timestamp,
  children,
  ...others
}) {
  return (
    <Grid gutter={ThemeSize.Small} {...others}>
      <GridCell fixedWidth>
        {
          sender
            ? (
              <PlayerIcon
                color={sender.color}
                status={sender.status}
              />
            )
            : (
              <Icon name={vader} />
            )
        }
      </GridCell>
      <GridCell>
        <Deck
          className="u-mb-small"
          orientation={Orientation.Horizontal}
        >
          <DeckItem>
            <Text emphasize={TextEmphasize.Loud}>
              {sender ? sender.nickname : 'DARK FORCE'}
            </Text>
          </DeckItem>
          <DeckItem stretch>
            <Text themeColor={ThemeColor.Default}>
              {formatDate(timestamp, TIME_FORMAT)}
            </Text>
          </DeckItem>
        </Deck>
        {children}
      </GridCell>
    </Grid>
  );
}

ChatMessage.propTypes = {
  sender: PLAYER_SHAPE,
  timestamp: PropTypes.instanceOf(Date).isRequired,
  children: PropTypes.node,
};

ChatMessage.defaultProps = {
  sender: null,
  children: null,
};
