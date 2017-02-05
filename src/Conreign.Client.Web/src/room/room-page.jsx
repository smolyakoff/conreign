import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import { values } from 'lodash';

import { RoomMode } from './../core';
import { getRoomState, selectRoom, setMapSelection } from './room';
import { LobbyPage } from './lobby';
import { GamePage } from './game';


function RoomPage({ mode, ...otherProps }) {
  switch (mode) {
    case RoomMode.Lobby:
      return <LobbyPage {...otherProps} />;
    case RoomMode.Game:
      return <GamePage {...otherProps} />;
    default:
      throw new Error(`Unexpected room mode: ${mode}.`);
  }
}
RoomPage.propTypes = {
  mode: PropTypes.oneOf(values(RoomMode)).isRequired,
  onMapSelectionChange: PropTypes.func.isRequired,
};
RoomPage.init = ({ params }) => getRoomState(params);

export default connect(
  (state, { params }) => selectRoom(state, params.roomId),
  {
    onMapSelectionChange: setMapSelection,
  },
)(RoomPage);
