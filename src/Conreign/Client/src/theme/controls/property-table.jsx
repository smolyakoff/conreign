import React from 'react';
import PropTypes from 'prop-types';
import bem from 'bem-cn';

import { decorate, withThemeSizes } from './decorators';

const table = bem('c-property-table');

function Property({ name, value }) {
  return (
    <tr>
      <td className={table('name')()}>
        {name}
      </td>
      <td className={table('value')()}>
        {value}
      </td>
    </tr>
  );
}
Property.propTypes = {
  name: PropTypes.node,
  value: PropTypes.node,
};

Property.defaultProps = {
  name: null,
  value: null,
};

function PropertyTable({ properties, className }) {
  return (
    <table className={table.mix(className)()}>
      <tbody>
        {properties.map(p => <Property key={p.name} {...p} />)}
      </tbody>
    </table>
  );
}

PropertyTable.propTypes = {
  className: PropTypes.string,
  properties: PropTypes.arrayOf(PropTypes.shape({
    name: PropTypes.node,
    value: PropTypes.node,
  })),
};

PropertyTable.defaultProps = {
  className: null,
  properties: [],
};

export default decorate(
  withThemeSizes(table(), { propName: 'themeSpacing', valuePrefix: 'spacing-' }),
)(PropertyTable);
