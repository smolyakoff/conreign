import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';

import {
  decorate,
  withThemeElevation,
  withThemeColors,
} from './decorators';

const css = bem('c-card');

function CardBase({ className, tagName, children, grouped }) {
  const Tag = tagName;
  const modifiers = {
    grouped,
  };
  return (
    <Tag className={css(modifiers).mix(className)()}>
      {children}
    </Tag>
  );
}

CardBase.propTypes = {
  className: PropTypes.string,
  tagName: PropTypes.string,
  children: PropTypes.node,
  grouped: PropTypes.bool,
};

CardBase.defaultProps = {
  className: null,
  tagName: 'div',
  children: null,
  grouped: false,
};

export const Card = decorate(withThemeElevation)(CardBase);

const itemCss = css('item');

function CardItemBase({ className, tagName, children, isDivider }) {
  const Tag = tagName;
  const modifiers = {
    divider: isDivider,
  };
  return (
    <Tag className={itemCss(modifiers).mix(className)()}>
      {children}
    </Tag>
  );
}

CardItemBase.propTypes = {
  className: PropTypes.string,
  tagName: PropTypes.string,
  children: PropTypes.node,
  isDivider: PropTypes.bool,
};

CardItemBase.defaultProps = {
  className: null,
  tagName: 'div',
  children: null,
  isDivider: false,
};

export const CardItem = decorate(withThemeColors(itemCss()))(CardItemBase);

export function CardHeader({ className, children }) {
  return (
    <header className={css('header').mix(className)()}>
      {children}
    </header>
  );
}

CardHeader.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

CardHeader.defaultProps = {
  className: null,
  children: null,
};

export function CardBody({ className, tagName, children }) {
  const Tag = tagName;
  return (
    <Tag className={css('body').mix(className)()}>
      {children}
    </Tag>
  );
}

CardBody.propTypes = {
  className: PropTypes.string,
  tagName: PropTypes.string,
  children: PropTypes.node,
};

CardBody.defaultProps = {
  className: null,
  tagName: 'div',
  children: null,
};

export function CardFooter({ className, children, block: isBlock }) {
  const modifiers = { block: isBlock };
  return (
    <footer className={css('footer')(modifiers).mix(className)()}>
      {children}
    </footer>
  );
}

CardFooter.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  block: PropTypes.bool,
};

CardFooter.defaultProps = {
  className: null,
  children: null,
  block: false,
};
