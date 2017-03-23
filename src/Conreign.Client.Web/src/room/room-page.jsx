import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { values } from 'lodash';

import { RoomMode } from './../core';
import { Box, BoxType, ThemeSize } from './../theme';
import {
  getRoomState,
  selectRoomPage,
  setMapSelection,
  sendMessage,
} from './room';
import { LobbyPage } from './lobby';
import { GamePage } from './game';
import renderers from './room-event-renderers';

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
                <LobbyPage
                  eventRenderers={renderers}
                  {...otherProps}
                />
              );
            case RoomMode.Game:
              return (
                <GamePage
                  eventRenderers={renderers}
                  {...otherProps}
                />
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

export default connect(
  (state, { params }) => selectRoomPage(state, params.roomId),
  {
    onMapSelectionChange: setMapSelection,
    onMessageSend: sendMessage,
  },
)(RoomPage);
