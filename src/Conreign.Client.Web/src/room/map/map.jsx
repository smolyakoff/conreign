import React, { PropTypes } from 'react';
import { range } from 'lodash';
import block from 'bem-cn';

import './map.scss';

function Map({ className, width, height, cellRenderer }) {
  const map = block('c-map');

  function cell(cellIndex) {
    const widthPercentage = `${100 / width}%`;
    return (
      <div
        key={cellIndex}
        className={map('cell').mix(className)()}
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
  className: PropTypes.string,
  width: PropTypes.number.isRequired,
  height: PropTypes.number.isRequired,
  cellRenderer: PropTypes.func,
};

Map.defaultProps = {
  className: null,
  cellRenderer: () => null,
};

export default Map;
