'use strict';
import isObject from 'lodash/isObject';
import isString from 'lodash/isString';
import isFunction from 'lodash/isFunction';
import isUndefined from 'lodash/isUndefined';
import values from 'lodash/values';
import extend from 'lodash/extend';
import defaults from 'lodash/defaults';
import invert from 'lodash/invert';

function reduceReducers(...reducers) {
    return (previous, current) => reducers.reduce((p, r) => r(p, current), previous);
}

export function handleAction(action, reducers) {
    let reducerKeyToType = {};
    if (isString(action)) {
        reducerKeyToType.success = [action];
    } else if (isFunction(action)) {
        extend(reducerKeyToType, action.$types);
    } else if (isObject(action)) {
        extend(reducerKeyToType, action);
    }
    const types = values(reducerKeyToType);
    if (types.length === 0) {
        throw new Error(`Could not determine action types to handle!
            Pass a string, action creator function or "reducer key - action type" mapping object.`)
    }
    const typeToReducerKey = invert(reducerKeyToType);

    let reducersMap = {};
    if (isFunction(reducers)) {
        reducersMap.success = reducers;
    } else if (isObject(reducers)) {
        extend(reducersMap, reducers);
    }

    return (state, action) => {
        const reducerKey = typeToReducerKey[action.type];
        if (!reducerKey) {
            return state;
        }
        const reducer = reducersMap[reducerKey];
        if (!reducer) {
            return state;
        }
        return reducer(state, action);
    };
}

export function handleActions(reducerPairs, defaultState = {}) {
    const reducers = reducerPairs.map(([key, handler]) => handleAction(key, handler));
    const reducer = reduceReducers(...reducers);

    return isUndefined(defaultState)
        ? reducer
        : (state = defaultState, action) => reducer(state, action);
}