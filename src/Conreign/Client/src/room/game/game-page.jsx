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
      playerStatistics: selectExtendedPlayerStatistics(props),
    }))(GameStatistics),
  ),
);

export default enhance(GameField);
