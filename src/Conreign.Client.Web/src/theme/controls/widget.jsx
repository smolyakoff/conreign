import React, { PropTypes } from 'react';
import cn from 'classnames';

import { Card, CardItem, CardBody } from './card';
import { ThemeColor } from './decorators';

export default function Widget({
  className,
  children,
  header,
  bodyClassName,
  ...otherProps
}) {
  const headerUi = header
    ? (
      <CardItem themeColor={ThemeColor.Info}>
        {header}
      </CardItem>
    )
    : null;
  return (
    <Card className={cn(className, 'u-full-height')} {...otherProps}>
      {headerUi}
      <CardBody className={bodyClassName}>
        {children}
      </CardBody>
    </Card>
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
