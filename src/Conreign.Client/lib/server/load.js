'use strict';
import _ from 'lodash';
import Promise from 'bluebird';

function load(renderProps, store) {
    const componentsToResolve = renderProps.components.filter(c => _.isFunction(c.resolve));
    const props = {
        location: renderProps.location,
        params: renderProps.params,
        dispatch: store.dispatch.bind(store)
    };
    const results = componentsToResolve.map(c => c.resolve(props));
    return Promise.all(results);
}

module.exports = load;