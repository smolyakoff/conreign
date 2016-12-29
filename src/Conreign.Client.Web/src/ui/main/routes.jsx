import React from 'react';
import { Route, IndexRoute } from 'react-router';
import { LayoutContainer } from './../layout';
import { HomePage } from './../home';

export const route = (
  <Route path="/" component={LayoutContainer}>
    <IndexRoute component={HomePage} />
  </Route>
);

export default route;
