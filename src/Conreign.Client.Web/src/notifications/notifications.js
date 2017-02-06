import { uniqueId } from 'lodash';

const ID_PREFIX = 'notification-';
const SHOW_NOTIFICATION = 'SHOW_NOTIFICATION';
const HIDE_NOTIFICATION = 'HIDE_NOTIFICATION';

export function showNotification(payload) {
  return {
    type: SHOW_NOTIFICATION,
    payload: {
      ...payload,
      id: uniqueId(ID_PREFIX),
    },
  };
}

export function hideNotification(payload) {
  return {
    type: HIDE_NOTIFICATION,
    payload,
  };
}

function reducer(state = [], action) {
  switch (action.type) {
    case SHOW_NOTIFICATION:
      return [...state, action.payload];
    case HIDE_NOTIFICATION:
      return state.filter(notification => notification.id !== action.payload.id);
    default:
      return state;
  }
}
reducer.$key = 'notifications';

export function selectNotifications(state) {
  return state[reducer.$key];
}

export default {
  reducer,
};
