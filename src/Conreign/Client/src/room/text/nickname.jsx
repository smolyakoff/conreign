import React from 'react';
import PropTypes from 'prop-types';

import { Text } from './../../theme';

export default function Nickname({ color, nickname }) {
  const style = { color };
  return <Text style={style}>{nickname}</Text>;
}

Nickname.propTypes = {
  color: PropTypes.string.isRequired,
  nickname: PropTypes.string.isRequired,
};
