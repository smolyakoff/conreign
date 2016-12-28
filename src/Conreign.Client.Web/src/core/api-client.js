// eslint-disable-next-line
import $ from 'expose-loader?jQuery!expose-loader?$!jquery';
import 'ms-signalr-client';
import Rx from 'rxjs/Rx';
import { once, fromPairs, map, startCase } from 'lodash';

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

function apiClient(options) {
  const { baseUrl } = options;

  const subject = Rx.Subject.create();
  let connection = null;
  let state = ApiClientState.Disconnected;
  let hub = null;

  function onStateChanged({ newState }) {
    state = connectionStateCodeMapping[newState];
  }

  function connect() {
    connection = $.hubConnection(baseUrl, { useDefaultPath: false });
    hub = connection.createHubProxy(SERVER_HUB_NAME);
    hub.on('OnNext', subject.onNext);
    connection.stateChanged(onStateChanged);
    return Rx.Observable.fromPromise(connection.start());
  }

  function send(message) {
    return Rx.Observable.fromPromise(hub.invoke('send', message));
  }

  return {
    get state() {
      return state;
    },
    connect: once(connect),
    send,
  };
}

apiClient.state = ApiClientState;

export default apiClient;
