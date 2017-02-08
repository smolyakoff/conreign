import { keyBy, findKey, isNumber } from 'lodash';
import { combineEpics } from 'redux-observable';

import { AsyncOperationState, createAsyncActionTypes } from './../core';
import { composeReducers } from './../util';
import lobby from './lobby';

const HANDLE_MAP_UPDATED = 'HANDLE_MAP_UPDATED';
const HANDLE_PLAYER_JOINED = 'HANDLE_PLAYER_JOINED';
const GET_ROOM_STATE = 'GET_ROOM_STATE';
const SET_MAP_SELECTION = 'SET_MAP_SELECTION';
const GET_ROOM_STATE_ACTIONS = createAsyncActionTypes('GET_ROOM_STATE');
const {
  [AsyncOperationState.Succeeded]: GET_ROOM_STATE_COMPLETED,
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

function createEpic(container) {
  const { apiDispatcher } = container;

  function getRoomStateEpic(action$) {
    return action$
      .ofType(GET_ROOM_STATE)
      .mergeMap(apiDispatcher);
  }
  return combineEpics(
    getRoomStateEpic,
    lobby.createEpic(container),
  );
}

function mapRoomDtoToRoomState(room) {
  const players = keyBy(room.players, p => p.userId);
  return {
    ...room,
    players,
  };
}

function moveCoordinate({ previousCoordinate, previousMap, currentMap }) {
  if (!isNumber(previousCoordinate)) {
    return null;
  }
  const planet = previousMap[previousCoordinate];
  return planet ? findKey(currentMap, v => v.name === planet.name) : null;
}

function moveSelection({ previousSelection, previousMap, currentMap }) {
  if (!previousSelection) {
    return { start: null, end: null };
  }
  return {
    start: moveCoordinate({
      previousCoordinate: previousSelection.start,
      previousMap,
      currentMap,
    }),
    end: moveCoordinate({
      previousCoordinate: previousSelection.end,
      previousMap,
      currentMap,
    }),
  };
}

function roomReducer(state = {}, action) {
  if (action.error) {
    return state;
  }
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
    case HANDLE_MAP_UPDATED: {
      const roomId = action.payload.roomId;
      if (!state[roomId]) {
        return state;
      }
      const {
        map: previousMap,
        mapSelection: previousSelection,
      } = state[roomId];
      const currentMap = action.payload.map;
      const currentSelection = moveSelection({
        previousSelection,
        previousMap,
        currentMap,
      });
      const { map } = action.payload;
      return {
        ...state,
        [action.payload.roomId]: {
          ...state[action.payload.roomId],
          map,
          mapSelection: currentSelection,
          gameSettings: null,
        },
      };
    }
    case HANDLE_PLAYER_JOINED: {
      const room = state[action.payload.roomId];
      return {
        ...state,
        [action.payload.roomId]: {
          ...room,
          players: {
            ...room.players,
            [action.payload.player.userId]: action.payload.player,
          },
        },
      };
    }
    default:
      return state;
  }
}

const reducer = composeReducers(roomReducer, lobby.reducer);
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
