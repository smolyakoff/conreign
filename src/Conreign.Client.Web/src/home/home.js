import { isSucceededAsyncAction, getOriginalPayload } from '../framework';

import { JOIN_ROOM } from '../api';

function createEpic({ apiDispatcher, history }) {
  function joinRoomEpic(action$) {
    return action$
      .ofType(JOIN_ROOM)
      .mergeMap(apiDispatcher)
      .doIf(
        isSucceededAsyncAction,
        action => history.push(`/${getOriginalPayload(action).roomId}`),
      );
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
