import { includes, mean } from 'lodash';
import { combineEpics } from 'redux-observable';

import Rx from './../../rx';
import {
  RoomMode,
  GAME_STARTED,
  GAME_TICKED,
  TURN_CALCULATION_STARTED,
  TURN_CALCULATION_ENDED,
} from './../../api';
import {
  mapEventNameToActionType,
} from './../../framework';

const AVERAGE_SERVER_TICK_INTERVAL = 5000;
const SERVER_TICK_RESOLUTION = 5;
const AVERAGE_CLIENT_TICK_INTERVAL = 1000;
const TICK_INTERVAL_EPSILON = 200;

const HANDLE_GAME_STARTED = mapEventNameToActionType(GAME_STARTED);
const HANDLE_GAME_TICKED = mapEventNameToActionType(GAME_TICKED);
const HANDLE_TURN_CALCULATION_STARTED = mapEventNameToActionType(TURN_CALCULATION_STARTED);
const HANDLE_TURN_CALCULATION_ENDED = mapEventNameToActionType(TURN_CALCULATION_ENDED);
const SET_TURN_TIMER_SECONDS = 'SET_TURN_TIMER_SECONDS';

function reducer(state, action) {
  if (state.mode !== RoomMode.Game) {
    return state;
  }
  switch (action.type) {
    case HANDLE_TURN_CALCULATION_STARTED:
      return {
        ...state,
        isCalculating: true,
      };
    case HANDLE_TURN_CALCULATION_ENDED: {
      const { map, turn, movingFleets } = action.payload;
      return {
        ...state,
        map,
        turn: turn + 1,
        movingFleets,
        isCalculating: false,
      };
    }
    case SET_TURN_TIMER_SECONDS:
      return {
        ...state,
        turnSeconds: action.payload,
      };
    default:
      return state;
  }
}

const TICK_ACTIONS = [
  HANDLE_GAME_STARTED,
  HANDLE_GAME_TICKED,
  HANDLE_TURN_CALCULATION_STARTED,
  HANDLE_TURN_CALCULATION_ENDED,
];

function isTickAction(action) {
  return includes(TICK_ACTIONS, action.type);
}

function mapTickActionToEvent(action) {
  switch (action.type) {
    case HANDLE_TURN_CALCULATION_ENDED:
      return { tick: 0, type: action.type };
    case HANDLE_GAME_STARTED:
      return { tick: 0, type: action.type };
    case HANDLE_GAME_TICKED:
      return {
        tick: action.payload.tick,
        type: action.type,
      };
    case HANDLE_TURN_CALCULATION_STARTED:
      return {
        type: action.type,
        tick: null,
      };
    default:
      throw new Error(`Unexpected action type: ${action.type}`);
  }
}

function createEpic() {
  function turnTimerEpic(action$) {
    const serverTickEvents$ = action$
      .filter(isTickAction)
      .map(mapTickActionToEvent);

    const serverTicks$ = serverTickEvents$
      .map(({ tick }) => tick);

    const serverTickIntervals$ = serverTickEvents$
      .timeInterval()
      .skip(1)
      .filter(({ value }) => value.type !== HANDLE_TURN_CALCULATION_ENDED)
      .map(({ interval }) => interval);

    const avgServerTickIntervals$ = serverTickIntervals$
      .bufferCount(4, 3)
      .map(mean)
      .startWith(AVERAGE_SERVER_TICK_INTERVAL);

    const clientTickIntervals$ = avgServerTickIntervals$
      .map(x => x / SERVER_TICK_RESOLUTION)
      .filter(x => Math.abs(AVERAGE_CLIENT_TICK_INTERVAL - x) < TICK_INTERVAL_EPSILON);

    const clientTicks$ = Rx.Observable
      .combineLatest(serverTicks$, clientTickIntervals$)
      .switchMap(([tick, interval]) => {
        if (tick === null) {
          return Rx.Observable.of(null);
        }
        const subTick$ = Rx.Observable.timer(0, interval)
          .take(SERVER_TICK_RESOLUTION + 1)
          .map(index => index === SERVER_TICK_RESOLUTION
            ? null
            : (tick * SERVER_TICK_RESOLUTION) + index,
          );
        return subTick$;
      })
      .sampleTime(AVERAGE_CLIENT_TICK_INTERVAL / 2);

    return clientTicks$
      .map(tick => ({
        type: SET_TURN_TIMER_SECONDS,
        payload: tick,
        meta: {
          $hideFromDevTools: true,
        },
      }));
  }

  return combineEpics(
    turnTimerEpic,
  );
}

export default {
  reducer,
  createEpic,
};
