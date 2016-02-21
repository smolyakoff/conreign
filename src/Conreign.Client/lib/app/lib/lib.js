import $ from 'jquery';
require('./signalr');

export const signalr = $.hubConnection;