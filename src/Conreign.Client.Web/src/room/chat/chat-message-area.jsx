import React, { Component, PropTypes } from 'react';
import { AutoSizer, CellMeasurer, List, CellMeasurerCache } from 'react-virtualized';
import { drop, some } from 'lodash';

import { GAME_EVENT_SHAPE, PLAYER_SHAPE } from '../room-schemas';
import ChatMessage from './chat-message';

export default class ChatMessageArea extends Component {
  static propTypes = {
    events: PropTypes.arrayOf(GAME_EVENT_SHAPE),
    players: PropTypes.objectOf(PLAYER_SHAPE),
    currentUserId: PropTypes.string.isRequired,
    renderers: PropTypes.objectOf(PropTypes.func).isRequired,
  };
  static defaultProps = {
    events: [],
    players: {},
  };
  state = {};
  componentWillReceiveProps(nextProps) {
    const currentEvents = this.props.events;
    const nextEvents = nextProps.events;
    if (currentEvents.length === nextEvents.length) {
      return;
    }
    const newEvents = drop(nextEvents, currentEvents.length);
    const state =
      currentEvents.length === 0 ||
      some(newEvents, e => e.payload.senderId === this.props.currentUserId)
        ? { scrollIndex: nextEvents.length - 1 }
        : { scrollIndex: undefined };
    this.setState(state);
  }
  _cache = new CellMeasurerCache({ fixedWidth: true, minHeight: 50 });
  _renderRow = ({ index, key, parent, style }) => {
    const { events, players, renderers } = this.props;
    const event = events[index];
    const { senderId, timestamp } = event.payload;
    const sender = senderId ? players[senderId] : null;
    return (
      <CellMeasurer
        cache={this._cache}
        key={key}
        rowIndex={index}
        columnIndex={0}
        parent={parent}
      >
        <div
          className="u-letter-box--small"
          style={style}
        >
          <ChatMessage
            key={key}
            sender={sender}
            timestamp={timestamp}
          >
            {renderers[event.type](event.payload)}
          </ChatMessage>
        </div>

      </CellMeasurer>
    );
  };
  _resize = () => {
    this._cache.clearAll();
    this._list.recomputeRowHeights();
  };
  render() {
    const { events } = this.props;
    return (
      <AutoSizer onResize={this._resize}>
        {({ height, width }) => (
          <List
            ref={(list) => { this._list = list; }}
            deferredMeasurementCache={this._cache}
            height={height}
            width={width}
            rowCount={events.length}
            rowHeight={this._cache.rowHeight}
            rowRenderer={this._renderRow}
            scrollToIndex={this.state.scrollIndex}
          />
        )}
      </AutoSizer>
    );
  }
}
