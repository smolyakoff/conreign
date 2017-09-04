import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { flow } from 'lodash';

import './home-page.scss';
import { joinRoom } from './../api';
import { selectHome, selectPendingActionCount } from './../state';
import { selectBusyRoomId } from './home';
import JoinRoomForm from './join-room-form';

function HomePage({ ...formProps }) {
  return (
    <div className="sky u-full-height">
      <div className="sky__star sky__star--small" />
      <div className="sky__star sky__star--medium" />
      <div className="u-absolute-center u-centered c-join-room-form-container">
        <h1>Welcome to Conreign</h1>
        <JoinRoomForm {...formProps} />
      </div>
    </div>
  );
}

HomePage.propTypes = {
  onSubmit: PropTypes.func.isRequired,
  busyRoomId: PropTypes.string,
  isSubmitting: PropTypes.bool.isRequired,
};

HomePage.defaultProps = {
  busyRoomId: null,
};

function selectHomePage(state) {
  return {
    busyRoomId: flow(selectHome, selectBusyRoomId)(state),
    isSubmitting: selectPendingActionCount(state) > 0,
  };
}

export default connect(
  selectHomePage,
  { onSubmit: joinRoom },
)(HomePage);
