import { isNil, isString, findKey, kebabCase } from 'lodash';

export default function mapEnumValueToModifierValue(value, enumObject) {
  if (isNil(value)) {
    return false;
  }
  if (isString(value)) {
    return kebabCase(value);
  }
  const key = findKey(enumObject, v => v === value);
  return isString(key) ? kebabCase(key) : false;
}
