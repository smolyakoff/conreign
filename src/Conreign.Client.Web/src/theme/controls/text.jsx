import React, { PropTypes } from 'react';
import bem from 'bem-cn';
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
  children,
  highlight,
  emphasize,
  help,
  mono,
  kbd,
  ...others
}) {
  const Tag = kbd ? 'kbd' : tagName;
  const css = bem('c-text');
  const modifiers = {
    highlight,
    [emphasize]: isNonEmptyString(emphasize),
    help,
    mono,
    kbd,
  };
  return (
    <Tag className={css(modifiers).mix(className)()} {...others}>
      {children}
    </Tag>
  );
}

TextBase.displayName = 'Text';
TextBase.propTypes = {
  tagName: PropTypes.oneOf(['span', 'abbr']),
  className: PropTypes.string,
  children: PropTypes.node,
  highlight: PropTypes.bool,
  emphasize: PropTypes.oneOf(TextEmphasizes),
  help: PropTypes.bool,
  mono: PropTypes.bool,
  kbd: PropTypes.bool,
};
TextBase.defaultProps = {
  className: null,
  children: null,
  tagName: 'span',
  highlight: false,
  emphasize: null,
  help: false,
  mono: false,
  kbd: false,
};

export const Text = decorate(withThemeSizes())(TextBase);

function Paragraph({ className, children, ...others }) {
  return (
    <p className={bem('c-paragraph').mix(className)()} {...others}>
      {children}
    </p>
  );
}

Paragraph.displayName = 'Paragraph';
Paragraph.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};
Paragraph.defaultProps = {
  className: null,
  children: null,
};

export const P = decorate(withThemeSizes())(Paragraph);

const heading = bem('c-heading');

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
  Heading.defaultProps = {
    children: null,
    className: null,
  };
  Heading.displayName = `H${i}`;
  return Heading;
});

export const H1 = headings[0];
export const H2 = headings[1];
export const H3 = headings[2];

const codeBlock = bem('c-code');

function CodeBase({ className, children, multiLine }) {
  const modifiers = {
    multiline: multiLine,
  };
  return (
    <code className={codeBlock(modifiers).mix(className)()}>
      {children}
    </code>
  );
}

CodeBase.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  multiLine: PropTypes.bool,
};

CodeBase.defaultProps = {
  className: null,
  children: null,
  multiLine: false,
};

export const Code = decorate(withThemeSizes())(CodeBase);
