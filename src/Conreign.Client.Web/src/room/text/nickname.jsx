import React from 'react';
import { omit } from 'lodash';

import { Text } from './../../theme';
import { PLAYER } from './../room-schemas';

export default function Nickname({ color, nickname }) {
  const style = { color };
  return <Text style={style}>{nickname}</Text>;
}

Nickname.propTypes = omit(PLAYER, 'status');
