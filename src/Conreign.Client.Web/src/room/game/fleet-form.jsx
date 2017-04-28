import React, { PropTypes } from 'react';
import { parseInt, isFinite } from 'lodash';
import { compose, withState, withHandlers } from 'recompose';

import {
  Grid,
  GridCell,
  Input,
  Button,
  Text,
  ThemeColor,
  ThemeSize,
  VerticalAlignment,
} from './../../theme';

function FleetForm({
  ships,
  distance,
  onShipsInputChange,
  onFormSubmit,
}) {
  return (
    <form onSubmit={onFormSubmit}>
      <Grid
        gutter={ThemeSize.Small}
        verticalAlignment={VerticalAlignment.Center}
      >
        <GridCell fixedWidth gutter={false}>
          <Button ghost>
            ▲
          </Button>
        </GridCell>
        <GridCell>
          <Input
            name="ships"
            type="number"
            value={ships}
            onChange={onShipsInputChange}
          />
        </GridCell>
        <GridCell fixedWidth>
          {'ships in '}
          <Text themeColor={ThemeColor.Default}>{distance}</Text>
          {' '}
          turns
        </GridCell>
        <GridCell fixedWidth>
          <Button
            themeColor={ThemeColor.Brand}
            ghost
          >
            Dispatch
          </Button>
        </GridCell>
      </Grid>
    </form>
  );
}

FleetForm.propTypes = {
  ships: PropTypes.number.isRequired,
  distance: PropTypes.number.isRequired,
  onShipsInputChange: PropTypes.func.isRequired,
  onFormSubmit: PropTypes.func.isRequired,
};

const onShipsInputChange = props => (event) => {
  const value = parseInt(event.target.value);
  if (!isFinite(value)) {
    props.setShips('');
    return;
  }
  const ships = Math.abs(Math.floor(value));
  props.setShips(ships);
};

const onFormSubmit = ({ ships, onSubmit }) => (event) => {
  event.preventDefault();
  onSubmit({ ships });
};

const enhance = compose(
  withState('ships', 'setShips', ({ maxShips }) => maxShips),
  withHandlers({
    onShipsInputChange,
    onFormSubmit,
  }),
);

export default enhance(FleetForm);
