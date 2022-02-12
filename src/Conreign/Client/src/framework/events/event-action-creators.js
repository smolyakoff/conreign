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

export function getEventNameFromAction(action) {
  return action.meta.$event;
}
