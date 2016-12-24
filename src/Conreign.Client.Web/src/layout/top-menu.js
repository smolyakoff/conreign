import React from 'react';
import cx from 'classnames';
import { Link } from 'react-router';

import logo from './logo.svg';

export function TopMenu({ className }) {
  return (
    <nav className={cx('c-nav', className, 'c-nav--inline')}>
      <li className="c-nav__item">
        <Link to="/">Home</Link>
      </li>
      <span className="c-nav__content u-window-box--small">
        <img className="o-image" src={logo}/>
      </span>
      <li className="c-nav__item">
        <Link to="/rules">Rules</Link>
      </li>
    </nav>
  );
}

export default TopMenu;
