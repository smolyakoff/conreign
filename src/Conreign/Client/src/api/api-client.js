// eslint-disable-next-line
import { HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'
import Rx from 'rxjs/Rx';
import { once, fromPairs, map, startCase } from 'lodash';

import mapError from './map-error';

export const ApiClientState = fromPairs(
  map(HubConnectionState, (v, k) => [startCase(k), k.toUpperCase()]),
);

function apiClient(accessTokenProvider, options) {
  const { baseUrl } = options;
  let connection = null;
  const events = new Rx.Subject();

  function connect() {
    const connectionBuilder = new HubConnectionBuilder()
      .withAutomaticReconnect()
      .withUrl(baseUrl);
    connection = connectionBuilder.build();
    connection.on('OnNext', events.next.bind(events));
    connection.on('OnError', events.error.bind(events));
    connection.on('OnComplete', events.complete.bind(events));
    return Rx.Observable.fromPromise(connection.start());
  }

  const connectOnce = once(connect);

  function send(message) {
    const packet = {
      type: startCase(message.type.toLowerCase()).replace(/\s/g, ''),
      payload: message.payload,
      meta: {
        ...message.meta,
        accessToken: accessTokenProvider(),
      },
    };
    return connectOnce()
      .mergeMap(() => connection.invoke('send', packet))
      .map(response => ({
        ...response,
        meta: {
          ...packet.meta,
          ...response.meta,
        },
      }))
      .catch((error) => {
        const finalError = mapError(error);
        throw finalError;
      });
  }

  return {
    get state() {
      return connection.state;
    },
    get events() {
      return events;
    },
    send,
  };
}

apiClient.state = ApiClientState;

export default apiClient;
