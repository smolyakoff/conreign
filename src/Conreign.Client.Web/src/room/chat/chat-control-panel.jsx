import React from 'react';
import PropTypes from 'prop-types';
import { noop } from 'lodash';
import { withState, withHandlers, compose } from 'recompose';

import gear from 'evil-icons/assets/icons/ei-gear.svg';
import comment from 'evil-icons/assets/icons/ei-comment.svg';

import {
  IconButton,
  Grid,
  GridCell,
  VerticalAlignment,
} from './../../theme';
import ChatMessageInput from './chat-message-input';


function ChatControlPanel({
  className,
  message,
  showSettings,
  onSettingsClick,
  onSendClick,
  onMessageSend,
  onMessageEdit,
}) {
  return (
    <Grid
      className={className}
      gutter
      verticalAlignment={VerticalAlignment.Center}
    >
      {
        showSettings && (
          <GridCell fixedWidth>
            <IconButton iconName={gear} onClick={onSettingsClick} />
          </GridCell>
        )
      }
      <GridCell gutter={!showSettings}>
        <ChatMessageInput
          value={message}
          onSend={onMessageSend}
          onChange={e => onMessageEdit(e.target.value)}
          onEdit={onMessageEdit}
        />
      </GridCell>
      <GridCell fixedWidth>
        <IconButton
          iconName={comment}
          onClick={onSendClick}
        >
          Send
        </IconButton>
      </GridCell>
    </Grid>
  );
}

ChatControlPanel.propTypes = {
  className: PropTypes.string,
  message: PropTypes.string.isRequired,
  showSettings: PropTypes.bool,
  onSettingsClick: PropTypes.func,
  onSendClick: PropTypes.func.isRequired,
  onMessageSend: PropTypes.func,
  onMessageEdit: PropTypes.func.isRequired,
};

ChatControlPanel.defaultProps = {
  className: null,
  showSettings: false,
  onSettingsClick: noop,
  onMessageSend: noop,
};

function send(props, event) {
  if (!props.message) {
    return;
  }
  (props.onMessageSend || noop)(props.message, event);
  props.onMessageEdit('');
}

export default compose(
  withState('message', 'onMessageEdit', ''),
  withHandlers({
    onSendClick: props => event => send(props, event),
  }),
)(ChatControlPanel);
