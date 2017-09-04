import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';

const spinner = bem('c-spinner-chasing-dots');

function Spinner({ className, size }) {
  const styles = {
    width: size,
    height: size,
  };
  return (
    <div className={spinner.mix(className)()} style={styles}>
      <div className={spinner('dot1')()} />
      <div className={spinner('dot2')()} />
    </div>
  );
}

Spinner.defaultProps = {
  className: null,
  size: 50,
};

Spinner.propTypes = {
  className: PropTypes.string,
  size: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number,
  ]),
};

export default Spinner;
