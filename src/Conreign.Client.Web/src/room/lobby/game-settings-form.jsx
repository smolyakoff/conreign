import React from 'react';
import PropTypes from 'prop-types';
import { keyBy, values as objectValues, isFinite } from 'lodash';
import { withHandlers, compose, pure } from 'recompose';

import {
  PropertyTable,
  Input,
  ThemeSize,
  ThemeColor,
  Button,
  Deck,
  DeckItem,
  Orientation,
} from './../../theme';
import { GAME_OPTIONS_SHAPE } from './lobby-schemas';

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
  {
    name: 'botsCount',
    label: 'Bots Count',
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

function GameSettingsForm({
  canStart,
  onChange,
  onSubmit,
  onStart,
  values,
}) {
  const properties = objectValues(FIELDS)
    .map(field => createFormProperty(field, values, onChange));
  return (
    <Deck themeSpacing={ThemeSize.Small}>
      <DeckItem>
        <PropertyTable
          properties={properties}
          themeSpacing={ThemeSize.Small}
        />
      </DeckItem>
      <DeckItem>
        <Deck orientation={Orientation.Horizontal}>
          <DeckItem>
            <Button onClick={onSubmit}>
              Generate Map
            </Button>
          </DeckItem>
          <DeckItem>
            <Button
              disabled={!canStart}
              themeColor={ThemeColor.Brand}
              onClick={onStart}
            >
              Start Game
            </Button>
          </DeckItem>
        </Deck>
      </DeckItem>
    </Deck>
  );
}

GameSettingsForm.propTypes = {
  values: GAME_OPTIONS_SHAPE.isRequired,
  canStart: PropTypes.bool.isRequired,
  onChange: PropTypes.func.isRequired,
  onSubmit: PropTypes.func.isRequired,
  onStart: PropTypes.func.isRequired,
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

export default compose(
  withHandlers({
    onChange: props => event => setField(event, props),
    onSubmit: props => event => submit(event, props),
  }),
  pure,
)(GameSettingsForm);
