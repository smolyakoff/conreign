import { combineEpics } from 'redux-observable';
import { values, pick, mapValues } from 'lodash';
import {
  createSucceededAsyncActionType,
  mapEventNameToActionType,
  getEventNameFromAction,
} from '../../framework';
import {
  PLAYER_JOINED,
  PLAYER_UPDATED,
  UPDATE_GAME_OPTIONS,
  UPDATE_PLAYER_OPTIONS,
  START_GAME,
  GET_ROOM_STATE,
  GAME_STARTED,
  PresenceStatus,
  RoomMode,
  TurnStatus,
} from '../../api';

const CHANGE_GAME_OPTIONS = 'CHANGE_GAME_OPTIONS';
const CHANGE_PLAYER_OPTIONS = 'CHANGE_PLAYER_OPTIONS';
const SET_PLAYER_OPTIONS_VISIBILITY = 'SET_PLAYER_OPTIONS_VISIBILITY';
const HANDLE_PLAYER_UPDATED = mapEventNameToActionType(PLAYER_UPDATED);
const HANDLE_PLAYER_JOINED = mapEventNameToActionType(PLAYER_JOINED);
const HANDLE_GAME_STARTED = mapEventNameToActionType(GAME_STARTED);
const UPDATE_PLAYER_OPTIONS_SUCCEEDED = createSucceededAsyncActionType(UPDATE_PLAYER_OPTIONS);
const GET_ROOM_STATE_SUCCEEDED = createSucceededAsyncActionType(GET_ROOM_STATE);


export function setPlayerOptionsVisibility(payload) {
  return {
    type: SET_PLAYER_OPTIONS_VISIBILITY,
    payload,
  };
}


export function changeGameOptions(payload) {
  return {
    type: CHANGE_GAME_OPTIONS,
    payload,
  };
}

export function changePlayerOptions(payload) {
  return {
    type: CHANGE_PLAYER_OPTIONS,
    payload,
  };
}

function updateGameOptionsEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(UPDATE_GAME_OPTIONS)
    .map(action => ({
      ...action,
      payload: {
        roomId: action.payload.roomId,
        options: action.payload.options,
      },
    }))
    .mergeMap(apiDispatcher);
}

function updatePlayerOptionsEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(UPDATE_PLAYER_OPTIONS)
    .map(action => ({
      ...action,
      payload: {
        roomId: action.payload.roomId,
        options: action.payload.options,
      },
    }))
    .mergeMap(apiDispatcher);
}

function startGameEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(START_GAME)
    .mergeMap(apiDispatcher);
}

const epic = combineEpics(
  updateGameOptionsEpic,
  updatePlayerOptionsEpic,
  startGameEpic,
);

function reducer(state, action) {
  if (state.mode !== RoomMode.Lobby) {
    return state;
  }
  switch (action.type) {
    case GET_ROOM_STATE_SUCCEEDED: {
      const map = action.payload.map;
      return {
        ...state,
        gameOptions: {
          mapWidth: map.width,
          mapHeight: map.height,
          neutralPlanetsCount: values(map.planets)
            .filter(planet => !planet.ownerId)
            .length,
        },
      };
    }
    case CHANGE_GAME_OPTIONS:
      return {
        ...state,
        gameOptions: action.payload.options,
      };
    case CHANGE_PLAYER_OPTIONS:
      return {
        ...state,
        playerOptions: action.payload.options,
      };
    case HANDLE_PLAYER_JOINED: {
      const { player } = action.payload;
      return {
        ...state,
        players: {
          ...state.players,
          [player.userId]: {
            ...player,
            status: PresenceStatus.Online,
          },
        },
        events: [
          ...state.events,
          {
            type: action.meta.$event,
            payload: action.payload,
          },
        ],
      };
    }
    case HANDLE_PLAYER_UPDATED: {
      const { player: currentPlayer, timestamp } = action.payload;
      const previousPlayer = state.players[currentPlayer.userId];
      return {
        ...state,
        players: {
          ...state.players,
          [currentPlayer.userId]: {
            ...previousPlayer,
            ...currentPlayer,
          },
        },
        events: [
          ...state.events,
          {
            type: getEventNameFromAction(action),
            payload: {
              timestamp,
              previousPlayer,
              currentPlayer,
            },
          },
        ],
      };
    }
    case UPDATE_PLAYER_OPTIONS_SUCCEEDED:
      return {
        ...state,
        playerOptionsOpen: false,
      };
    case SET_PLAYER_OPTIONS_VISIBILITY:
      return {
        ...state,
        playerOptionsOpen: action.payload,
      };
    case HANDLE_GAME_STARTED:
      return {
        ...pick(state, [
          'roomId',
          'players',
          'map',
          'leaderUserId',
          'presenceStatuses',
        ]),
        events: [],
        mode: RoomMode.Game,
        deadPlayers: [],
        turn: 0,
        turnSeconds: null,
        waitingFleets: [],
        movingFleets: [],
        turnStatus: TurnStatus.Thinking,
        turnStatuses: mapValues(state.players, () => TurnStatus.Thinking),
      };
    default:
      return state;
  }
}

export {
  updateGameOptions,
  updatePlayerOptions,
  startGame,
} from './../../api';

export default {
  reducer,
  epic,
};
