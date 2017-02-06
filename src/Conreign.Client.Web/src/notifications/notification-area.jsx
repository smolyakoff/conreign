import React, { PropTypes } from 'react';
import { connect } from 'react-redux';
import block from 'bem-cn';
import TransitionGroup from 'react-addons-css-transition-group';

import { animation, Side } from './../theme';
import { selectNotifications, hideNotification } from './notifications';
import Notification from './notification';
import './notification-area.scss';

const css = block('c-notification-area');

function NotificationArea({
  notifications,
  renderers,
  onNotificationClick,
}) {
  return (
    <div className={css.mix('u-small')()}>
      <TransitionGroup {...animation.slide({ side: Side.Right })}>
        {
          notifications.map((notification) => {
            const { id, content, rendererName } = notification;
            const Renderer = renderers[rendererName] || Notification;
            return (
              <Renderer
                key={id}
                {...content}
                id={id}
                onClose={onNotificationClick}
              />
            );
          })
        }
      </TransitionGroup>
    </div>
  );
}

NotificationArea.propTypes = {
  notifications: PropTypes.arrayOf(PropTypes.shape({
    id: PropTypes.string,
    content: PropTypes.any,
  })),
  renderers: PropTypes.objectOf(PropTypes.func),
  onNotificationClick: PropTypes.func.isRequired,
};

NotificationArea.defaultProps = {
  notifications: [],
  renderers: {},
};

export default connect(
  state => ({ notifications: selectNotifications(state) }),
  {
    onNotificationClick: hideNotification,
  },
)(NotificationArea);
