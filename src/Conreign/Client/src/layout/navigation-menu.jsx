import React from 'react';
import { Link } from 'react-router-dom';

import logo from './logo.svg';
import { Nav, NavItem, NavContent, Icon } from './../theme';

export default function NavigationMenu(props) {
  return (
    <Nav {...props} inline>
      <Link to="/">
        <NavItem>
          Home
        </NavItem>
      </Link>
      <NavContent className="u-window-box--small">
        <Icon name={logo.id} viewBox={logo.viewBox} />
      </NavContent>
      <Link to="/$/rules">
        <NavItem>Rules</NavItem>
      </Link>
    </Nav>
  );
}
