import React, { PropTypes } from 'react';
import { values } from 'lodash';
import block from 'bem-cn';

import { Grid, GridCell } from './grid';
import { Orientation } from './enums';
import { decorate, withThemeSizes } from './decorators';

const css = block('o-stack-panel');

export function StackPanel({
  className,
  children,
  orientation,
  ...otherProps
}) {
  const modifiers = {
    [orientation]: true,
  };
  const cells = React.Children.map(children, child => (
    <GridCell
      className={css('item')()}
      width={orientation === Orientation.Vertical ? 100 : null}
      fixedWidth={orientation === Orientation.Horizontal}
    >
      {child}
    </GridCell>
  ));
  return (
    <Grid
      className={css(modifiers).mix(className)()}
      wrap
      {...otherProps}
    >
      {cells}
    </Grid>
  );
}

StackPanel.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  orientation: PropTypes.oneOf(values(Orientation)),
};

StackPanel.defaultProps = {
  className: null,
  children: null,
  orientation: Orientation.Horizontal,
};

export default decorate(
  withThemeSizes(css(), { propName: 'themeSpacing', valuePrefix: 'spacing-' }),
)(StackPanel);
