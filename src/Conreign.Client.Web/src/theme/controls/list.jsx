import React, { PropTypes } from 'react';
import bem from 'bem-cn';

const list = bem('c-list');
const listItem = list('item');

export function List({
  className,
  children,
  ordered,
  unstyled,
  inline,
}) {
  const Tag = ordered ? 'ol' : 'ul';
  const modifiers = {
    ordered,
    unstyled,
    inline,
  };
  const finalClassName = list(modifiers).mix(className)();
  return (
    <Tag className={finalClassName}>
      {children}
    </Tag>
  );
}

List.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  ordered: PropTypes.bool,
  unstyled: PropTypes.bool,
  inline: PropTypes.bool,
};

List.defaultProps = {
  className: null,
  children: null,
  ordered: false,
  unstyled: false,
  inline: false,
};

export function ListItem({
  className,
  children,
  unstyled,
}) {
  const finalClassName = listItem({ unstyled }).mix(className)();
  return (
    <li className={finalClassName}>
      {children}
    </li>
  );
}

ListItem.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  unstyled: PropTypes.bool,
};

ListItem.defaultProps = {
  className: null,
  children: null,
  unstyled: false,
};
