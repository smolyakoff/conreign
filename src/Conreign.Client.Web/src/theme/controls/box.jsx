import React, { PropTypes } from 'react';
import { values } from 'lodash';
import bem from 'bem-cn';

import { ThemeSize } from './decorators';

export const BoxType = {
  Letter: 'letter',
  Pillar: 'pillar',
  Window: 'window',
};

export default function Box({ className, type, themeSize, children }) {
  const css = bem(`u-${type}-box`)({
    [themeSize || 'none']: true,
  });
  return (
    <div className={css.mix(className)()}>
      {children}
    </div>
  );
}

Box.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  type: PropTypes.oneOf(values(BoxType)),
  themeSize: PropTypes.oneOf(values(ThemeSize).concat(null)),
};

Box.defaultProps = {
  className: null,
  children: null,
  type: BoxType.Window,
  themeSize: ThemeSize.Medium,
};
