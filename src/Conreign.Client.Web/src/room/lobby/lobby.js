import { combineEpics } from 'redux-observable';
import { values } from 'lodash';
import {
  createAsyncActionTypes,
  AsyncOperationState,
  PresenceStatus,
  GameEventType,
} from './../../core';

const START_GAME = 'START_GAME';
const SUBMIT_GAME_SETTINGS = 'UPDATE_GAME_OPTIONS';
const CHANGE_GAME_SETTINGS = 'CHANGE_GAME_SETTINGS';
const CHANGE_PLAYER_SETTINGS = 'CHANGE_PLAYER_SETTINGS';
const SUBMIT_PLAYER_SETTINGS = 'UPDATE_PLAYER_OPTIONS';
const HANDLE_PLAYER_UPDATED = 'HANDLE_PLAYER_UPDATED';
const HANDLE_PLAYER_JOINED = 'HANDLE_PLAYER_JOINED';
const SET_PLAYER_SETTINGS_VISIBILITY = 'SET_PLAYER_SETTINGS_VISIBILITY';
const {
  [AsyncOperationState.Succeeded]: SUBMIT_PLAYER_SETTINGS_SUCCEEDED,
} = createAsyncActionTypes(SUBMIT_PLAYER_SETTINGS);


export function setPlayerSettingsVisibility(payload) {
  return {
    type: SET_PLAYER_SETTINGS_VISIBILITY,
    payload,
  };
}

export function submitGameSettings(payload) {
  return {
    type: SUBMIT_GAME_SETTINGS,
    payload,
  };
}

export function changeGameSettings(payload) {
  return {
    type: CHANGE_GAME_SETTINGS,
    payload,
  };
}

export function changePlayerSettings(payload) {
  return {
    type: CHANGE_PLAYER_SETTINGS,
    payload,
  };
}

export function submitPlayerSettings(payload) {
  return {
    type: SUBMIT_PLAYER_SETTINGS,
    payload,
  };
}

export function startGame(payload) {
  return {
    type: START_GAME,
    payload,
  };
}

function createEpic({ apiDispatcher }) {
  function submitGameSettingsEpic(action$) {
    return action$
      .ofType(SUBMIT_GAME_SETTINGS)
      .map(action => ({
        ...action,
        payload: {
          roomId: action.payload.roomId,
          options: action.payload.settings,
        },
      }))
      .mergeMap(apiDispatcher);
  }

  function submitPlayerSettingsEpic(action$) {
    return action$
      .ofType(SUBMIT_PLAYER_SETTINGS)
      .map(action => ({
        ...action,
        payload: {
          roomId: action.payload.roomId,
          options: action.payload.settings,
        },
      }))
      .mergeMap(apiDispatcher);
  }

  function startGameEpic(action$) {
    return action$
      .ofType(START_GAME)
      .mergeMap(apiDispatcher);
  }

  return combineEpics(
    submitGameSettingsEpic,
    submitPlayerSettingsEpic,
    startGameEpic,
  );
}

function reducer(state, action) {
  if (!state.gameSettings && state.map) {
    state = {
      ...state,
      gameSettings: {
        mapWidth: state.map.width,
        mapHeight: state.map.height,
        neutralPlanetsCount: values(state.map.planets)
          .filter(planet => !planet.ownerId)
          .length,
      },
    };
  }
  switch (action.type) {
    case CHANGE_GAME_SETTINGS:
      return {
        ...state,
        gameSettings: action.payload.settings,
      };
    case CHANGE_PLAYER_SETTINGS:
      return {
        ...state,
        playerSettings: action.payload.settings,
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
            type: GameEventType.PlayerJoined,
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
            type: GameEventType.PlayerUpdated,
            payload: {
              timestamp,
              previousPlayer,
              currentPlayer,
            },
          },
        ],
      };
    }
    case SUBMIT_PLAYER_SETTINGS_SUCCEEDED:
      return {
        ...state,
        playerSettingsOpen: false,
      };
    case SET_PLAYER_SETTINGS_VISIBILITY:
      return {
        ...state,
        playerSettingsOpen: action.payload,
      };
    default:
      return state;
  }
}

export default {
  reducer,
  createEpic,
};
