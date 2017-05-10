import cn from 'classnames';
import React, { PropTypes } from 'react';
import { values, flow, each, isFunction, flatten, defaults, omit, isUndefined } from 'lodash';

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

export function withThemeColors(blockClass, options = {}) {
  options = defaults(options, {
    defaultValue: null,
    getClassName: props => blockClass
      ? `${blockClass}--${props.themeColor}`
      : `u-color-${props.themeColor}`,
  });
  function mapProps(props) {
    const { className, themeColor, ...others } = props;
    const css = cn(
      className,
      isNonEmptyString(themeColor)
        ? options.getClassName(props)
        : null,
    );
    return {
      className: css,
      ...others,
    };
  }
  function extendStatics(Control) {
    Control.propTypes = {
      ...Control.propTypes,
      themeColor: PropTypes.oneOf(values(ThemeColor)),
    };
    Control.defaultProps = {
      ...Control.defaultProps,
      themeColor: options.defaultValue,
    };
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
    className: cn(
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
    const css = cn(
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
    const css = cn(
      className,
      active ? `${blockClass}--active` : null,
    );
    return {
      className: css,
      ...others,
    };
  }
  function extendStatics(Control) {
    Control.propTypes = {
      ...Control.propTypes,
      active: PropTypes.bool,
    };
    Control.defaultProps = {
      ...Control.defaultProps,
      active: false,
    };
  }

  return {
    mapProps,
    extendStatics,
  };
}
