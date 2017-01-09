import { AsyncOperationState, createAsyncActionTypes } from './../core';

const GET_ROOM_STATE = 'GET_ROOM_STATE';
const GET_ROOM_STATE_ACTIONS = createAsyncActionTypes('GET_ROOM_STATE');
const {
  [AsyncOperationState.Completed]: GET_ROOM_STATE_COMPLETED,
} = GET_ROOM_STATE_ACTIONS;

export function getRoomState(payload) {
  return {
    type: GET_ROOM_STATE,
    payload,
  };
}

function createEpic({ apiDispatcher }) {
  function getRoomStateEpic(action$) {
    return action$
      .ofType(GET_ROOM_STATE)
      .mergeMap(apiDispatcher);
  }
  return getRoomStateEpic;
}

function reducer(state = {}, action) {
  switch (action.type) {
    case GET_ROOM_STATE_COMPLETED:
      return {
        ...state,
        [action.payload.roomId]: action.payload,
      };
    default:
      return state;
  }
}
reducer.$key = 'rooms';

export function selectRoom(state, roomId) {
  return state.rooms[roomId];
}

export default {
  createEpic,
  reducer,
};
