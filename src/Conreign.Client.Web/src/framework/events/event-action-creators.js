import { get, isString } from 'lodash';

import mapEventNameToActionType from './map-event-name-to-action-type';

export function createEventAction(event) {
  return {
    ...event,
    type: mapEventNameToActionType(event.type),
    meta: {
      ...event.meta,
      $event: event.type,
    },
  };
}

export function isEventAction(action) {
  const eventName = get(action, 'meta.$event');
  return isString(eventName);
}

export function getEventTypeFromAction(action) {
  return action.meta.$event;
}
