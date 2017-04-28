import { isSucceededAsyncAction } from '../framework';

import { JOIN_ROOM } from '../api';

function createEpic({ apiDispatcher, history }) {
  function joinRoomThenRedirectOnSuccess(action) {
    return apiDispatcher(action)
      .doIf(
        isSucceededAsyncAction,
        () => history.push(`/${action.payload.roomId}`),
      );
  }

  function joinRoomEpic(action$) {
    return action$
      .ofType(JOIN_ROOM)
      .mergeMap(joinRoomThenRedirectOnSuccess);
  }

  return joinRoomEpic;
}

function reducer(state) {
  return state;
}

export default {
  createEpic,
  reducer,
};
