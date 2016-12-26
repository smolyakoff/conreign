import React from 'react';
import { Link } from 'react-router';

import logo from './logo.svg';
import { Nav, NavItem, NavContent, Image } from './../theme';

export default function NavigationMenu(props) {
  return (
    <Nav {...props} inline>
      <NavItem>
        <Link to="/">Home</Link>
      </NavItem>
      <NavContent className="u-window-box--small">
        <Image src={logo} />
      </NavContent>
      <NavItem>
        <Link to="/rules">Rules</Link>
      </NavItem>
    </Nav>
  );
}
