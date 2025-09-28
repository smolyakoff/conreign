export default function composeReducers(...reducers) {
  return (state, action) => reducers.reduce(
    (intermediateState, reducer) => reducer(intermediateState, action),
    state,
  );
}
