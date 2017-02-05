import { PropTypes } from 'react';

export const GAME_SETTINGS_SHAPE = PropTypes.shape({
  mapWidth: PropTypes.number.isRequired,
  mapHeight: PropTypes.number.isRequired,
  neutralPlanetsCount: PropTypes.number.isRequired,
});

const SUBMIT_GAME_SETTINGS = 'UPDATE_GAME_OPTIONS';
const CHANGE_GAME_SETTINGS = 'CHANGE_GAME_SETTINGS';

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
  return submitGameSettingsEpic;
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
    default:
      return state;
  }
}

export default {
  reducer,
  createEpic,
};
