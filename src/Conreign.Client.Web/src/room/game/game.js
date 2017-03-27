import { pick } from 'lodash';
import { combineEpics } from 'redux-observable';

import { TurnStatus, RoomMode } from './../../core';

const HANDLE_GAME_STARTED = 'HANDLE_GAME_STARTED';
const LAUNCH_FLEET = 'LAUNCH_FLEET';

function reducer(state, action) {
  switch (action.type) {
    case HANDLE_GAME_STARTED:
      return {
        ...pick(state, [
          'roomId',
          'players',
          'map',
          'leaderUserId',
          'playerStatuses',
        ]),
        events: [],
        mode: RoomMode.Game,
        deadPlayers: [],
        turn: 0,
        waitingFleets: [],
        movingFleets: [],
        turnStatus: TurnStatus.Thinking,
      };
    default:
      return state;
  }
}

function createEpic({ apiDispathcer }) {
  function launchFleetEpic(action$) {
    return action$
      .ofType(LAUNCH_FLEET)
      .mergeMap(apiDispathcer);
  }

  return combineEpics(launchFleetEpic);
}

export default {
  reducer,
  createEpic,
};
