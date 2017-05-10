import React, { PropTypes } from 'react';
import bem from 'bem-cn';
import { values } from 'lodash';

import { VerticalAlignment, HorizontalAlignment } from './enums';
import { decorate, withActiveState } from './decorators';

const tableBlock = bem('c-table');

export function Table({
  className,
  children,
  clickable,
  striped,
  condensed,
  ...others
}) {
  const modifiers = {
    clickable,
    striped,
    condensed,
  };
  return (
    <table
      className={tableBlock(modifiers).mix(className)()}
      {...others}
    >
      {children}
    </table>
  );
}

Table.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  clickable: PropTypes.bool,
  striped: PropTypes.bool,
  condensed: PropTypes.bool,
};

Table.defaultProps = {
  className: null,
  children: null,
  clickable: false,
  striped: false,
  condensed: false,
};

export function TableCaption({
  className,
  children,
  ...others
}) {
  return (
    <caption
      className={tableBlock('caption').mix(className)()}
      {...others}
    >
      {children}
    </caption>
  );
}

TableCaption.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

TableCaption.defaultProps = {
  className: null,
  children: null,
};

export function TableHead({
  className,
  children,
  ...others
}) {
  return (
    <thead
      className={tableBlock('head').mix(className)()}
      {...others}
    >
      {children}
    </thead>
  );
}

TableHead.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

TableHead.defaultProps = {
  className: null,
  children: null,
};

export function TableBody({
  className,
  children,
  ...others
}) {
  return (
    <thead
      className={tableBlock('body').mix(className)()}
      {...others}
    >
      {children}
    </thead>
  );
}

TableBody.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

TableBody.defaultProps = {
  className: null,
  children: null,
};

const rowBlock = tableBlock('row');

function TableRowBase({
  className,
  children,
  heading,
  disabled,
  clickable,
  verticalAlignment,
  ...others
}) {
  const modifiers = {
    heading,
    disabled,
    clickable,
    [`v-align-${verticalAlignment}`]: true,
  };
  return (
    <tr
      className={rowBlock(modifiers).mix(className)()}
      {...others}
    >
      {children}
    </tr>
  );
}

TableRowBase.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  heading: PropTypes.bool,
  disabled: PropTypes.bool,
  clickable: PropTypes.bool,
  verticalAlignment: PropTypes.oneOf(values(VerticalAlignment)),
};

TableRowBase.defaultProps = {
  className: null,
  children: null,
  heading: false,
  disabled: false,
  clickable: false,
  verticalAlignment: VerticalAlignment.Center,
};

export const TableRow = decorate(
  withActiveState(rowBlock()),
)(TableRowBase);

export function TableCell({
  className,
  children,
  fixedWidth,
  horizontalAlignment,
  ...others
}) {
  const modifiers = {
    'width-fixed': fixedWidth,
    'h-align': horizontalAlignment,
  };
  return (
    <td
      className={tableBlock('cell')(modifiers).mix(className)()}
      {...others}
    >
      {children}
    </td>
  );
}

TableCell.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  fixedWidth: PropTypes.bool,
  horizontalAlignment: PropTypes.oneOf(values(HorizontalAlignment)),
};

TableCell.defaultProps = {
  className: null,
  children: null,
  fixedWidth: false,
  horizontalAlignment: HorizontalAlignment.Left,
};
