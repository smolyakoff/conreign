import React, { PropTypes } from 'react';
import { values } from 'lodash';
import block from 'bem-cn';

import { Orientation } from './enums';
import { decorate, withThemeSizes, ThemeSize } from './decorators';

const deck = block('o-deck');

export function DeckItem({ children, stretch }) {
  const item = deck('item');
  const modifiers = { stretch };
  return (
    <div className={item(modifiers)()}>
      {children}
    </div>
  );
}

DeckItem.propTypes = {
  children: PropTypes.node,
  stretch: PropTypes.bool,
};

DeckItem.defaultProps = {
  children: null,
  stretch: false,
};

function DeckBase({
  className,
  children,
  orientation,
  ...otherProps
}) {
  const modifiers = {
    orientation,
  };
  return (
    <div
      className={deck(modifiers).mix(className)()}
      {...otherProps}
    >
      {children}
    </div>
  );
}

DeckBase.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  orientation: PropTypes.oneOf(values(Orientation)),
};

DeckBase.defaultProps = {
  className: null,
  children: null,
  orientation: Orientation.Vertical,
};

export const Deck = decorate(
  withThemeSizes(deck(), { propName: 'themeSpacing', valuePrefix: 'spacing-', defaultValue: ThemeSize.Medium }),
)(DeckBase);
