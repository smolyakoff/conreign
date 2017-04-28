import React, { PropTypes } from 'react';
import block from 'bem-cn';
import { values } from 'lodash';

import { PresenceStatus } from './../../api';
import './player-icon.scss';

const css = block('c-player-icon');

export default function PlayerIcon({ color, status }) {
  const modifiers = {
    online: status === PresenceStatus.Online,
  };
  return (
    <div className={css()} style={{ backgroundColor: color }}>
      <div className={css('status')(modifiers)()} />
    </div>
  );
}

PlayerIcon.propTypes = {
  color: PropTypes.string.isRequired,
  status: PropTypes.oneOf(values(PresenceStatus)).isRequired,
};
