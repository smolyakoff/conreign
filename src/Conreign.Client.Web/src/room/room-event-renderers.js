import { GameEventType } from './../core';

export default {
  [GameEventType.ChatMessageReceived]: ({ message }) => message.text,
};
