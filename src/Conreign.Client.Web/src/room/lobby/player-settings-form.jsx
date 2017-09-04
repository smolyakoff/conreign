import React from 'react';
import PropTypes from 'prop-types';
import { compose, withHandlers, withProps } from 'recompose';
import { isEmpty, includes, isEqual } from 'lodash';
import {
  createValidator,
  composeValidators,
  combineValidators,
} from 'revalidate';

import {
  Deck,
  DeckItem,
  Input,
  Button,
  Text,
  ThemeSize,
  ThemeColor,
  Orientation,
} from './../../theme';
import { PLAYER_OPTIONS_SHAPE } from './lobby-schemas';

const FIELD = {
  name: 'nickname',
};

function createTypedValidator(type, validate) {
  return createValidator(
    error => (...args) => validate(...args)
      ? undefined
      : {
        ...error,
        context: {
          ...error.context,
          value: args[0],
        },
      },
    (label, others) => ({
      type,
      context: { label, ...others },
    }),
  );
}

const ErrorType = {
  Required: 'required',
  AlreadyInUse: 'alreadyInUse',
};

const isRequired = createTypedValidator(
  ErrorType.Required,
  value => !isEmpty(value),
);
function notIn(valuesSelector) {
  return createTypedValidator(
    ErrorType.AlreadyInUse,
    (value, values) => !includes(valuesSelector(values), value),
  );
}

const MESSAGES = {
  [ErrorType.Required]: ({ label }) => `${label} cannot be empty.`,
  [ErrorType.AlreadyInUse]: ({ label, value }) => `${label} "${value}" is already taken.`,
};

function formatMessage(errors, field) {
  const error = errors[field];
  if (!error) {
    return null;
  }
  return MESSAGES[error.type](error.context);
}

const validate = combineValidators({
  [FIELD.name]: composeValidators(
    isRequired,
    notIn(x => x.usedNicknames),
  )('Nickname'),
});

function PlayerSettingsForm({
  values,
  previousValues,
  errors,
  onChange,
  onSubmit,
}) {
  const canSubmit = isEmpty(errors) && !isEqual(values, previousValues);

  return (
    <form onSubmit={onSubmit}>
      <Deck
        themeSpacing={ThemeSize.Small}
        orientation={Orientation.Horizontal}
      >
        <DeckItem stretch>
          <Input
            name={FIELD.name}
            type="text"
            value={values[FIELD.name]}
            onChange={onChange}
            themeColor={errors[FIELD.name] ? ThemeColor.Error : null}
          />
        </DeckItem>
        <DeckItem>
          <Button type="submit" disabled={!canSubmit}>
            Update
          </Button>
        </DeckItem>
      </Deck>
      <Text themeColor={ThemeColor.Error}>
        {formatMessage(errors, FIELD.name)}
      </Text>
    </form>
  );
}

PlayerSettingsForm.propTypes = {
  previousValues: PLAYER_OPTIONS_SHAPE.isRequired,
  values: PLAYER_OPTIONS_SHAPE.isRequired,
  errors: PropTypes.objectOf(PropTypes.object).isRequired,
  onChange: PropTypes.func.isRequired,
  onSubmit: PropTypes.func.isRequired,
};

function change(props, event) {
  const { name, value } = event.target;
  const values = props.values;
  const currentValue = values[name];
  if (value === currentValue) {
    return;
  }
  props.onChange({
    ...props.values,
    [name]: value,
  });
}

function submit(props, event) {
  event.preventDefault();
  props.onSubmit(props.values, event);
}

export default compose(
  withProps(props => ({
    errors: validate({
      ...props.values,
      usedNicknames: props.usedNicknames,
    }),
  })),
  withHandlers({
    onChange: props => event => change(props, event),
    onSubmit: props => event => submit(props, event),
  }),
)(PlayerSettingsForm);
