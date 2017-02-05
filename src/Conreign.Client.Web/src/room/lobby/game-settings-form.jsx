import React, { PropTypes } from 'react';
import { keyBy, values as objectValues, isFinite } from 'lodash';
import { withHandlers } from 'recompose';

import {
  PropertyTable,
  Input,
  ThemeSize,
  Button,
  Box,
  BoxType,
} from './../../theme';
import { GAME_SETTINGS_SHAPE } from './lobby';

function NumericInput(props) {
  return (
    <Input
      type="number"
      step="1"
      {...props}
    />
  );
}

const FIELDS = keyBy([
  {
    name: 'mapWidth',
    label: 'Map Width',
  },
  {
    name: 'mapHeight',
    label: 'Map Height',
  },
  {
    name: 'neutralPlanetsCount',
    label: 'Neutral Planets',
  },
], f => f.name);


function createFormProperty(field, values, onChange) {
  return {
    name: field.label,
    value: (
      <NumericInput
        name={field.name}
        value={values[field.name]}
        onChange={onChange}
      />
    ),
  };
}

function GameSettingsForm({ onFieldChange, onSubmit, values }) {
  const properties = objectValues(FIELDS)
    .map(field => createFormProperty(field, values, onFieldChange));
  return (
    <div>
      <PropertyTable
        properties={properties}
        themeSpacing={ThemeSize.Small}
      />
      <Box type={BoxType.Letter} themeSize={ThemeSize.XSmall}>
        <Button onClick={onSubmit}>
          Generate Map
        </Button>
      </Box>
    </div>
  );
}

GameSettingsForm.propTypes = {
  values: GAME_SETTINGS_SHAPE.isRequired,
  onFieldChange: PropTypes.func.isRequired,
  onSubmit: PropTypes.func.isRequired,
};


function setField(event, props) {
  const name = event.target.name;
  const uiValue = event.target.value;
  const values = props.values;
  const currentValue = values[name];
  let modelValue = parseInt(uiValue, 10);
  if (!isFinite(modelValue)) {
    modelValue = 1;
  }
  if (modelValue === currentValue) {
    return;
  }
  props.onChange({
    ...props.values,
    [name]: modelValue,
  });
}

function submit(event, props) {
  props.onSubmit(props.values, event);
}

export default withHandlers({
  onFieldChange: props => event => setField(event, props),
  onSubmit: props => event => submit(event, props),
})(GameSettingsForm);
