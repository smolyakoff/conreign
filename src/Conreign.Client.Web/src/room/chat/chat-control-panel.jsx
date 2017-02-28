import React, { PropTypes } from 'react';
import { noop } from 'lodash';

import gear from 'evil-icons/assets/icons/ei-gear.svg';
import comment from 'evil-icons/assets/icons/ei-comment.svg';

import {
  IconButton,
  Grid,
  GridCell,
  VerticalAlignment,
} from './../../theme';


export default function ChatControlPanel({
  className,
  showSettings,
  onSettingsClick,
}) {
  return (
    <Grid
      className={className}
      gutter
      verticalAlignment={VerticalAlignment.Center}
    >
      {
        showSettings && (
          <GridCell fixedWidth>
            <IconButton iconName={gear} onClick={onSettingsClick} />
          </GridCell>
        )
      }
      <GridCell gutter={false}>
        <textarea />
      </GridCell>
      <GridCell fixedWidth>
        <IconButton iconName={comment}>
          Send
        </IconButton>
      </GridCell>
    </Grid>
  );
}

ChatControlPanel.propTypes = {
  className: PropTypes.string,
  showSettings: PropTypes.bool,
  onSettingsClick: PropTypes.func,
};

ChatControlPanel.defaultProps = {
  className: null,
  showSettings: false,
  onSettingsClick: noop,
};
