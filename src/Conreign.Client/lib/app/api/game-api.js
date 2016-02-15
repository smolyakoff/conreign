'use strict';
import fetch from 'isomorphic-fetch';

export function getGame({id}) {
    const options = this.options;
    return fetch('http://google.com')
        .then(response => response.text());
}

export default {
    getGame: getGame
}
