import { combineEpics } from 'redux-observable';
import { values, pick, mapValues, omit, keyBy } from 'lodash';
import { createSelector } from 'reselect';

import {
  createSucceededAsyncActionType,
  mapEventNameToActionType,
} from '../../framework';
import {
  PLAYER_JOINED,
  PLAYER_LIST_CHANGED,
  PLAYER_UPDATED,
  UPDATE_GAME_OPTIONS,
  UPDATE_PLAYER_OPTIONS,
  START_GAME,
  GET_ROOM_STATE,
  GAME_STARTED,
  PlayerType,
  PresenceStatus,
  RoomMode,
  TurnStatus,
} from '../../api';
import { composeReducers, count } from '../../util';
import createEventReducer from '../event-reducer';
import { createWelcomeMessageEvent } from './welcome-message-event';


const CHANGE_GAME_OPTIONS = 'CHANGE_GAME_OPTIONS';
const CHANGE_PLAYER_OPTIONS = 'CHANGE_PLAYER_OPTIONS';
const SET_PLAYER_OPTIONS_VISIBILITY = 'SET_PLAYER_OPTIONS_VISIBILITY';
const HANDLE_PLAYER_UPDATED = mapEventNameToActionType(PLAYER_UPDATED);
const HANDLE_PLAYER_JOINED = mapEventNameToActionType(PLAYER_JOINED);
const HANDLE_GAME_STARTED = mapEventNameToActionType(GAME_STARTED);
const HANDLE_PLAYER_LIST_CHANGED = mapEventNameToActionType(PLAYER_LIST_CHANGED);
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

const selectPlayersMap = state => state.players;
const selectPlayers = createSelector(selectPlayersMap, values);
const selectPlayerByUserId = (state, userId) => state.players[userId];
const selectHumanPlayersCount = createSelector(
  selectPlayers,
  players => count(players, player => player.type === PlayerType.Human),
);

function updateGameOptionsEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(UPDATE_GAME_OPTIONS)
    .mergeMap(apiDispatcher);
}

function updatePlayerOptionsEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(UPDATE_PLAYER_OPTIONS)
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

function initializeEvents(room) {
  const { events, roomId } = room;
  if (selectHumanPlayersCount(room) > 1) {
    return events;
  }
  return [
    createWelcomeMessageEvent(roomId),
    ...events,
  ];
}

function lobbyReducer(state, action) {
  if (state.mode !== RoomMode.Lobby) {
    return state;
  }
  switch (action.type) {
    case GET_ROOM_STATE_SUCCEEDED: {
      const { map, players } = action.payload;
      return {
        ...state,
        gameOptions: {
          mapWidth: map.width,
          mapHeight: map.height,
          neutralPlanetsCount: values(map.planets)
            .filter(planet => !planet.ownerId)
            .length,
          botsCount: values(players).filter(player => player.type === PlayerType.Bot).length,
        },
        events: initializeEvents(state),
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
    case HANDLE_PLAYER_LIST_CHANGED: {
      const { playersLeft, playersJoined } = action.payload;
      const joinedPlayersWithStatus = playersJoined.map(player => ({
        ...player,
        status: PresenceStatus.Online,
      }));
      return {
        ...state,
        players: {
          ...omit(state.players, playersLeft),
          ...keyBy(joinedPlayersWithStatus, player => player.userId),
        },
      };
    }
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
      };
    }
    case HANDLE_PLAYER_UPDATED: {
      const { player: currentPlayer } = action.payload;
      const previousPlayer = selectPlayerByUserId(state, currentPlayer.userId);
      return {
        ...state,
        players: {
          ...state.players,
          [currentPlayer.userId]: {
            ...previousPlayer,
            ...currentPlayer,
          },
        },
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

const lobbyEventReducer = createEventReducer({
  [PLAYER_JOINED]: null,
  [PLAYER_UPDATED]: (payload, state) => {
    const { player: currentPlayer } = payload;
    const previousPlayer = selectPlayerByUserId(state, currentPlayer.userId);
    return {
      currentPlayer,
      previousPlayer,
    };
  },
});

const reducer = composeReducers(
  lobbyEventReducer,
  lobbyReducer,
);

export {
  updateGameOptions,
  updatePlayerOptions,
  startGame,
} from './../../api';

export default {
  reducer,
  epic,
};
