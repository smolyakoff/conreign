import { isString } from 'lodash';

// eslint-disable-next-line import/prefer-default-export
export function isNonEmptyString(value) {
  return isString(value) && value.length > 0;
}
