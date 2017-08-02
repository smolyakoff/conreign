import { conforms, matches } from 'lodash';

import {
  isSucceededAsyncAction,
  getOriginalPayload,
  createFailedAsyncActionType,
 } from '../framework';
import {
  JOIN_ROOM,
  UserErrorCategory,
  GameplayErrorCode,
} from '../api';
import {
  ignoreErrorActionNotification,
} from '../errors';

const JOIN_ROOM_FAILED = createFailedAsyncActionType(JOIN_ROOM);

function joinRoom(action$, state, { apiDispatcher, history }) {
  return action$
    .ofType(JOIN_ROOM)
    .mergeMap(apiDispatcher)
    .map(ignoreErrorActionNotification)
    .do((action) => {
      if (isSucceededAsyncAction(action)) {
        history.push(`/${getOriginalPayload(action).roomId}`);
      }
    });
}

const initialState = {
  busyRoomId: null,
};

const isGameInProgressErrorAction = conforms({
  type: type => type === JOIN_ROOM_FAILED,
  payload: matches({
    category: UserErrorCategory.Gameplay,
    code: GameplayErrorCode.GameIsAlreadyInProgress,
  }),
});

function reducer(state = initialState, action) {
  if (isGameInProgressErrorAction(action)) {
    return {
      ...state,
      busyRoomId: getOriginalPayload(action).roomId,
    };
  }
  return state;
}

export function selectBusyRoomId(state) {
  return state.busyRoomId;
}

export default {
  epic: joinRoom,
  reducer,
};
