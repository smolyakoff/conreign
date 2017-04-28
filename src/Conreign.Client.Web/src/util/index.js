import { isString, partial } from 'lodash';

export function isNonEmptyString(x) {
  return isString(x) && x.length > 0;
}

export function composeReducers(...reducers) {
  return (state, action) => reducers.reduce(
    (intermediateState, reducer) => reducer(intermediateState, action),
    state,
  );
}

// Conditional observable operators
function applyOperatorIf(operator, predicate, action, ...params) {
  return this[operator](
    value => predicate(value)
      ? action(value, ...params)
      : value,
  );
}
export const mapIf = partial(applyOperatorIf, 'map');
export const doIf = partial(applyOperatorIf, 'do');
