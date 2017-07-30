import { combineReducers } from 'redux';

import {
  routeTransitionReducer,
  asyncActionsCounter,
} from './framework';
import auth from './auth';
import errors from './errors';
import notifications from './notifications';
import room from './room';

const reducer = combineReducers({
  pendingActionCount: asyncActionsCounter,
  isRouteLoading: routeTransitionReducer,
  auth: auth.reducer,
  errors: errors.reducer,
  notifications: notifications.reducer,
  room: room.reducer,
});

export default reducer;

export const selectIsRouteLoading = state => state.isRouteLoading;
export const selectPendingActionCount = state => state.pendingActionCount;
export const selectAuth = state => state.auth;
export const selectNotifications = state => state.notifications;
export const selectRoom = state => state.room;
export const selectErrors = state => state.errors;
