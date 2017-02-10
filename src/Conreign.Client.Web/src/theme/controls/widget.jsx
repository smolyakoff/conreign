import React, { PropTypes } from 'react';

import { Card, CardItem, CardBody } from './card';
import { ThemeColor } from './decorators';

export default function Widget({
  header,
  bodyClassName,
  children,
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
    <Card {...otherProps}>
      {headerUi}
      <CardBody className={bodyClassName}>
        {children}
      </CardBody>
    </Card>
  );
}

Widget.propTypes = {
  header: PropTypes.node,
  children: PropTypes.node,
  bodyClassName: PropTypes.string,
};

Widget.defaultProps = {
  header: null,
  children: null,
  bodyClassName: null,
};
