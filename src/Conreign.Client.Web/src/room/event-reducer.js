import { pick, has, isFunction, identity } from 'lodash';

import { GET_ROOM_STATE } from './../api';
import {
  isEventAction,
  getEventTypeFromAction,
  createSucceededAsyncActionType,
} from './../framework';

const GET_ROOM_STATE_SUCCEEDED = createSucceededAsyncActionType(GET_ROOM_STATE);

export default function createEventReducer(payloadMappersByEventType) {
  function getMapperForEventTypeOrNull(eventType) {
    if (!has(payloadMappersByEventType, eventType)) {
      return null;
    }
    const mapPayload = payloadMappersByEventType[eventType];
    function mapEvent(event, state) {
      const { type, payload } = event;
      return {
        type,
        payload: {
          ...mapPayload(payload, state),
          ...pick(payload, ['timestamp', 'senderId']),
        },
      };
    }
    return isFunction(mapPayload) ? mapEvent : identity;
  }

  function reduce(state, action) {
    if (action.type === GET_ROOM_STATE_SUCCEEDED) {
      // Here it's assumed that events has been already assigned to state
      // It's very confusing
      // TODO: Consider lifting the state handling into a separate module under room
      const { events } = state;
      return {
        ...state,
        events: events.map((event) => {
          const mapEvent = getMapperForEventTypeOrNull(event.type);
          if (!mapEvent) {
            return event;
          }
          return mapEvent(event, state);
        }),
      };
    }
    if (!isEventAction(action)) {
      return state;
    }
    const eventType = getEventTypeFromAction(action);
    const mapEvent = getMapperForEventTypeOrNull(eventType);
    if (!mapEvent) {
      return state;
    }
    const event = {
      type: eventType,
      payload: action.payload,
    };
    return {
      ...state,
      events: [...state.events, mapEvent(event, state)],
    };
  }
  return reduce;
}

