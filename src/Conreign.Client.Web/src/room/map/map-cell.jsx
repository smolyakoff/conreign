import React, { PropTypes } from 'react';
import { withHandlers, compose, pure } from 'recompose';
import { isString, values, noop } from 'lodash';
import bem from 'bem-cn';

import './map-cell.scss';

const block = bem('c-map-cell');

export const CellSelection = {
  Start: 'start',
  End: 'end',
  Intermediate: 'intermediate',
  None: null,
};

function MapCell({
  cellIndex,
  cellRenderer,
  selection,
  mapWidth,
  mapHeight,
  onFocus,
}) {
  const content = cellRenderer({
    width: mapWidth,
    height: mapHeight,
    cellIndex,
  });
  const widthPercentage = `${100 / mapWidth}%`;
  const modifiers = {
    [`selection-${selection}`]: isString(selection),
  };
  const Tag = content ? 'button' : 'div';
  return (
    <Tag
      key={cellIndex}
      className={block(modifiers)()}
      style={{ flexBasis: widthPercentage }}
      onFocus={onFocus}
    >
      <div className={block('content')()}>
        {content}
      </div>
    </Tag>
  );
}

MapCell.propTypes = {
  cellIndex: PropTypes.number.isRequired,
  mapWidth: PropTypes.number.isRequired,
  mapHeight: PropTypes.number.isRequired,
  cellRenderer: PropTypes.func,
  selection: PropTypes.oneOf(values(CellSelection)),
  onFocus: PropTypes.func,
};

MapCell.defaultProps = {
  cellRenderer: () => null,
  selection: CellSelection.None,
  onFocus: noop,
};

function focus(props, event) {
  props.onFocus({ event, cellIndex: props.cellIndex });
}

export default compose(
  pure,
  withHandlers({
    onFocus: props => event => focus(props, event),
  }),
)(MapCell);
