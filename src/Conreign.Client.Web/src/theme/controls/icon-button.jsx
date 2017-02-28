import React, { PropTypes } from 'react';
import bem from 'bem-cn';

import {
  decorate,
  withThemeSizes,
  withThemeColors,
  ThemeSize,
  ThemeColor,
} from './decorators';
import Icon from './icon';

const button = bem('c-icon-button');

function IconButton({
  className,
  iconName,
  children,
  ...others
}) {
  return (
    <button className={button.mix(className)()} {...others}>
      <div className={button('content')}>
        <Icon className={button('icon')()} name={iconName} />
        {children}
      </div>
    </button>
  );
}

IconButton.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  iconName: PropTypes.string.isRequired,
};

IconButton.defaultProps = {
  className: null,
  children: null,
};

export default decorate(
  withThemeSizes(button(), { defaultValue: ThemeSize.Medium }),
  withThemeColors(button(), { defaultValue: ThemeColor.Default }),
)(IconButton);
