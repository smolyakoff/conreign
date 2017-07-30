import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { values } from 'lodash';

import { Box, BoxType, ThemeSize } from './../theme';
import { selectRoom, selectAuth } from './../state';
import { selectUser } from './../auth';
import {
  RoomMode,
  setMapSelection,
  getRoomState,
  sendMessage,
  selectRoomOfUser,
} from './room';
import { LobbyPage } from './lobby';
import { GamePage } from './game';

function RoomPage({ mode, ...otherProps }) {
  return (
    <Box
      className="u-full-height"
      type={BoxType.Letter}
      themeSize={ThemeSize.Small}
    >
      {
        (() => {
          switch (mode) {
            case RoomMode.Lobby:
              return (
                <LobbyPage {...otherProps} />
              );
            case RoomMode.Game:
              return (
                <GamePage {...otherProps} />
              );
            default:
              throw new Error(`Unexpected room mode: ${mode}.`);
          }
        })()
      }
    </Box>
  );
}

RoomPage.propTypes = {
  mode: PropTypes.oneOf(values(RoomMode)).isRequired,
  onMapSelectionChange: PropTypes.func.isRequired,
  onMessageSend: PropTypes.func.isRequired,
};
RoomPage.init = ({ params }) => getRoomState(params);

function selectUserRoom(state) {
  const auth = selectAuth(state);
  const user = selectUser(auth);
  const room = selectRoom(state);
  return selectRoomOfUser(room, user);
}

export default connect(
  selectUserRoom,
  {
    onMapSelectionChange: setMapSelection,
    onMessageSend: sendMessage,
  },
)(RoomPage);
