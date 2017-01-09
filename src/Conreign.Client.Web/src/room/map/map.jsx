import React, { PropTypes } from 'react';
import { range } from 'lodash';
import block from 'bem-cn';

import './map.scss';

function Map({ width, height, cellRenderer }) {
  const map = block('c-map');

  function cell(cellIndex) {
    const widthPercentage = `${100 / width}%`;
    return (
      <div
        key={cellIndex}
        className={map('cell')()}
        style={{ flexBasis: widthPercentage }}
      >
        <div className={map('content')}>
          {
            cellRenderer({
              width,
              height,
              cellIndex,
            })
          }
        </div>
      </div>
    );
  }

  return (
    <div className={map()}>
      { range(0, height * width).map(cell) }
    </div>
  );
}

Map.propTypes = {
  width: PropTypes.number.isRequired,
  height: PropTypes.number.isRequired,
  cellRenderer: PropTypes.func,
};

Map.defaultProps = {
  cellRenderer: () => 'A',
};

export default Map;
