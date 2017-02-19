import cx from 'classnames';
import React, { PropTypes } from 'react';
import { values, flow, each, isFunction, extend, flatten, defaults, omit, isUndefined } from 'lodash';

import { isNonEmptyString } from './util';

export const ThemeColor = {
  Default: 'default',
  Brand: 'brand',
  Info: 'info',
  Warning: 'warning',
  Success: 'success',
  Error: 'error',
};

export function decorate(...decorators) {
  const decoratorsList = flatten(decorators);
  const mapProps = flow(decoratorsList.map(d => d.mapProps));
  const extenders = decoratorsList
    .map(d => d.extendStatics)
    .filter(isFunction);

  return function wrap(Control) {
    function Decorated(props) {
      const mappedProps = mapProps(props);
      return <Control {...mappedProps} />;
    }
    Decorated.displayName = `Decorated${Control.displayName || Control.name}`;
    each(extenders, e => e(Decorated));
    return Decorated;
  };
}

export function withThemeColors(blockClass) {
  function mapProps({ className, themeColor, ...others }) {
    const css = cx(
      className,
      isNonEmptyString(themeColor) ? `${blockClass}--${themeColor}` : null,
    );
    return {
      className: css,
      ...others,
    };
  }
  function extendStatics(Control) {
    const propTypes = Control.propTypes || {};
    extend(propTypes, {
      themeColor: PropTypes.oneOf(values(ThemeColor)),
    });
    extend(Control, {
      propTypes,
    });
  }

  return {
    mapProps,
    extendStatics,
  };
}

export const ThemeSize = {
  Tiny: 'tiny',
  XSmall: 'xsmall',
  Small: 'small',
  Medium: 'medium',
  Large: 'large',
  XLarge: 'xlarge',
  Super: 'super',
};

export const ThemeElevation = {
  None: null,
  High: 'high',
  Higher: 'higher',
  Highest: 'highest',
};

export const withThemeElevation = {
  mapProps: ({ className, themeElevation, ...otherProps }) => ({
    className: cx(
      className,
      isNonEmptyString(themeElevation) ? `u-${themeElevation}` : null,
    ),
    ...otherProps,
  }),
  extendStatics: (Control) => {
    Control.propTypes = {
      ...Control.propTypes,
      themeElevation: PropTypes.oneOf(values(ThemeElevation)),
    };
    Control.defaultProps = {
      ...Control.defaultProps,
    };
  },
};

export function withThemeSizes(
  blockClass = null,
  options,
) {
  options = defaults(options, {
    propName: 'themeSize',
    valuePrefix: '',
  });
  const { propName, valuePrefix } = options;

  function formatSizeClass(size) {
    return isNonEmptyString(blockClass)
      ? `${blockClass}--${valuePrefix}${size}`
      : `u-${valuePrefix}${size}`;
  }

  function mapProps({ className, ...others }) {
    const size = others[propName];
    const css = cx(
      className,
      isNonEmptyString(size)
        ? formatSizeClass(size)
        : null,
    );
    return {
      className: css,
      ...omit(others, propName),
    };
  }

  function extendStatics(Control) {
    Control.propTypes = {
      ...Control.propTypes,
      [propName]: PropTypes.oneOf(values(ThemeSize)),
    };
    if (!isUndefined(options.defaultValue)) {
      Control.defaultProps = {
        ...Control.defaultProps,
        [propName]: options.defaultValue,
      };
    }
  }

  return {
    mapProps,
    extendStatics,
  };
}

export function withActiveState(blockClass) {
  function mapProps({ className, active, ...others }) {
    const css = cx(
      className,
      isNonEmptyString(active) ? `${blockClass}--active` : null,
    );
    return {
      className: css,
      ...others,
    };
  }
  function extendStatics(Control) {
    const propTypes = Control.propTypes || {};
    const defaultProps = Control.defaultProps || {};
    extend(propTypes, {
      active: PropTypes.bool,
    });
    extend(defaultProps, {
      active: false,
    });
    extend(Control, {
      propTypes,
      defaultProps,
    });
  }

  return {
    mapProps,
    extendStatics,
  };
}
