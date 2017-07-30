import { includes, mean, groupBy, mapValues, find, values } from 'lodash';
import { createSelector } from 'reselect';
import { combineEpics } from 'redux-observable';

import Rx from './../../rx';
import {
  RoomMode,
  TurnStatus,
  GAME_STARTED,
  GAME_TICKED,
  TURN_CALCULATION_STARTED,
  TURN_CALCULATION_ENDED,
  ATTACK_HAPPENED,
  LAUNCH_FLEET,
  CANCEL_FLEET,
  END_TURN,
  TURN_ENDED,
} from './../../api';
import {
  mapEventNameToActionType,
  getEventNameFromAction,
  createAsyncActionTypes,
  getOriginalPayload,
  getAsyncOperationCorrelationId,
  createSucceededAsyncActionType,
  AsyncOperationState,
} from './../../framework';
import { count } from './../../util';
import { getDistance, SET_MAP_SELECTION } from './../map';

const AVERAGE_SERVER_TICK_INTERVAL = 5000;
const SERVER_TICK_RESOLUTION = 5;
const AVERAGE_CLIENT_TICK_INTERVAL = 1000;
const TICK_INTERVAL_EPSILON = 200;

const HANDLE_GAME_STARTED = mapEventNameToActionType(GAME_STARTED);
const HANDLE_GAME_TICKED = mapEventNameToActionType(GAME_TICKED);
const HANDLE_TURN_CALCULATION_STARTED = mapEventNameToActionType(TURN_CALCULATION_STARTED);
const HANDLE_TURN_CALCULATION_ENDED = mapEventNameToActionType(TURN_CALCULATION_ENDED);
const HANDLE_ATTACK_HAPPENED = mapEventNameToActionType(ATTACK_HAPPENED);
const HANDLE_TURN_ENDED = mapEventNameToActionType(TURN_ENDED);
const SET_TURN_TIMER_SECONDS = 'SET_TURN_TIMER_SECONDS';
const CHANGE_FLEET = 'CHANGE_FLEET';
const SELECT_FLEET = 'SELECT_FLEET';
const {
  [AsyncOperationState.Pending]: LAUNCH_FLEET_PENDING,
  [AsyncOperationState.Failed]: LAUNCH_FLEET_FAILED,
} = createAsyncActionTypes(LAUNCH_FLEET);
const CANCEL_FLEET_SUCCEEDED = createSucceededAsyncActionType(CANCEL_FLEET);

export function changeFleet(payload) {
  return {
    type: CHANGE_FLEET,
    payload,
  };
}

export function selectFleet(payload) {
  return {
    type: SELECT_FLEET,
    payload,
  };
}

function reducer(state, action) {
  if (state.mode !== RoomMode.Game) {
    return state;
  }
  const { payload, type } = action;
  switch (type) {
    case SET_MAP_SELECTION: {
      if (state.fleetShips !== 0) {
        return state;
      }
      const sourcePlanet = state.map.planets[payload.start];
      if (sourcePlanet.ships === 0) {
        return state;
      }
      return {
        ...state,
        fleetShips: sourcePlanet.ships,
      };
    }
    case CHANGE_FLEET: {
      return {
        ...state,
        fleetShips: payload.ships,
      };
    }
    case SELECT_FLEET: {
      return {
        ...state,
        selectedFleetIndex: payload.index,
      };
    }
    case LAUNCH_FLEET_PENDING: {
      const fleet = payload.fleet;
      const planets = state.map.planets;
      const sourcePlanet = planets[fleet.from];
      const shipsLeft = sourcePlanet.ships - fleet.ships;
      return {
        ...state,
        map: {
          ...state.map,
          planets: {
            ...planets,
            [fleet.from]: {
              ...sourcePlanet,
              ships: shipsLeft,
            },
          },
        },
        waitingFleets: [
          ...state.waitingFleets,
          {
            id: getAsyncOperationCorrelationId(action),
            ...payload.fleet,
          },
        ],
        fleetShips: fleet.ships > shipsLeft ? shipsLeft : fleet.ships,
      };
    }
    case LAUNCH_FLEET_FAILED: {
      const id = getAsyncOperationCorrelationId(action);
      const { fleet } = getOriginalPayload(action);
      const planets = state.map.planets;
      const sourcePlanet = planets[fleet.from];
      return {
        ...state,
        waitingFleets: state.waitingFleets.filter(x => x.id !== id),
        map: {
          ...state.map,
          planets: {
            ...planets,
            [fleet.from]: {
              ...sourcePlanet,
              ships: sourcePlanet.ships + fleet.ships,
            },
          },
        },
      };
    }
    case CANCEL_FLEET_SUCCEEDED: {
      const { index } = getOriginalPayload(action);
      const { waitingFleets, map } = state;
      const fleet = waitingFleets[index];
      const planets = map.planets;
      const sourcePlanet = planets[fleet.from];
      return {
        ...state,
        map: {
          ...state.map,
          planets: {
            ...planets,
            [fleet.from]: {
              ...sourcePlanet,
              ships: sourcePlanet.ships + fleet.ships,
            },
          },
        },
        waitingFleets: [
          ...waitingFleets.slice(0, index),
          ...waitingFleets.slice(index + 1),
        ],
        selectedFleetIndex: null,
      };
    }
    case HANDLE_TURN_CALCULATION_STARTED:
      return {
        ...state,
        isCalculating: true,
      };
    case HANDLE_TURN_CALCULATION_ENDED: {
      const { map, turn, movingFleets } = action.payload;
      const { turnStatuses } = state;
      return {
        ...state,
        map,
        turn: turn + 1,
        movingFleets,
        waitingFleets: [],
        turnStatuses: mapValues(turnStatuses, () => TurnStatus.Thinking),
        selectedFleetIndex: null,
        isCalculating: false,
      };
    }
    case HANDLE_ATTACK_HAPPENED: {
      const {
        timestamp,
        outcome,
        attackerUserId,
        defenderUserId,
        planetName,
      } = payload;
      const planet = find(state.map.planets, p => p.name === planetName);
      return {
        ...state,
        events: [
          ...state.events,
          {
            type: getEventNameFromAction(action),
            payload: {
              timestamp,
              outcome,
              planet,
              attacker: state.players[attackerUserId],
              defender: defenderUserId ? state.players[defenderUserId] : null,
            },
          },
        ],
      };
    }
    case SET_TURN_TIMER_SECONDS:
      return {
        ...state,
        turnSeconds: action.payload,
      };
    case HANDLE_TURN_ENDED:
      return {
        ...state,
        turnStatuses: {
          ...state.turnStatuses,
          [payload.userId]: TurnStatus.Ended,
        },
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

function launchFleetEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(LAUNCH_FLEET)
    .mergeMap(apiDispatcher);
}

function cancelFleetEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(CANCEL_FLEET)
    .mergeMap(apiDispatcher);
}

function endTurnEpic(action$, store, { apiDispatcher }) {
  return action$
    .ofType(END_TURN)
    .mergeMap(apiDispatcher);
}

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

const epic = combineEpics(
  launchFleetEpic,
  cancelFleetEpic,
  endTurnEpic,
  turnTimerEpic,
);

const selectWaitingFleets = state => state.waitingFleets;
const selectMap = state => state.map;
const selectMovingFleets = state => state.movingFleets;
const selectPlayers = state => state.players;
const selectPlanets = state => state.map.planets;
const selectDeadPlayerIds = state => state.deadPlayers;
const selectTurnStatuses = state => state.turnStatuses;

const selectWaitingFleetsWithDetails = createSelector(
  selectWaitingFleets,
  selectMap,
  (waitingFleets, map) => waitingFleets.map(fleet => ({
    id: fleet.id,
    from: map.planets[fleet.from],
    to: map.planets[fleet.to],
    ships: fleet.ships,
    distance: getDistance(fleet.from, fleet.to, map.width),
  })),
);

const selectMovingFleetsByPosition = createSelector(
  selectMovingFleets,
  movingFleets => mapValues(
    groupBy(movingFleets, fleetLocation => fleetLocation.position),
    locations => locations.map(l => l.fleet),
  ),
);

const selectPlayersWithStatistics = createSelector(
  selectPlayers,
  selectPlanets,
  selectDeadPlayerIds,
  selectTurnStatuses,
  (players, planets, deadPlayerIds, turnStatuses) => mapValues(players, player => ({
    ...player,
    planetsCount: count(values(planets), planet => planet.ownerId === player.userId),
    isDead: includes(deadPlayerIds, player.userId),
    turnStatus: turnStatuses[player.userId],
  })),
);

export const selectGame = createSelector(
  selectPlayersWithStatistics,
  selectMovingFleetsByPosition,
  selectWaitingFleetsWithDetails,
  room => room,
  (players, movingFleets, waitingFleets, room) => ({
    ...room,
    movingFleets,
    waitingFleets,
    players,
  }),
);

export { launchFleet, cancelFleet, endTurn } from './../../api';

export default {
  reducer,
  epic,
};
