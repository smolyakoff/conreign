import decodeJwt from 'jwt-decode';
import { combineEpics } from 'redux-observable';

import { createAsyncActionTypes, AsyncOperationState } from './../framework';
import { LOGIN } from './../api';

export const {
  [AsyncOperationState.Succeeded]: LOGIN_SUCCEEDED,
} = createAsyncActionTypes(LOGIN);
const AUTH_REDUCER_KEY = 'auth';

export function login(payload = { force: false }) {
  return {
    type: LOGIN,
    payload,
  };
}

function createEpic({ apiDispatcher, userStore }) {
  function loginEpic(action$, store) {
    return action$
      .ofType(LOGIN)
      .filter((action) => {
        const auth = store.getState()[AUTH_REDUCER_KEY];
        return action.payload.force || !auth.user;
      })
      .map((action) => {
        const user = userStore.get() || {};
        return {
          ...action,
          payload: {
            accessToken: user.accessToken,
          },
        };
      })
      .mergeMap(apiDispatcher)
      .map((action) => {
        if (action.type !== LOGIN_SUCCEEDED) {
          return action;
        }
        const { payload } = action;
        const user = decodeJwt(payload.accessToken);
        const userWithToken = {
          user,
          accessToken: payload.accessToken,
        };
        return {
          ...action,
          payload: userWithToken,
        };
      });
  }

  function persistUserEpic(action$) {
    return action$
      .ofType(LOGIN_SUCCEEDED)
      .do(action => userStore.set(action.payload))
      .ignoreElements();
  }

  return combineEpics(loginEpic, persistUserEpic);
}

const INITIAL_STATE = {
  accessToken: null,
  user: null,
};

function reducer(state = INITIAL_STATE, action) {
  switch (action.type) {
    case LOGIN_SUCCEEDED:
      return {
        ...state,
        ...action.payload,
      };
    default:
      return state;
  }
}

reducer.$key = AUTH_REDUCER_KEY;

export function selectCurrentUser(state) {
  const tokenPayload = state[reducer.$key].user;
  return {
    id: tokenPayload.sub,
  };
}

export default {
  createEpic,
  reducer,
};
