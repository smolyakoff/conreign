import React from 'react';
import { connect } from 'react-redux';

import { getRoomState, selectRoom } from './room';

function RoomPage() {
  return (
    <div>Room</div>
  );
}

RoomPage.init = ({ params }) => getRoomState(params);

export default connect(
  (state, { params }) => selectRoom(state, params.roomId),
)(RoomPage);
