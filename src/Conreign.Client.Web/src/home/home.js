import { isSucceededAsyncAction } from './../core';

const JOIN_ROOM = 'JOIN_ROOM';

export function joinRoom(payload) {
  return {
    type: JOIN_ROOM,
    payload,
  };
}

function createEpic({ apiDispatcher, history }) {
  function joinRoomEpic(action$) {
    return action$.ofType(JOIN_ROOM)
      .mergeMap(action => apiDispatcher(action)
        .do((result) => {
          if (!isSucceededAsyncAction(result, JOIN_ROOM)) {
            return;
          }
          history.push(`/${action.payload.roomId}`);
        }));
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
