import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { values } from 'lodash';

import { RoomMode } from './../core';
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
  const props = {
    ...otherProps,
    eventRenderers: renderers,
  };
  switch (mode) {
    case RoomMode.Lobby:
      return <LobbyPage {...props} />;
    case RoomMode.Game:
      return <GamePage {...props} />;
    default:
      throw new Error(`Unexpected room mode: ${mode}.`);
  }
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
