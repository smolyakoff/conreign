import React, { PropTypes } from 'react';
import { pure } from 'recompose';
import { range, isNumber, noop } from 'lodash';
import bem from 'bem-cn';

import './map.scss';
import MapCell, { CellSelection } from './map-cell';
import { generatePath } from './coordinate-system';

const block = bem('c-map');

function chooseCellSelection(cellIndex, { start, end }, pathSet) {
  if (start === cellIndex) {
    return CellSelection.Start;
  }
  if (end === cellIndex) {
    return CellSelection.End;
  }
  if (pathSet === null) {
    return CellSelection.None;
  }
  return pathSet.has(cellIndex)
    ? CellSelection.Intermediate
    : CellSelection.None;
}

function isDoubleSelection({ start, end }) {
  return isNumber(start) && isNumber(end);
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
  onCellClick,
}) {
  const pxWidth = fitWidthToView(width, height, viewWidth, viewHeight);
  const style = {};
  if (isNumber(pxWidth)) {
    style.width = pxWidth;
  }
  const path = isDoubleSelection(selection)
    ? generatePath(selection.start, selection.end, width)
    : null;
  const pathSet = path === null ? null : new Set(path);
  return (
    <div className={block.mix(className)} style={style}>
      {
        range(0, height * width).map((cellIndex) => {
          const content = cells[cellIndex];
          const cellSelection = chooseCellSelection(cellIndex, selection, pathSet);
          return (
            <MapCell
              key={cellIndex}
              cellIndex={cellIndex}
              selection={cellSelection}
              mapWidth={width}
              onClick={content ? onCellClick : noop}
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
  onCellClick: PropTypes.func,
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
  onCellClick: noop,
};

export default pure(Map);
