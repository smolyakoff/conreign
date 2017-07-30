import { isSucceededAsyncAction, getOriginalPayload } from '../framework';

import { JOIN_ROOM } from '../api';

function joinRoom(action$, state, { apiDispatcher, history }) {
  return action$
    .ofType(JOIN_ROOM)
    .mergeMap(apiDispatcher)
    .doIf(
      isSucceededAsyncAction,
      action => history.push(`/${getOriginalPayload(action).roomId}`),
    );
}

function reducer(state) {
  return state;
}

export default {
  epic: joinRoom,
  reducer,
};
