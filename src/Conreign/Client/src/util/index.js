import { partial } from 'lodash';

export { default as composeReducers } from './compose-reducers';
export { default as isNonEmptyString } from './is-non-empty-string';
export { default as count } from './count';

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
