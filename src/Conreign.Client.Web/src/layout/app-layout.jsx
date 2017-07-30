import React, { PropTypes } from 'react';

import FooterLayout from './footer-layout';
import { NotificationArea } from './../notifications';
import { ErrorNotification } from './../errors';

const NOTIFICATION_RENDERERS = {
  ErrorNotification,
};

export default function AppLayout({ children }) {
  return (
    <FooterLayout view={children}>
      <NotificationArea renderers={NOTIFICATION_RENDERERS} />
    </FooterLayout>
  );
}

AppLayout.propTypes = {
  children: PropTypes.node,
};

AppLayout.defaultProps = {
  children: null,
};
