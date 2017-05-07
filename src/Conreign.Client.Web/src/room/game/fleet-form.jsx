import React, { PropTypes } from 'react';
import { parseInt, isFinite } from 'lodash';
import { compose, withHandlers } from 'recompose';

import arrows from './../icons/arrows.svg';

import {
  Grid,
  GridCell,
  Input,
  IconButton,
  Button,
  Text,
  P,
  ThemeColor,
  ThemeSize,
  VerticalAlignment,
} from './../../theme';

function FleetForm({
  ships,
  maxShips,
  distance,
  onRaiseButtonClick,
  onShipsInputChange,
  onFormSubmit,
}) {
  const canSubmit = ships > 0 && ships <= maxShips;
  const canRaise = ships < maxShips;
  const errorMessage = ships > maxShips
    ? `Only ${maxShips} ships are ready to be dispatched from the source planet.`
    : null;
  return (
    <form onSubmit={onFormSubmit}>
      <Grid
        gutter={ThemeSize.Small}
        verticalAlignment={VerticalAlignment.Center}
      >
        <GridCell fixedWidth>
          <IconButton
            disabled={!canRaise}
            iconName={arrows}
            themeSize={ThemeSize.Small}
            themeColor={ThemeColor.Info}
            onClick={onRaiseButtonClick}
          />
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
            type="submit"
            disabled={!canSubmit}
            themeColor={ThemeColor.Info}
          >
            Dispatch
          </Button>
        </GridCell>
      </Grid>
      {
        errorMessage &&
          <P
            themeColor={ThemeColor.Error}
            themeSize={ThemeSize.Small}
          >
            {errorMessage}
          </P>
      }
    </form>
  );
}

FleetForm.propTypes = {
  ships: PropTypes.oneOfType([PropTypes.number, PropTypes.string]).isRequired,
  maxShips: PropTypes.number.isRequired,
  distance: PropTypes.number.isRequired,
  onShipsInputChange: PropTypes.func.isRequired,
  onFormSubmit: PropTypes.func.isRequired,
  onRaiseButtonClick: PropTypes.func.isRequired,
};

const onShipsInputChange = props => (event) => {
  const value = parseInt(event.target.value);
  if (!isFinite(value)) {
    props.onChange({ ships: '' });
    return;
  }
  const ships = Math.abs(Math.floor(value));
  props.onChange({ ships });
};

const onFormSubmit = ({ ships, onSubmit }) => (event) => {
  event.preventDefault();
  onSubmit({ ships });
};

const onRaiseButtonClick = ({ onChange, maxShips }) => (event) => {
  event.preventDefault();
  onChange({ ships: maxShips });
};

const enhance = compose(
  withHandlers({
    onShipsInputChange,
    onFormSubmit,
    onRaiseButtonClick,
  }),
);

export default enhance(FleetForm);
