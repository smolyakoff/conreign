import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';
import { values } from 'lodash';

import { PresenceStatus, PlayerType } from './../../api';
import { Text, TextEmphasize } from './../../theme';
import './player-icon.scss';

const block = bem('c-player-icon');
const statusElement = block('status');
const aiLabelElement = block('ai-label');

export default function PlayerIcon({
  className,
  color,
  status,
  type,
}) {
  const modifiers = {
    online: status === PresenceStatus.Online,
  };
  return (
    <div
      className={block.mix(className)()}
      style={{ backgroundColor: color }}
    >
      {
        type === PlayerType.Human && (
          <div className={statusElement(modifiers)()} />
        )
      }
      {
        type === PlayerType.Bot && (
          <Text
            className={aiLabelElement()}
            emphasize={TextEmphasize.Loud}
          >
            AI
          </Text>
        )
      }
    </div>
  );
}

PlayerIcon.propTypes = {
  className: PropTypes.string,
  color: PropTypes.string.isRequired,
  status: PropTypes.oneOf(values(PresenceStatus)).isRequired,
  type: PropTypes.oneOf(values(PlayerType)).isRequired,
};

PlayerIcon.defaultProps = {
  className: null,
};
