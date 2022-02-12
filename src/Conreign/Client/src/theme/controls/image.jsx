import bem from 'bem-cn';
import React from 'react';
import PropTypes from 'prop-types';

export default function Image({ className, ...others }) {
  // eslint-disable-next-line jsx-a11y/alt-text
  return <img className={bem('o-image').mix(className)()} {...others} />;
}

Image.propTypes = {
  className: PropTypes.string,
};

Image.defaultProps = {
  className: null,
};
