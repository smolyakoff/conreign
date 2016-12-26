import block from 'bem-cn';
import React, { PropTypes } from 'react';

import { Alignment } from './enums';
import { supportsThemeColors, supportsThemeSizes } from './decorators';
import { isNonEmptyString } from './util';

const css = block('c-nav');

export function Nav({
  tagName,
  inline,
  fixed,
  position,
  className,
  children,
  ...others
}) {
  const modifiers = {
    inline,
    fixed,
    [position || Alignment.Top]: isNonEmptyString(position),
  };
  const Tag = tagName;
  return (
    <Tag className={css(modifiers).mix(className)()} {...others}>
      {children}
    </Tag>
  );
}

Nav.defaultProps = {
  tagName: 'nav',
  inline: false,
  fixed: false,
  position: null,
};

Nav.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  tagName: PropTypes.string,
  inline: PropTypes.bool,
  fixed: PropTypes.bool,
  position: PropTypes.oneOf([null, Alignment.Top, Alignment.Bottom]),
};

function NavContentBase({ tagName, className, children, ...others }) {
  const Tag = tagName;
  return (
    <Tag className={css('content').mix(className)()} {...others}>
      {children}
    </Tag>
  );
}

export const NavContent = supportsThemeSizes()(NavContentBase);

NavContentBase.defaultProps = {
  tagName: 'li',
};

NavContentBase.propTypes = {
  tagName: PropTypes.string,
  className: PropTypes.string,
  children: PropTypes.node,
};

function NavItemBase({
  tagName,
  className,

  alignment,
  active,
  children,
  ...others
}) {
  const modifiers = {
    active,
    [alignment || Alignment.Left]: isNonEmptyString(alignment),
  };
  const Tag = tagName;
  return (
    <Tag className={css('item', modifiers).mix(className)} {...others}>
      {children}
    </Tag>
  );
}

NavItemBase.propTypes = {
  tagName: PropTypes.string,
  className: PropTypes.string,
  children: PropTypes.node,
  alignment: PropTypes.oneOf([null, Alignment.Right]),
  active: PropTypes.bool,
};

NavItemBase.defaultProps = {
  tagName: 'li',
  active: false,
};

export const NavItem = supportsThemeColors(css('item')())(NavItemBase);
