import { CHAT_MESSAGE_RECEIVED } from './../api';

export default {
  [CHAT_MESSAGE_RECEIVED]: ({ message }) => message.text,
};
