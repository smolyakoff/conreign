import React from 'react';
import PropTypes from 'prop-types';
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
  onClick,
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
      onClick={onClick}
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
  onClick: PropTypes.func,
};

MapCell.defaultProps = {
  children: null,
  selection: CellSelection.None,
  onClick: noop,
};

function focus(props, event) {
  props.onClick({ event, cellIndex: props.cellIndex });
}

export default compose(
  pure,
  withHandlers({
    onClick: props => event => focus(props, event),
  }),
)(MapCell);
