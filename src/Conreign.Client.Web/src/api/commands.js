export const LOGIN = 'LOGIN';
export const JOIN_ROOM = 'JOIN_ROOM';
export const START_GAME = 'START_GAME';
export const UPDATE_GAME_OPTIONS = 'UPDATE_GAME_OPTIONS';
export const UPDATE_PLAYER_OPTIONS = 'UPDATE_PLAYER_OPTIONS';
export const GET_ROOM_STATE = 'GET_ROOM_STATE';
export const SEND_MESSAGE = 'SEND_MESSAGE';

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
