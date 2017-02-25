import bem from 'bem-cn';
import React, { PropTypes } from 'react';

import { Alignment } from './enums';
import {
  withThemeColors,
  withThemeSizes,
  decorate,
} from './decorators';
import { isNonEmptyString } from './util';

const css = bem('c-nav');

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
  className: null,
  children: null,
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

NavContentBase.displayName = 'NavContent';
NavContentBase.defaultProps = {
  tagName: 'li',
  className: null,
  children: null,
};

NavContentBase.propTypes = {
  tagName: PropTypes.string,
  className: PropTypes.string,
  children: PropTypes.node,
};

export const NavContent = decorate(withThemeSizes())(NavContentBase);

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

NavItemBase.displayName = 'NavItem';

NavItemBase.propTypes = {
  tagName: PropTypes.string,
  className: PropTypes.string,
  children: PropTypes.node,
  alignment: PropTypes.oneOf([null, Alignment.Right]),
  active: PropTypes.bool,
};

NavItemBase.defaultProps = {
  className: null,
  children: null,
  alignment: null,
  tagName: 'li',
  active: false,
};

export const NavItem = decorate(
  withThemeColors(css('item').toString()),
)(NavItemBase);
