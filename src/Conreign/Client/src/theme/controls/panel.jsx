import bem from 'bem-cn';
import React from 'react';
import PropTypes from 'prop-types';

export function PanelContainer({ className, children, ...others }) {
  const css = bem('o-panel-container');
  return (
    <div className={css.mix(className)()} {...others}>
      {children}
    </div>
  );
}

PanelContainer.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

PanelContainer.defaultProps = {
  className: null,
  children: null,
};

export function Panel({
  hasTopNav,
  hasBottomNav,
  children,
  className,
  ...others
}) {
  const css = bem('o-panel');
  const modifiers = {
    'nav-top': hasTopNav,
    'nav-bottom': hasBottomNav,
  };
  return (
    <div className={css(modifiers).mix(className)()} {...others}>
      {children}
    </div>
  );
}

Panel.defaultProps = {
  hasTopNav: false,
  hasBottomNav: false,
  className: null,
  children: null,
};

Panel.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  hasTopNav: PropTypes.bool.isRequired,
  hasBottomNav: PropTypes.bool.isRequired,
};
