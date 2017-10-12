import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import block from 'bem-cn';
import { TransitionGroup } from 'react-transition-group';

import { Side, SlideTransition } from './../theme';
import { selectNotifications } from './../state';
import { hideNotification } from './notifications';
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
      <TransitionGroup>
        {
          notifications.map((notification) => {
            const { id, content, rendererName } = notification;
            const Renderer = renderers[rendererName] || Notification;
            return (
              <SlideTransition key={id} side={Side.Right}>
                <Renderer
                  id={id}
                  onClose={e => onNotificationClick(notification, e)}
                  {...content}
                />
              </SlideTransition>
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
