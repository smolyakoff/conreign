import cx from 'classnames';
import React, { PropTypes } from 'react';
import { values } from 'lodash';

import { isNonEmptyString } from './util';

export const ThemeColor = {
  Default: 'default',
  Brand: 'brand',
  Info: 'info',
  Warning: 'warning',
  Success: 'success',
  Error: 'error',
};

export function supportsThemeColors(blockClass) {
  return function wrap(Control) {
    function Colored({ className, themeColor, ...others }) {
      const css = cx(
        className,
        isNonEmptyString(themeColor) ? `${blockClass}--${themeColor}` : null,
      );
      return <Control className={css} {...others} />;
    }
    Colored.displayName = `Colored${Control.displayName || 'Control'}`;
    Colored.propTypes = {
      className: PropTypes.string,
      themeColor: PropTypes.oneOf(values(ThemeColor)),
    };
    return Colored;
  };
}

export const ThemeSize = {
  XSmall: 'xsmall',
  Small: 'small',
  Medium: 'medium',
  Large: 'large',
  XLarge: 'xlarge',
  Super: 'super',
};

export function supportsThemeSizes(blockClass = null) {
  function formatSizeClass(size) {
    return isNonEmptyString(blockClass)
      ? `${blockClass}--${size}`
      : `u-${size}`;
  }
  return function wrap(Control) {
    function Sized({ className, themeSize, ...others }) {
      const css = cx(
        className,
        isNonEmptyString(themeSize)
          ? formatSizeClass(themeSize)
          : null,
      );
      return <Control className={css} {...others} />;
    }
    Sized.displayName = `Sized${Control.displayName || 'Control'}`;
    Sized.propTypes = {
      className: PropTypes.string,
      themeSize: PropTypes.oneOf(values(ThemeSize)),
    };
    return Sized;
  };
}

export function supportsActiveState(blockClass) {
  return function wrap(Control) {
    function Activated({ className, active, ...others }) {
      const css = cx(
        className,
        isNonEmptyString(active) ? `${blockClass}--active` : null,
      );
      return <Control className={css} {...others} />;
    }
    Activated.displayName = `Activated${Control.displayName || 'Control'}`;
    Activated.propTypes = {
      className: PropTypes.string,
      active: PropTypes.bool,
    };
    Activated.defaultProps = {
      active: false,
    };
    return Activated;
  };
}
