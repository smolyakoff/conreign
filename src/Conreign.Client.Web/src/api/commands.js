export const LOGIN = 'LOGIN';
export const JOIN_ROOM = 'JOIN_ROOM';
export const START_GAME = 'START_GAME';
export const UPDATE_GAME_OPTIONS = 'UPDATE_GAME_OPTIONS';
export const UPDATE_PLAYER_OPTIONS = 'UPDATE_PLAYER_OPTIONS';
export const GET_ROOM_STATE = 'GET_ROOM_STATE';
export const SEND_MESSAGE = 'SEND_MESSAGE';
export const LAUNCH_FLEET = 'LAUNCH_FLEET';
export const CANCEL_FLEET = 'CANCEL_FLEET';
export const END_TURN = 'END_TURN';

export function login(payload) {
  return {
    type: LOGIN,
    payload,
  };
}

export function getRoomState(payload) {
  return {
    type: GET_ROOM_STATE,
    payload,
  };
}

export function joinRoom(payload) {
  return {
    type: JOIN_ROOM,
    payload,
  };
}

export function updateGameOptions(payload) {
  return {
    type: UPDATE_GAME_OPTIONS,
    payload,
  };
}

export function updatePlayerOptions(payload) {
  return {
    type: UPDATE_PLAYER_OPTIONS,
    payload,
  };
}

export function startGame(payload) {
  return {
    type: START_GAME,
    payload,
  };
}

export function sendMessage(payload) {
  return {
    type: SEND_MESSAGE,
    payload,
  };
}

export function launchFleet(payload) {
  return {
    type: LAUNCH_FLEET,
    payload,
  };
}

export function cancelFleet(payload) {
  return {
    type: CANCEL_FLEET,
    payload,
  };
}

export function endTurn(payload) {
  return {
    type: END_TURN,
    payload,
  };
}
