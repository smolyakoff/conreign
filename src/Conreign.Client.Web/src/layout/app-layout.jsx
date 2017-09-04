import React from 'react';
import PropTypes from 'prop-types';

import FooterLayout from './footer-layout';
import { NotificationArea } from './../notifications';
import {
  SystemErrorNotification,
  ValidationErrorNotification,
} from './../errors';

const NOTIFICATION_RENDERERS = {
  SystemErrorNotification,
  ValidationErrorNotification,
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
