import React, { Component } from 'react';
import { keyBy, values as objectValues, isFinite } from 'lodash';

import {
  PropertyTable,
  Input,
  ThemeSize,
  ThemeColor,
  StackPanel,
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

export default class GameSettingsForm extends Component {
  static propTypes = {
    defaultValues: GAME_SETTINGS_SHAPE.isRequired,
  };
  constructor(props, state) {
    super(props, state);
    this.state = {
      values: props.defaultValues,
    };
    this.fields = keyBy([
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
  }
  setField(e) {
    const name = e.target.name;
    const uiValue = e.target.value;
    const values = this.state.values;
    const currentValue = values[name];
    let modelValue = parseInt(uiValue, 10);
    if (!isFinite(modelValue)) {
      modelValue = 1;
    }
    if (modelValue === currentValue) {
      return;
    }
    this.setState({
      values: {
        ...values,
        [name]: modelValue,
      },
    });
  }
  createProperty(field) {
    const values = this.state.values;
    return {
      name: field.label,
      value: (
        <NumericInput
          name={field.name}
          value={values[field.name]}
          onChange={e => this.setField(e)}
        />
      ),
    };
  }
  render() {
    const properties = objectValues(this.fields)
      .map(f => this.createProperty(f));
    return (
      <div>
        <PropertyTable
          properties={properties}
          themeSpacing={ThemeSize.Small}
        />
        <Box type={BoxType.Letter} themeSize={ThemeSize.XSmall}>
          <StackPanel themeSpacing={ThemeSize.Small}>
            <Button>Randomize Map</Button>
            <Button themeColor={ThemeColor.Info}>Update Settings</Button>
          </StackPanel>
        </Box>
      </div>
    );
  }
}
