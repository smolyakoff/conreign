import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';

const widget = bem('c-widget');

function WidgetHeader({ children }) {
  return (
    <div className={widget('header')()}>
      {children}
    </div>
  );
}

WidgetHeader.propTypes = {
  children: PropTypes.node,
};

WidgetHeader.defaultProps = {
  children: null,
};

function WidgetBody({ className, children }) {
  return (
    <div className={widget('body').mix(className)()}>
      {children}
    </div>
  );
}

WidgetBody.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
};

WidgetBody.defaultProps = {
  className: null,
  children: null,
};

export default function Widget({
  className,
  children,
  header,
  bodyClassName,
  ...otherProps
}) {
  const headerUi = header
    ? (
      <WidgetHeader>
        {header}
      </WidgetHeader>
    )
    : null;
  return (
    <div
      className={widget.mix(className)()}
      {...otherProps}
    >
      {headerUi}
      <WidgetBody className={bodyClassName}>
        {children}
      </WidgetBody>
    </div>
  );
}

Widget.propTypes = {
  className: PropTypes.string,
  children: PropTypes.node,
  header: PropTypes.node,
  bodyClassName: PropTypes.string,
};

Widget.defaultProps = {
  className: null,
  children: null,
  header: null,
  bodyClassName: null,
};
