import React from 'react';

import "./home-page.scss";
import JoinRoomForm from './join-room-form';

export function HomePage() {
  return (
    <div className="sky u-full-height">
      <div className="sky__star sky__star--small"></div>
      <div className="sky__star sky__star--medium"></div>
      <div className="u-absolute-center">
        <h1>Welcome to Conreign</h1>
        <JoinRoomForm/>
      </div>
    </div>
  );
}

export default HomePage;
