import { cond, conforms, pick, isString } from 'lodash';

import { UserError } from './errors';

const isUserError = conforms({
  data: conforms({
    category: isString,
    code: isString,
  }),
});

function mapUserError(error) {
  const data = pick(error.data, [
    'message',
    'category',
    'code',
    'details',
  ]);
  return new UserError(data);
}

const mapErrorInternal = cond([
  [isUserError, mapUserError],
]);

export default function mapError(error) {
  return mapErrorInternal(error) || error;
}
