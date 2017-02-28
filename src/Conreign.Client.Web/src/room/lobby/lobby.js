import { combineEpics } from 'redux-observable';
import { createAsyncActionTypes, AsyncOperationState, isSucceededAsyncAction } from './../../core';

const SUBMIT_GAME_SETTINGS = 'UPDATE_GAME_OPTIONS';
const CHANGE_GAME_SETTINGS = 'CHANGE_GAME_SETTINGS';
const CHANGE_PLAYER_SETTINGS = 'CHANGE_PLAYER_SETTINGS';
const SUBMIT_PLAYER_SETTINGS = 'UPDATE_PLAYER_OPTIONS';
const HANDLE_PLAYER_UPDATED = 'HANDLE_PLAYER_UPDATED';
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
      .mergeMap(action =>
        apiDispatcher({
          ...action,
          payload: {
            roomId: action.payload.roomId,
            options: action.payload.settings,
          },
        })
        .map((resultAction) => {
          if (isSucceededAsyncAction(resultAction)) {
            return {
              ...resultAction,
              payload: action.payload,
            };
          }
          return resultAction;
        }),
      );
  }
  return combineEpics(
    submitGameSettingsEpic,
    submitPlayerSettingsEpic,
  );
}

function reducer(state, action) {
  switch (action.type) {
    case CHANGE_GAME_SETTINGS: {
      const { roomId, settings } = action.payload;
      const room = state[roomId];
      return {
        ...state,
        [action.payload.roomId]: {
          ...room,
          gameSettings: settings,
        },
      };
    }
    case CHANGE_PLAYER_SETTINGS: {
      const { roomId, settings } = action.payload;
      const room = state[roomId];
      return {
        ...state,
        [roomId]: {
          ...room,
          playerSettings: settings,
        },
      };
    }
    case HANDLE_PLAYER_UPDATED: {
      const { roomId, player } = action.payload;
      const room = state[roomId];
      const currentPlayer = room.players[player.userId];
      return {
        ...state,
        [roomId]: {
          ...room,
          players: {
            ...room.players,
            [player.userId]: {
              ...currentPlayer,
              ...player,
            },
          },
        },
      };
    }
    case SUBMIT_PLAYER_SETTINGS_SUCCEEDED: {
      const { roomId } = action.payload;
      const room = state[roomId];
      return {
        ...state,
        [roomId]: {
          ...room,
          playerSettingsOpen: false,
        },
      };
    }
    case SET_PLAYER_SETTINGS_VISIBILITY: {
      const { roomId, visible } = action.payload;
      const room = state[roomId];
      return {
        ...state,
        [roomId]: {
          ...room,
          playerSettingsOpen: visible,
        },
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
