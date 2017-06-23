import { isString } from 'lodash';

export default function isNonEmptyString(x) {
  return isString(x) && x.length > 0;
}
