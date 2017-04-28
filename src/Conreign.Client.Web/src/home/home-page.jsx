import React, { PropTypes } from 'react';
import { connect } from 'react-redux';

import './home-page.scss';
import { joinRoom } from './../api';
import JoinRoomForm from './join-room-form';

function HomePage({ onJoin }) {
  return (
    <div className="sky u-full-height">
      <div className="sky__star sky__star--small" />
      <div className="sky__star sky__star--medium" />
      <div className="u-absolute-center u-centered">
        <h1>Welcome to Conreign</h1>
        <JoinRoomForm onJoin={onJoin} />
      </div>
    </div>
  );
}

HomePage.propTypes = {
  onJoin: PropTypes.func.isRequired,
};

export default connect(
  () => ({}),
  dispatch => ({
    onJoin: form => dispatch(joinRoom(form)),
  }),
)(HomePage);
