import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';
import cn from 'classnames';

const tableBlock = bem('c-plain-table');

function mapPropsByDefault(props) {
  return {
    modifiers: {},
    dom: props,
  };
}

function createComponent(
  tag,
  bemClass,
  mapProps = mapPropsByDefault,
) {
  const Tag = tag;
  const component = ({
    className,
    children,
    ...otherProps
  }) => {
    const { dom, modifiers } = mapProps(otherProps);
    const finalClassName = cn(
      bemClass(modifiers)(),
      className,
    );
    return (
      <Tag
        className={finalClassName}
        {...dom}
      >
        {children}
      </Tag>
    );
  };
  component.propTypes = {
    className: PropTypes.string,
    children: PropTypes.node,
  };
  component.defaultProps = {
    className: null,
    children: null,
  };
  return component;
}

export const Table = createComponent('table', tableBlock);
export const THead = createComponent('thead', tableBlock('head'));
export const TBody = createComponent(
  'tbody',
  tableBlock('body'),
  ({ striped, ...dom }) => ({
    dom,
    modifiers: {
      striped,
    },
  }),
);
export const TR = createComponent(
  'tr',
  tableBlock('row'),
  ({ heading, textAlignment, ...dom }) => ({
    dom,
    modifiers: {
      heading,
      text: textAlignment,
    },
  }),
);

function createCellComponent(tagName, bemClass) {
  return createComponent(
    tagName,
    bemClass,
    ({ textAlignment, ...dom }) => ({
      dom,
      modifiers: {
        text: textAlignment,
      },
    }),
  );
}

export const TD = createCellComponent(
  'td',
  tableBlock('cell'),
);
export const TH = createCellComponent(
  'th',
  tableBlock('head-cell'),
);
