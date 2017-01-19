import block from 'bem-cn';
import React, { PropTypes } from 'react';

export function PanelContainer({ className, children, ...others }) {
  const css = block('o-panel-container');
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
  const css = block('o-panel');
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
