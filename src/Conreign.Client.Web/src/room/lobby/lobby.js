import { combineEpics } from 'redux-observable';
import { createAsyncActionTypes, AsyncOperationState, PresenceStatus } from './../../core';

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
  return combineEpics(
    submitGameSettingsEpic,
    submitPlayerSettingsEpic,
  );
}

function reducer(state, action) {
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
      };
    }
    case HANDLE_PLAYER_UPDATED: {
      const { player } = action.payload;
      const currentPlayer = state.players[player.userId];
      return {
        ...state,
        players: {
          ...state.players,
          [player.userId]: {
            ...currentPlayer,
            ...player,
          },
        },
      };
    }
    case SUBMIT_PLAYER_SETTINGS_SUCCEEDED:
      return {
        ...state,
        playerSettingsOpen: false,
      };
    case SET_PLAYER_SETTINGS_VISIBILITY: {
      const { visible } = action.payload;
      return {
        ...state,
        playerSettingsOpen: visible,
      };
    }
    default:
      return state;
  }
}

export default {
  reducer,
  createEpic,
};
