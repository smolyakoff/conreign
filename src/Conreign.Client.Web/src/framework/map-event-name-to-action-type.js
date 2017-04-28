import { snakeCase } from 'lodash';

export default function mapEventNameToActionType(name) {
  return snakeCase(`Handle${name}`).toUpperCase();
}
