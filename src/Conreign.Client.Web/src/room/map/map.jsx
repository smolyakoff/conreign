import React, { PropTypes } from 'react';
import { pure } from 'recompose';
import { range, isNumber, noop, memoize, includes } from 'lodash';
import bem from 'bem-cn';

import './map.scss';
import MapCell, { CellSelection } from './map-cell';
import { generatePath } from './../../core';

const block = bem('c-map');

const isInPath = memoize((position, start, end, width) => {
  if (position === start || position === end) {
    return false;
  }
  const path = generatePath({ start, end, width });
  return includes(path, position);
}, (...args) => args.join('-'));

function chooseCellSelection(cellIndex, { start, end }, width) {
  if (start === cellIndex) {
    return CellSelection.Start;
  }
  if (end === cellIndex) {
    return CellSelection.End;
  }
  if (isNumber(start) && isNumber(end)) {
    return isInPath(cellIndex, start, end, width)
      ? CellSelection.Intermediate
      : CellSelection.None;
  }
  return CellSelection.None;
}

function fitWidthToView(mapWidth, mapHeight, viewWidth = null, viewHeight = null) {
  const cellSizes = [];
  if (isNumber(viewWidth)) {
    const wCellSize = viewWidth / mapWidth;
    cellSizes.push(wCellSize);
  }
  if (isNumber(viewHeight)) {
    const hCellSize = viewHeight / mapHeight;
    cellSizes.push(hCellSize);
  }
  if (cellSizes.length === 0) {
    return null;
  }
  const width = Math.min(...cellSizes) * mapWidth;
  return width;
}

function Map({
  className,
  width,
  height,
  viewWidth,
  viewHeight,
  cells,
  selection,
  onCellFocus,
}) {
  const pxWidth = fitWidthToView(width, height, viewWidth, viewHeight);
  const style = {};
  if (isNumber(pxWidth)) {
    style.width = pxWidth;
  }
  return (
    <div className={block.mix(className)} style={style}>
      {
        range(0, height * width).map((cellIndex) => {
          const content = cells[cellIndex];
          const cellSelection = chooseCellSelection(cellIndex, selection, width);
          return (
            <MapCell
              key={cellIndex}
              cellIndex={cellIndex}
              selection={cellSelection}
              mapWidth={width}
              onFocus={content ? onCellFocus : noop}
            >
              {content}
            </MapCell>
          );
        })
      }
    </div>
  );
}

export const MAP_SELECTION_SHAPE = PropTypes.shape({
  start: PropTypes.number,
  end: PropTypes.number,
});

Map.propTypes = {
  selection: MAP_SELECTION_SHAPE,
  onCellFocus: PropTypes.func,
  className: PropTypes.string,
  width: PropTypes.number.isRequired,
  height: PropTypes.number.isRequired,
  viewWidth: PropTypes.number,
  viewHeight: PropTypes.number,
  cells: PropTypes.objectOf(PropTypes.node),
};

Map.defaultProps = {
  selection: {
    start: null,
    end: null,
  },
  viewWidth: null,
  viewHeight: null,
  className: null,
  cells: {},
  onCellFocus: noop,
};

export default pure(Map);
