import decodeJwt from 'jwt-decode';

import { createAsyncActionTypes, AsyncOperationState } from './../core';

const LOGIN = 'LOGIN';
export const {
  [AsyncOperationState.Completed]: LOGIN_COMPLETED,
} = createAsyncActionTypes(LOGIN);

export function login() {
  return { type: LOGIN };
}

function createEpic({ apiDispatcher, userStore }) {
  function loginEpic(action$) {
    return action$
      .ofType(LOGIN)
      .mergeMap(apiDispatcher)
      .map((action) => {
        if (action.type !== LOGIN_COMPLETED) {
          return action;
        }
        const { payload } = action;
        const user = decodeJwt(payload.accessToken);
        const userWithToken = {
          user,
          accessToken: payload.accessToken,
        };
        userStore.set(userWithToken);
        return {
          ...action,
          payload: userWithToken,
        };
      });
  }
  return loginEpic;
}

const INITIAL_STATE = {
  accessToken: null,
  user: null,
};

function reducer(state = INITIAL_STATE, action) {
  switch (action.type) {
    case LOGIN_COMPLETED:
      return {
        ...state,
        ...action.payload,
      };
    default:
      return state;
  }
}

export const AUTH_REDUCER_KEY = 'auth';
reducer.$key = AUTH_REDUCER_KEY;

export default {
  createEpic,
  reducer,
};
