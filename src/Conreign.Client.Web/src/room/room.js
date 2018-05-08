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
import createEventReducer from './event-reducer';
import { composeReducers } from './../util';

const HANDLE_MAP_UPDATED = mapEventNameToActionType(MAP_UPDATED);
const HANDLE_USER_STATUS_CHANGED = mapEventNameToActionType(USER_STATUS_CHANGED);
const HANDLE_LEADER_CHANGED = mapEventNameToActionType(LEADER_CHANGED);
const GET_ROOM_STATE_SUCCEEDED = createSucceededAsyncActionType(GET_ROOM_STATE);

function initializeRoomState(room) {
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

function mapGetRoomStateAction(action) {
  if (action.type !== GET_ROOM_STATE_SUCCEEDED) {
    return action;
  }
  return {
    ...action,
    payload: initializeRoomState(action.payload),
  };
}

function getRoomStateEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(GET_ROOM_STATE)
    .mergeMap(apiDispatcher)
    .map(mapGetRoomStateAction);
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


function roomReducer(state = {}, action) {
  if (action.error) {
    return state;
  }
  switch (action.type) {
    case GET_ROOM_STATE_SUCCEEDED:
      return {
        ...state,
        ...action.payload,
      };
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
    default:
      return state;
  }
}

const roomEventReducer = createEventReducer({
  [CHAT_MESSAGE_RECEIVED]: null,
});

const reducer = composeReducers(
  roomReducer,
  roomEventReducer,
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
