'use strict';
import extend from 'lodash/extend';
import gameApi from './game-api';


export class ApiClient {
    constructor(options) {
        this._options = options;
    }
}
extend(ApiClient.prototype, gameApi);

export default ApiClient;