import { sumBy, flow } from 'lodash';

export default function count(items, predicate) {
  return sumBy(items, flow(predicate, Number));
}
