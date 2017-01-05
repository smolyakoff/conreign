const LOGIN = 'LOGIN';

function login() {
  return { type: LOGIN };
}

function createEpic({ apiDispatcher, userStore }) {
  function loginEpic(action$) {
    return action$
      .first()
      .mapTo(login())
      .mergeMap(apiDispatcher)
      .do(({ payload }) => userStore.set(payload));
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
