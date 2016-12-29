import { CONNECT_TO_SERVER_COMPLETED } from './../main';

const LOGIN = 'LOGIN';
const LOGIN_COMPLETED = 'LOGIN_COMPLETED';

function login() {
  return { type: LOGIN };
}

function loginCompleted(payload) {
  return { type: LOGIN_COMPLETED, payload };
}

function createEpic({ apiClient, userStore }) {
  function loginEpic(action$) {
    return action$.ofType(CONNECT_TO_SERVER_COMPLETED)
      .mapTo(login())
      .mergeMap(message => apiClient.send(message))
      .do(({ payload }) => userStore.set(payload))
      .map(loginCompleted);
  }
  return loginEpic;
}

function reducer(state) {
  return state;
}

export default {
  createEpic,
  reducer,
};
