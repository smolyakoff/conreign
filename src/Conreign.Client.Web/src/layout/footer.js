import React from 'react';
import cx from 'classnames';

export function Footer({ className }) {
  const now = new Date();
  const year = now.getFullYear();
  return (
    <footer className={cx('c-nav', className)}>
      <span className="c-nav__content">Conreign &copy; {year}</span>
      <div className="c-nav__content u-xsmall">
        <div className="c-text--quiet">
          Icons made by <a href="http://www.freepik.com" title="Freepik">Freepik</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank">CC 3.0 BY</a>
        </div>
      </div>
    </footer>
  );
}

export default Footer;
