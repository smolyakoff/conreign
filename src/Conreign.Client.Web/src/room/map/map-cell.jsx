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
  children,
  cellIndex,
  selection,
  mapWidth,
  onFocus,
}) {
  const widthPercentage = `${100 / mapWidth}%`;
  const modifiers = {
    [`selection-${selection}`]: isString(selection),
  };
  const Tag = children ? 'button' : 'div';
  return (
    <Tag
      key={cellIndex}
      className={block(modifiers)()}
      style={{ flexBasis: widthPercentage }}
      onFocus={onFocus}
    >
      <div className={block('content')()}>
        {children}
      </div>
    </Tag>
  );
}

MapCell.propTypes = {
  children: PropTypes.node,
  cellIndex: PropTypes.number.isRequired,
  mapWidth: PropTypes.number.isRequired,
  selection: PropTypes.oneOf(values(CellSelection)),
  onFocus: PropTypes.func,
};

MapCell.defaultProps = {
  children: null,
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
