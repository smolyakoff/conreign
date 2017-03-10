import React, { PropTypes } from 'react';

import { PanelContainer, Panel } from './../../theme';
import { GAME_EVENT_SHAPE, PLAYER_SHAPE } from '../room-schemas';
import ChatMessage from './chat-message';

export default function ChatMessageArea({
  events,
  renderers,
  players,
}) {
  return (
    <PanelContainer className="u-full-height">
      <Panel className="u-letter-box--small">
        {
          events.map((event, idx) => {
            const { senderId, timestamp } = event.payload;
            const sender = senderId ? players[senderId] : null;
            return (
              <ChatMessage
                className="u-mb-small"
                // eslint-disable-next-line react/no-array-index-key
                key={idx}
                sender={sender}
                timestamp={timestamp}
              >
                {renderers[event.type](event.payload)}
              </ChatMessage>
            );
          })
        }
      </Panel>
    </PanelContainer>
  );
}

ChatMessageArea.propTypes = {
  events: PropTypes.arrayOf(GAME_EVENT_SHAPE),
  players: PropTypes.objectOf(PLAYER_SHAPE),
  renderers: PropTypes.objectOf(PropTypes.func).isRequired,
};

ChatMessageArea.defaultProps = {
  events: [],
  players: {},
};
