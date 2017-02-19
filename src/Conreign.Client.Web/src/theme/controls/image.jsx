import block from 'bem-cn';
import React, { PropTypes } from 'react';

export default function Image({ className, ...others }) {
  // eslint-disable-next-line jsx-a11y/img-has-alt
  return <img className={block('o-image').mix(className)()} {...others} />;
}

Image.propTypes = {
  className: PropTypes.string,
};

Image.defaultProps = {
  className: null,
};
