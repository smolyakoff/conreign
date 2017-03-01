import React, { PropTypes } from 'react';
import { compose, withHandlers, setPropTypes } from 'recompose';
import { cond, isString, conforms, omit } from 'lodash';

import { Input } from './../../theme';
import './chat-message-input.scss';

const ENTER_KEY_CODE = 13;

function ChatMessageInput({
  onKeyDown,
  ...others
}) {
  return (
    <Input
      className="c-chat-message-input"
      tagName="textarea"
      onKeyDown={onKeyDown}
      rows={2}
      {...omit(others, ['onSend', 'onEdit'])}
    />
  );
}

ChatMessageInput.propTypes = {
  onKeyDown: PropTypes.func.isRequired,
};

const Action = {
  Send: 'Send',
  GoToNextLine: 'GoToNextLine',
  Ignore: 'Ignore',
};

const chooseAction = cond([
  [
    conforms({
      ctrlKey: x => x,
      keyCode: x => x === ENTER_KEY_CODE,
      text: x => isString(x) && x.length > 0,
    }),
    () => Action.GoToNextLine,
  ],
  [
    conforms({
      ctrlKey: x => !x,
      keyCode: x => x === ENTER_KEY_CODE,
      text: x => isString(x) && x.length > 0,
    }),
    () => Action.Send,
  ],
  [
    () => true,
    () => Action.Ignore,
  ],
]);

function handleKeyDown(props, event) {
  const action = chooseAction({
    keyCode: event.keyCode,
    ctrlKey: event.ctrlKey,
    text: props.value,
  });
  switch (action) {
    case Action.Send:
      event.preventDefault();
      props.onSend(props.value, event);
      props.onEdit('');
      break;
    case Action.GoToNextLine:
      props.onEdit(`${props.value}\n`);
      break;
    default:
      break;
  }
}

export default compose(
  withHandlers({
    onKeyDown: props => event => handleKeyDown(props, event),
  }),
  setPropTypes({
    ...ChatMessageInput.propTypes,
    onSend: PropTypes.func.isRequired,
    onEdit: PropTypes.func.isRequired,
  }),
)(ChatMessageInput);
