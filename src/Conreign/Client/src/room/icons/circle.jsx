import React from 'react';
import PropTypes from 'prop-types';
import block from 'bem-cn';

import './circle.scss';

const css = block('o-circle');

export default function Circle({ className, color }) {
  return (
    <div
      className={css.mix(className)()}
      style={{ backgroundColor: color }}
    />
  );
}

Circle.propTypes = {
  className: PropTypes.string,
  color: PropTypes.string,
};

Circle.defaultProps = {
  className: null,
  color: 'white',
};
