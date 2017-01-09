import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { values } from 'lodash';

import { RoomMode } from './gameplay';
import { getRoomState, selectRoom } from './room';
import { LobbyPage } from './lobby';
import { GamePage } from './game';


function RoomPage({ mode, ...otherParams }) {
  switch (mode) {
    case RoomMode.Lobby:
      return <LobbyPage {...otherParams} />;
    case RoomMode.Game:
      return <GamePage {...otherParams} />;
    default:
      throw new Error(`Unexpected room mode: ${mode}.`);
  }
}
RoomPage.propTypes = {
  mode: PropTypes.oneOf(values(RoomMode)).isRequired,
};
RoomPage.init = ({ params }) => getRoomState(params);

export default connect(
  (state, { params }) => selectRoom(state, params.roomId),
)(RoomPage);
