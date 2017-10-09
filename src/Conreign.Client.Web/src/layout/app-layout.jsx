import React from 'react';
import { renderRoutes } from 'react-router-config';

import FooterLayout from './footer-layout';
import { ROUTE_SHAPE } from './../framework';
import { NotificationArea } from './../notifications';
import {
  SystemErrorNotification,
  ValidationErrorNotification,
} from './../errors';

const NOTIFICATION_RENDERERS = {
  SystemErrorNotification,
  ValidationErrorNotification,
};

export default function AppLayout({ route }) {
  return (
    <FooterLayout view={renderRoutes(route.routes)}>
      <NotificationArea renderers={NOTIFICATION_RENDERERS} />
    </FooterLayout>
  );
}

AppLayout.propTypes = {
  route: ROUTE_SHAPE.isRequired,
};

AppLayout.defaultProps = {
  children: null,
};
