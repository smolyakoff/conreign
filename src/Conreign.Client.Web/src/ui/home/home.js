const JOIN_ROOM = 'JOIN_ROOM';

export function joinRoom(payload) {
  return {
    type: JOIN_ROOM,
    payload,
  };
}

function createEpic({ apiClient }) {
  function joinRoomEpic(action$) {
    return action$.ofType(JOIN_ROOM)
      .mergeMap(apiClient.send);
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
