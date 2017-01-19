import block from 'bem-cn';
import React, { PropTypes } from 'react';

export function Image({ className, ...others }) {
  // eslint-disable-next-line jsx-a11y/img-has-alt
  return <img className={block('o-image').mix(className)()} {...others} />;
}

Image.propTypes = {
  className: PropTypes.string,
};

Image.defaultProps = {
  className: null,
};

export function Icon({ tagName, className, children, ...others }) {
  const Tag = tagName;
  return (
    <Tag className={block('c-icon').mix(className)()} {...others}>
      {children}
    </Tag>
  );
}

Icon.propTypes = {
  tagName: PropTypes.string,
  className: PropTypes.string,
  children: PropTypes.node,
};

Icon.defaultProps = {
  tagName: 'i',
  className: null,
  children: null,
};
