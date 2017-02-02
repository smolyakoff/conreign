import { keyBy } from 'lodash';

import { AsyncOperationState, createAsyncActionTypes } from './../core';

const GET_ROOM_STATE = 'GET_ROOM_STATE';
const SET_MAP_SELECTION = 'SET_MAP_SELECTION';
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

export function setMapSelection(payload) {
  return {
    type: SET_MAP_SELECTION,
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

function mapRoomDtoToRoomState(room) {
  const players = keyBy(
    room.players.map(player => ({
      ...player,
      status: room.playerStatuses[player.userId],
    })),
    x => x.userId,
  );
  return {
    ...room,
    players,
  };
}

function reducer(state = {}, action) {
  switch (action.type) {
    case GET_ROOM_STATE_COMPLETED:
      return {
        ...state,
        [action.payload.roomId]: mapRoomDtoToRoomState(action.payload),
      };
    case SET_MAP_SELECTION:
      return {
        ...state,
        [action.payload.roomId]: {
          ...state[action.payload.roomId],
          mapSelection: action.payload.selection,
        },
      };
    default:
      return state;
  }
}
reducer.$key = 'rooms';


export function selectRoom(state, roomId) {
  return {
    ...state[reducer.$key][roomId],
    roomId,
    currentUser: {
      id: state.auth.user.sub,
    },
  };
}

export default {
  createEpic,
  reducer,
};
