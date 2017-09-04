import { combineEpics } from 'redux-observable';
import { omit } from 'lodash';
import fp from 'lodash/fp';

import {
  createSucceededAsyncActionType,
  mapEventNameToActionType,
} from './../framework';

import {
  MAP_UPDATED,
  USER_STATUS_CHANGED,
  LEADER_CHANGED,
  CHAT_MESSAGE_RECEIVED,
  GET_ROOM_STATE,
  SEND_MESSAGE,
} from './../api';
import {
  resetMapSelection,
  ensureMapSelection,
  SET_MAP_SELECTION,
} from './map';
import lobby from './lobby';
import game from './game';
import { composeReducers } from './../util';

const HANDLE_MAP_UPDATED = mapEventNameToActionType(MAP_UPDATED);
const HANDLE_USER_STATUS_CHANGED = mapEventNameToActionType(USER_STATUS_CHANGED);
const HANDLE_LEADER_CHANGED = mapEventNameToActionType(LEADER_CHANGED);
const HANDLE_CHAT_MESSAGE_RECEIVED = mapEventNameToActionType(CHAT_MESSAGE_RECEIVED);
const GET_ROOM_STATE_SUCCEEDED = createSucceededAsyncActionType(GET_ROOM_STATE);

function getRoomStateEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(GET_ROOM_STATE)
    .mergeMap(apiDispatcher);
}

function sendMessageEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(SEND_MESSAGE)
    .mergeMap(apiDispatcher);
}

const epic = combineEpics(
  getRoomStateEpic,
  sendMessageEpic,
  lobby.epic,
  game.epic,
);

function normalizeRoomState(room) {
  const players = fp.flow(
    fp.keyBy(p => p.userId),
    fp.mapValues(p => ({
      ...p,
      status: room.presenceStatuses[p.userId],
    })),
  )(room.players);
  return {
    ...omit(room, 'presenceStatuses'),
    players,
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
      const currentSelection = resetMapSelection(
        previousSelection,
        previousMap,
        currentMap,
      );
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
          type: CHAT_MESSAGE_RECEIVED,
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

export function selectRoomOfUser(room, currentUser) {
  const mapSelection = ensureMapSelection(
    room.mapSelection || {},
    room.map.planets,
    currentUser,
  );
  return {
    ...room,
    mapSelection,
    currentUser,
  };
}

export { RoomMode, sendMessage, getRoomState } from './../api';
export { setMapSelection } from './map';

export default {
  epic,
  reducer,
};
