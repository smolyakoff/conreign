import React from 'react';
import PropTypes from 'prop-types';
import block from 'bem-cn';
import { values } from 'lodash';

import { PresenceStatus } from './../../api';
import './player-icon.scss';

const css = block('c-player-icon');

export default function PlayerIcon({ className, color, status }) {
  const modifiers = {
    online: status === PresenceStatus.Online,
  };
  return (
    <div
      className={css.mix(className)()}
      style={{ backgroundColor: color }}
    >
      <div className={css('status')(modifiers)()} />
    </div>
  );
}

PlayerIcon.propTypes = {
  className: PropTypes.string,
  color: PropTypes.string.isRequired,
  status: PropTypes.oneOf(values(PresenceStatus)).isRequired,
};

PlayerIcon.defaultProps = {
  className: null,
};
