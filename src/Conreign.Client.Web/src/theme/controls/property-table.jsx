import React, { PropTypes } from 'react';
import block from 'bem-cn';

const table = block('c-property-table');

function Property({ name, value }) {
  return (
    <tr>
      <td className={table('property-name')()}>
        {name}
      </td>
      <td className={table('property-value')()}>
        {value}
      </td>
    </tr>
  );
}
Property.propTypes = {
  name: PropTypes.node,
  value: PropTypes.node,
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
  properties: [],
};

export default PropertyTable;
