import React from 'react';

import { Text } from './../../theme';
import { PLAYER } from './../room-schemas';

export default function Nickname({ color, nickname }) {
  const style = { color };
  return <Text style={style}>{nickname}</Text>;
}

Nickname.propTypes = {
  color: PLAYER.color.isRequired,
  nickname: PLAYER.nickname.isRequired,
};
