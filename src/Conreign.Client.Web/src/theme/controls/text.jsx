import React, { PropTypes } from 'react';
import block from 'bem-cn';
import { values, range } from 'lodash';

import { withThemeSizes, decorate } from './decorators';
import { isNonEmptyString } from './util';

export const TextEmphasize = {
  Quiet: 'quiet',
  Loud: 'loud',
};

export const TextEmphasizes = values(TextEmphasize);

function TextBase({
  className,
  tagName,
  text,
  children,
  highlight,
  emphasize,
  help,
  mono,
  kbd,
  ...others
}) {
  const Tag = kbd ? 'kbd' : tagName;
  const css = block('c-text');
  const modifiers = {
    highlight,
    [emphasize]: isNonEmptyString(emphasize),
    help,
    mono,
    kbd,
  };
  return (
    <Tag className={css(modifiers).mix(className)()} {...others}>
      {text || children}
    </Tag>
  );
}

TextBase.displayName = 'Text';
TextBase.propTypes = {
  tagName: PropTypes.oneOf(['span', 'abbr']),
  className: PropTypes.string,
  children: PropTypes.node,
  text: PropTypes.string,
  highlight: PropTypes.bool,
  emphasize: PropTypes.oneOf(TextEmphasizes),
  help: PropTypes.bool,
  mono: PropTypes.bool,
  kbd: PropTypes.bool,
};
TextBase.defaultProps = {
  tagName: 'span',
  highlight: false,
  emphasize: null,
  help: false,
  mono: false,
  kbd: false,
};

export const Text = decorate(withThemeSizes())(TextBase);

function Paragraph({ className, children, text, ...others }) {
  return (
    <p className={block('c-paragraph').mix(className)()} {...others}>
      {text || children}
    </p>
  );
}

Paragraph.displayName = 'Paragraph';
Paragraph.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  text: PropTypes.string,
};

export const P = decorate(withThemeSizes())(Paragraph);

const heading = block('c-heading');

const headings = range(1, 6).map((i) => {
  const Tag = `h${i}`;
  function Heading({ children, className }) {
    return (
      <Tag className={heading.mix(className)}>
        {children}
      </Tag>
    );
  }
  Heading.propTypes = {
    children: PropTypes.node,
    className: PropTypes.string,
  };
  Heading.displayName = `H${i}`;
  return Heading;
});

export const H1 = headings[0];
export const H2 = headings[1];
export const H3 = headings[2];
