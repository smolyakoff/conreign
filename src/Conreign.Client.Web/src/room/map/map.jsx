import React, { PropTypes } from 'react';
import { compose, pure, withHandlers } from 'recompose';
import { range, isNumber } from 'lodash';
import bem from 'bem-cn';

import './map.scss';
import MapCell, { CellSelection } from './map-cell';

const block = bem('c-map');

function chooseCellSelection(cellIndex, selection) {
  if (selection.start === cellIndex) {
    return CellSelection.Start;
  }
  if (selection.end === cellIndex) {
    return CellSelection.End;
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
  cellRenderer,
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
          const cellSelection = chooseCellSelection(cellIndex, selection);
          return (
            <MapCell
              key={cellIndex}
              cellIndex={cellIndex}
              cellRenderer={cellRenderer}
              selection={cellSelection}
              mapWidth={width}
              mapHeight={height}
              onFocus={onCellFocus}
            />
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
  onCellFocus: PropTypes.func.isRequired,
  className: PropTypes.string,
  width: PropTypes.number.isRequired,
  height: PropTypes.number.isRequired,
  viewWidth: PropTypes.number,
  viewHeight: PropTypes.number,
  cellRenderer: PropTypes.func,
};

Map.defaultProps = {
  selection: {
    start: null,
    end: null,
  },
  viewWidth: null,
  viewHeight: null,
  className: null,
  cellRenderer: () => null,
};

function focus({ onSelectionChanged, selection }, { cellIndex }) {
  const currentSelection = {
    ...selection,
    start: cellIndex,
  };
  onSelectionChanged(currentSelection);
}

export default compose(
  pure,
  withHandlers({
    onCellFocus: props => event => focus(props, event),
  }),
)(Map);
