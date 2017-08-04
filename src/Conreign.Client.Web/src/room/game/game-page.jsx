/* eslint-disable */
import React from 'react';
import {
  branch,
  renderComponent,
  mapProps,
} from 'recompose';
import { isObject } from 'lodash';

import { selectExtendedPlayerStatistics } from './game';
import GameStatistics from './game-statistics';
import GameField from './game-field';

const enhance = branch(
  ({ playerStatistics }) => isObject(playerStatistics),
  renderComponent(
    mapProps(props => ({
      roomId: props.roomId,
      playerStatistics: selectExtendedPlayerStatistics(props)
    }))(GameStatistics)
  ),
);

export default enhance(GameField);

// const players = {
//   john: {
//     userId: 'john',
//     nickname: 'johny',
//     color: 'red',
//     status: 0,
//   },
//   alice: {
//     userId: 'alice',
//     nickname: 'boomer',
//     color: 'blue',
//     status: 1,
//   },
//   natalie: {
//     userId: 'natalie',
//     nickname: 'natalie',
//     color: 'green',
//     status: 1,
//   },
// };
//
// const stats = {
//   john: {
//     deathTurn: 35,
//     shipsProduced: 15,
//     battlesWon: 5,
//     battlesLost: 3,
//     shipsLost: 48,
//     shipsDestroyed: 185,
//   },
//   alice: {
//     deathTurn: null,
//     shipsProduced: 25,
//     battlesWon: 5,
//     battlesLost: 3,
//     shipsLost: 48,
//     shipsDestroyed: 185,
//   },
//   natalie: {
//     deathTurn: 25,
//     shipsProduced: 46,
//     battlesWon: 12,
//     battlesLost: 56,
//     shipsLost: 48,
//     shipsDestroyed: 185,
//   },
// };
//
// const stats2 = selectExtendedPlayerStatistics({
//   players,
//   playerStatistics: stats,
// });
//
// export default () => {
//   return (
//     <GameStatistics playerStatistics={stats2}/>
//   );
// }
