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

function ParagraphBase({ className, children, text, ...others }) {
  return (
    <p className={block('c-paragraph').mix(className)()} {...others}>
      {text || children}
    </p>
  );
}

ParagraphBase.displayName = 'Paragraph';
ParagraphBase.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  text: PropTypes.string,
};

export const Paragraph = decorate(withThemeSizes())(ParagraphBase);

const headings = range(1, 6).map((i) => {
  const Tag = `h${i}`;
  function Heading({ children }) {
    return (
      <Tag className="c-heading">
        {children}
      </Tag>
    );
  }
  Heading.propTypes = {
    children: PropTypes.node,
  };
  Heading.displayName = `H${i}`;
  return Heading;
});

export const H1 = headings[0];
export const H2 = headings[1];
export const H3 = headings[2];
