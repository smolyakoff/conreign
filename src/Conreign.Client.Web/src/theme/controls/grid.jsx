import bem from 'bem-cn';
import React from 'react';
import PropTypes from 'prop-types';
import {
  isNumber,
  isNil,
  isBoolean,
  isObject,
  keys,
  values,
  every,
  difference,
  each,
} from 'lodash';

import { VerticalAlignment } from './enums';
import { ThemeSize } from './decorators';
import { isNonEmptyString } from './util';

const css = bem('o-grid');

export const GridMode = {
  Fit: 'fit',
  Full: 'full',
};

export function GridCell({
  className,
  children,
  width,
  gutter,
  fixedWidth,
  responsiveWidth,
  offset,
  wrap,
  verticalAlignment,
  ...others
}) {
  const b = css('cell');
  const modifiers = {
    width: fixedWidth ? false : width,
    offset,
    wrap,
    'no-gutter': isBoolean(gutter) && !gutter,
    'width-fixed': fixedWidth,
    [verticalAlignment || VerticalAlignment.Top]: isNonEmptyString(verticalAlignment),
  };
  each(responsiveWidth || {}, (rWidth, size) => {
    modifiers[`width-${rWidth}@${size}`] = true;
  });
  let style = {};
  if (fixedWidth) {
    style = {
      ...style,
      width,
    };
  }
  return (
    <div className={b(modifiers).mix(className)()} {...others} style={style}>
      {children}
    </div>
  );
}

function validateResponsiveWidth(props, propName) {
  const value = props[propName];
  if (isNil(value)) {
    return;
  }
  if (!isObject(value)) {
    throw new Error(`${propName} should be an object mapping screen size to grid mode (fit or full).`);
  }
  const screenSizes = keys(value);
  const diff = difference(screenSizes, values(ThemeSize));
  if (diff.length > 0) {
    throw new Error(`${propName} specifies invalid screen sizes: ${diff.join(', ')}.`);
  }
  const numbers = values(value);
  if (!every(numbers, isNumber)) {
    throw new Error(`${propName} values should be numbers only.`);
  }
}

GridCell.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  width: PropTypes.number,
  gutter: PropTypes.bool,
  fixedWidth: PropTypes.bool,
  responsiveWidth: validateResponsiveWidth,
  offset: PropTypes.number,
  wrap: PropTypes.bool,
  verticalAlignment: PropTypes.oneOf(values(VerticalAlignment).concat(null)),
};


GridCell.defaultProps = {
  className: null,
  children: null,
  width: null,
  responsiveWidth: {},
  offset: null,
  verticalAlignment: null,
  wrap: false,
  fixedWidth: false,
  gutter: null,
};

export function Grid({
  gutter,
  wrap,
  className,
  children,
  verticalAlignment,
  responsiveness,
  ...others
}) {
  const modifiers = {
    'no-gutter': !gutter,
    gutter,
    wrap,
    [verticalAlignment || VerticalAlignment.Top]: isNonEmptyString(verticalAlignment),
    ...responsiveness || {},
  };
  return (
    <div className={css(modifiers).mix(className)()} {...others}>
      {children}
    </div>
  );
}

function validateResponsivenessConfiguration(props, propName) {
  const value = props[propName];
  if (isNil(value)) {
    return;
  }
  if (!isObject(value)) {
    throw new Error(`${propName} should be an object mapping screen size to grid mode (fit or full).`);
  }
  const screenSizes = keys(value);
  let diff = difference(screenSizes, values(ThemeSize));
  if (diff.length > 0) {
    throw new Error(`${propName} specifies invalid screen sizes: ${diff.join(', ')}.`);
  }
  const modes = values(value);
  diff = difference(modes, values(GridMode));
  if (diff.length > 0) {
    throw new Error(`${propName} specifies invalid grid modes: ${diff.join(', ')}.`);
  }
}

Grid.propTypes = {
  gutter: PropTypes.oneOf(values(ThemeSize).concat([null, true])),
  wrap: PropTypes.bool,
  className: PropTypes.string,
  children: PropTypes.node,
  verticalAlignment: PropTypes.oneOf(values(VerticalAlignment).concat([null])),
  responsiveness: validateResponsivenessConfiguration,
};

Grid.defaultProps = {
  className: null,
  children: null,
  verticalAlignment: null,
  responsiveness: {},
  gutter: null,
  wrap: false,
};
