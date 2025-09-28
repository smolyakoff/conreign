import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';
import { values } from 'lodash';

import { VerticalAlignment, HorizontalAlignment } from './enums';
import { decorate, withActiveState } from './decorators';

const tableBlock = bem('c-table');

export function FlexTable({
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

FlexTable.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  clickable: PropTypes.bool,
  striped: PropTypes.bool,
  condensed: PropTypes.bool,
};

FlexTable.defaultProps = {
  className: null,
  children: null,
  clickable: false,
  striped: false,
  condensed: false,
};

export function FlexTableCaption({
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

FlexTableCaption.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

FlexTableCaption.defaultProps = {
  className: null,
  children: null,
};

export function FlexTableHead({
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

FlexTableHead.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

FlexTableHead.defaultProps = {
  className: null,
  children: null,
};

export function FlexTableBody({
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

FlexTableBody.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

FlexTableBody.defaultProps = {
  className: null,
  children: null,
};

const rowBlock = tableBlock('row');

function FlexTableRowBase({
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

FlexTableRowBase.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  heading: PropTypes.bool,
  disabled: PropTypes.bool,
  clickable: PropTypes.bool,
  verticalAlignment: PropTypes.oneOf(values(VerticalAlignment)),
};

FlexTableRowBase.defaultProps = {
  className: null,
  children: null,
  heading: false,
  disabled: false,
  clickable: false,
  verticalAlignment: VerticalAlignment.Center,
};

export const FlexTableRow = decorate(
  withActiveState(rowBlock()),
)(FlexTableRowBase);

export function FlexTableCell({
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

FlexTableCell.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  fixedWidth: PropTypes.bool,
  horizontalAlignment: PropTypes.oneOf(values(HorizontalAlignment)),
};

FlexTableCell.defaultProps = {
  className: null,
  children: null,
  fixedWidth: false,
  horizontalAlignment: HorizontalAlignment.Left,
};
