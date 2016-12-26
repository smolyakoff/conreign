import _ from 'lodash';
import xregexp from 'xregexp/src/xregexp';
import unicodeBase from 'xregexp/src/addons/unicode-base';
import unicodeCategories from 'xregexp/src/addons/unicode-categories';

unicodeBase(xregexp);
unicodeCategories(xregexp);
const HASH_TAG_CHAR_REGEX = xregexp('[\\pL\\d-_]');

// eslint-disable-next-line import/prefer-default-export
export function sanitizeRoomId(input) {
  return _.filter(input, HASH_TAG_CHAR_REGEX.test.bind(HASH_TAG_CHAR_REGEX))
    .map(x => x.toLowerCase())
    .join('');
}
