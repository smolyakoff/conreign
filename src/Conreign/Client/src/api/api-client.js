// eslint-disable-next-line
import $ from 'expose-loader?jQuery!expose-loader?$!jquery';
import 'ms-signalr-client';
import Rx from 'rxjs/Rx';
import { once, fromPairs, map, startCase } from 'lodash';

import mapError from './map-error';

const signalRConnectionState = $.signalR.connectionState;

export const ApiClientState = fromPairs(
  map(signalRConnectionState, (v, k) => [startCase(k), k.toUpperCase()]),
);

const connectionStateCodeMapping = {
  [signalRConnectionState.disconnected]: ApiClientState.Disconnected,
  [signalRConnectionState.connecting]: ApiClientState.Connecting,
  [signalRConnectionState.connected]: ApiClientState.Connected,
  [signalRConnectionState.reconnecting]: ApiClientState.Reconnecting,
};

const SERVER_HUB_NAME = 'GameHub';

function apiClient(accessTokenProvider, options) {
  const { baseUrl } = options;
  let connection = null;
  let state = ApiClientState.Disconnected;
  let hub = null;
  const events = new Rx.Subject();

  function onStateChanged({ newState }) {
    state = connectionStateCodeMapping[newState];
  }

  function connect() {
    connection = $.hubConnection(baseUrl, { useDefaultPath: false });
    hub = connection.createHubProxy(SERVER_HUB_NAME);
    hub.on('OnNext', events.next.bind(events));
    hub.on('OnError', events.error.bind(events));
    hub.on('OnComplete', events.complete.bind(events));
    connection.stateChanged(onStateChanged);
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
      .mergeMap(() => hub.invoke('send', packet))
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
      return state;
    },
    get events() {
      return events;
    },
    send,
  };
}

apiClient.state = ApiClientState;

export default apiClient;
