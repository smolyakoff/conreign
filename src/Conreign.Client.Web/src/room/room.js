import { omit, findKey, isNumber } from 'lodash';
import fp from 'lodash/fp';
import { combineEpics } from 'redux-observable';

import { AsyncOperationState, createAsyncActionTypes } from './../core';
import { composeReducers } from './../util';
import lobby from './lobby';

const HANDLE_MAP_UPDATED = 'HANDLE_MAP_UPDATED';
const HANDLE_USER_STATUS_CHANGED = 'HANDLE_USER_STATUS_CHANGED';
const HANDLE_LEADER_CHANGED = 'HANDLE_LEADER_CHANGED';
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

function normalizeRoomState(room) {
  const players = fp.flow(
      fp.keyBy(p => p.userId),
      fp.mapValues(p => ({
        ...p,
        status: room.playerStatuses[p.userId],
      })),
    )(room.players);
  return {
    ...omit(room, 'playerStatuses'),
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
      return normalizeRoomState(action.payload);
    case SET_MAP_SELECTION:
      return {
        ...state,
        mapSelection: action.payload.selection,
      };
    case HANDLE_MAP_UPDATED: {
      const {
        map: previousMap,
        mapSelection: previousSelection,
      } = state;
      const currentMap = action.payload.map;
      const currentSelection = moveSelection({
        previousSelection,
        previousMap,
        currentMap,
      });
      return {
        ...state,
        map: currentMap,
        mapSelection: currentSelection,
        gameSettings: null,
      };
    }
    case HANDLE_USER_STATUS_CHANGED: {
      const event = action.payload;
      const player = state.players[event.userId];
      return {
        ...state,
        players: {
          ...state.players,
          [event.userId]: {
            ...player,
            status: event.status,
          },
        },
      };
    }
    case HANDLE_LEADER_CHANGED: {
      const event = action.payload;
      return {
        ...state,
        leaderUserId: event.userId,
      };
    }
    default:
      return state;
  }
}

const reducer = composeReducers(roomReducer, lobby.reducer);
reducer.$key = 'room';


export function selectRoomPage(state, roomId) {
  return {
    ...state.room,
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
