const JOIN_ROOM = 'JOIN_ROOM';

export function joinRoom(payload) {
  return {
    type: JOIN_ROOM,
    payload,
  };
}

function createEpic({ apiClient, history }) {
  function joinRoomEpic(action$) {
    return action$.ofType(JOIN_ROOM)
      .mergeMap(action =>
        apiClient
          .send(action)
          .do(() => history.push(`/${action.payload.roomId}`))
          .ignoreElements(),
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
