export const SET_MAP_SELECTION = 'SET_MAP_SELECTION';

export function setMapSelection(payload) {
  return {
    type: SET_MAP_SELECTION,
    payload,
  };
}
