import React, { PropTypes } from 'react';
import bem from 'bem-cn';

import { Progress, ThemeColor } from './../theme';
import './loading-bar.scss';

const css = bem('c-loading-bar');

export default function LoadingBar({ isHidden }) {
  return (
    <Progress
      className={css({ hidden: isHidden })()}
      themeColor={ThemeColor.Brand}
      value={100}
      rounded={false}
      animated
      striped
    />
  );
}

LoadingBar.propTypes = {
  isHidden: PropTypes.bool,
};

LoadingBar.defaultProps = {
  isHidden: false,
};
