import React, { PropTypes } from 'react';
import block from 'bem-cn';

import { Grid, GridCell } from './grid';

const table = block('c-property-table');

function Property({ name, value }) {
  return (
    <Grid gutter>
      <GridCell
        className={table('property-name')()}
        gutter={false}
        fixedWidth
      >
        {name}
      </GridCell>
      <GridCell
        className={table('property-value')()}
        gutter={false}
      >
        {value}
      </GridCell>
    </Grid>
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
    <div className={table.mix(className)()}>
      {properties.map(p => <Property key={p.name} {...p} />)}
    </div>
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

export default PropertyTable;
