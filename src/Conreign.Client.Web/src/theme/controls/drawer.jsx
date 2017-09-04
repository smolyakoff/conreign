import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';
import { values, noop } from 'lodash';

import { Position } from './enums';

function Overlay({ dismissable, ...others }) {
  const block = bem('c-overlay');
  const modifiers = {
    dismissable,
  };
  return (
    <div className={block(modifiers)()} {...others} />
  );
}

Overlay.propTypes = {
  dismissable: PropTypes.bool,
};

Overlay.defaultProps = {
  dismissable: false,
};

function Drawer({ className, children, visible, position, ...others }) {
  const block = bem('o-drawer');
  const modifiers = {
    visible,
    [position]: true,
  };
  return (
    <div className={block(modifiers).mix(className)()} {...others}>
      {children}
    </div>
  );
}

Drawer.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  visible: PropTypes.bool,
  position: PropTypes.oneOf(values(Position)),
};

Drawer.defaultProps = {
  className: null,
  children: null,
  visible: false,
  position: Position.Top,
};

export default function DrawerContainer({
  className,
  children,
  drawerChildren,
  drawerClassName,
  drawerVisible,
  drawerPosition,
  onOverlayClick,
}) {
  const block = bem('o-drawer-container');
  return (
    <div className={block.mix(className)()}>
      {children}
      {
        drawerVisible && (
          <Overlay dismissable onClick={onOverlayClick} />
        )
      }
      <Drawer
        className={drawerClassName}
        visible={drawerVisible}
        position={drawerPosition}
      >
        {drawerChildren}
      </Drawer>
    </div>
  );
}

DrawerContainer.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  drawerChildren: PropTypes.node,
  drawerClassName: PropTypes.string,
  drawerVisible: PropTypes.bool,
  drawerPosition: PropTypes.oneOf(values(Position)),
  onOverlayClick: PropTypes.func,
};

DrawerContainer.defaultProps = {
  className: null,
  children: null,
  drawerChildren: Drawer.defaultProps.children,
  drawerClassName: Drawer.defaultProps.className,
  drawerVisible: Drawer.defaultProps.visible,
  drawerPosition: Drawer.defaultProps.position,
  onOverlayClick: noop,
};
