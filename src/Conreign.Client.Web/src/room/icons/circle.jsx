import React, { PropTypes } from 'react';

import './circle.scss';

export default function Circle({ color }) {
  return (
    <div
      className="o-circle"
      style={{ backgroundColor: color }}
    />
  );
}

Circle.propTypes = {
  color: PropTypes.string,
};

Circle.defaultProps = {
  color: 'white',
};
