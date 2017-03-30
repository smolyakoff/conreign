import { omit, findKey, isNumber, parseInt } from 'lodash';
import fp from 'lodash/fp';
import { combineEpics } from 'redux-observable';

import { AsyncOperationState, createAsyncActionTypes, GameEventType } from './../core';
import { composeReducers } from './../util';
import lobby from './lobby';
import game from './game';

const HANDLE_MAP_UPDATED = 'HANDLE_MAP_UPDATED';
const HANDLE_USER_STATUS_CHANGED = 'HANDLE_USER_STATUS_CHANGED';
const HANDLE_LEADER_CHANGED = 'HANDLE_LEADER_CHANGED';
const HANDLE_CHAT_MESSAGE_RECEIVED = 'HANDLE_CHAT_MESSAGE_RECEIVED';
const GET_ROOM_STATE = 'GET_ROOM_STATE';
const SET_MAP_SELECTION = 'SET_MAP_SELECTION';
const SEND_MESSAGE = 'SEND_MESSAGE';

const GET_ROOM_STATE_ACTIONS = createAsyncActionTypes('GET_ROOM_STATE');
const {
  [AsyncOperationState.Succeeded]: GET_ROOM_STATE_SUCCEEDED,
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

export function sendMessage(payload) {
  return {
    type: SEND_MESSAGE,
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

  function sendMessageEpic(action$) {
    return action$
      .ofType(SEND_MESSAGE)
      .mergeMap(apiDispatcher);
  }

  return combineEpics(
    getRoomStateEpic,
    sendMessageEpic,
    lobby.createEpic(container),
    game.createEpic(container),
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
    case GET_ROOM_STATE_SUCCEEDED:
      return normalizeRoomState(action.payload);
    case SET_MAP_SELECTION:
      return {
        ...state,
        mapSelection: action.payload,
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
    case HANDLE_CHAT_MESSAGE_RECEIVED:
      return {
        ...state,
        events: [...state.events, {
          type: GameEventType.ChatMessageReceived,
          payload: action.payload,
        }],
      };
    default:
      return state;
  }
}

const reducer = composeReducers(
  roomReducer,
  lobby.reducer,
  game.reducer,
);
reducer.$key = 'room';

function dehydrateEvent(event) {
  const payload = event.payload;
  return {
    ...event,
    payload: {
      ...payload,
      timestamp: new Date(payload.timestamp),
    },
  };
}

function ensureMapSelection(map, mapSelection, currentUser) {
  if (isNumber(mapSelection.start)) {
    return mapSelection;
  }
  const currentUserPlanetPosition = findKey(
    map.planets,
    p => p.ownerId === currentUser.id,
  );
  return { start: parseInt(currentUserPlanetPosition) };
}

export function selectRoomPage(state, roomId) {
  const room = state[reducer.$key];
  const currentUser = {
    id: state.auth.user.sub,
  };
  return {
    ...room,
    mapSelection: ensureMapSelection(
      room.map,
      room.mapSelection || {},
      currentUser,
    ),
    events: room.events.map(dehydrateEvent),
    roomId,
    currentUser,
  };
}

export default {
  createEpic,
  reducer,
};
