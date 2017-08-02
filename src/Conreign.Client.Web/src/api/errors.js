import { BaseError } from 'make-error';
import { extend } from 'lodash';

export const UserErrorCategory = {
  Gameplay: 'GameplayError',
  Validation: 'ValidationError',
};

export const GameplayErrorCode = {
  GameIsAlreadyInProgress: 'GameIsAlreadyInProgress',
};

export class UserError extends BaseError {
  constructor(data) {
    super(data.message);
    extend(this, data);
  }
}
