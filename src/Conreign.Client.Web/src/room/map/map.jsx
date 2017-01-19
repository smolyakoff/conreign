import React, { PropTypes } from 'react';
import { range, noop, isString, values } from 'lodash';
import block from 'bem-cn';

import './map.scss';

const css = block('c-map');

const CellSelection = {
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
  function onContentCellFocus(e) {
    onFocus({ event: e, cellIndex });
  }
  const onCellFocus = content ? onContentCellFocus : null;
  const modifiers = {
    [`selection-${selection}`]: isString(selection),
  };
  const Tag = content ? 'button' : 'div';
  return (
    <Tag
      key={cellIndex}
      className={css('cell')(modifiers)}
      style={{ flexBasis: widthPercentage }}
      onFocus={onCellFocus}
    >
      <div className={css('content')}>
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
  onFocus: null,
};

function chooseCellSelection(cellIndex, selection) {
  if (selection.start === cellIndex) {
    return CellSelection.Start;
  }
  if (selection.end === cellIndex) {
    return CellSelection.End;
  }
  return CellSelection.None;
}

function Map({
  className,
  width,
  height,
  cellRenderer,
  selection,
  onSelectionChanged,
}) {
  function onCellFocus({ cellIndex }) {
    const currentSelection = {
      ...selection,
      start: cellIndex,
    };
    onSelectionChanged(currentSelection);
  }
  return (
    <div className={css.mix(className)}>
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
  onSelectionChanged: PropTypes.func,
  className: PropTypes.string,
  width: PropTypes.number.isRequired,
  height: PropTypes.number.isRequired,
  cellRenderer: PropTypes.func,
};

Map.defaultProps = {
  selection: {
    start: null,
    end: null,
  },
  onSelectionChanged: noop,
  className: null,
  cellRenderer: () => null,
};

export default Map;
