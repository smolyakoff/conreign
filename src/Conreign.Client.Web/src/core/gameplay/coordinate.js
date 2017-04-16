import { memoize } from 'lodash';

export function getRowIndex(position, width) {
  return position % width;
}

export function getColumnIndex(position, width) {
  return Math.floor(position / width);
}

export function getPosition(x, y, width) {
  return x + (y * width);
}

function generatePathImpl(start, end, width) {
  const srcX = getRowIndex(start, width);
  const srcY = getColumnIndex(start, width);
  const destX = getRowIndex(end, width);
  const destY = getColumnIndex(end, width);
  const distanceX = Math.abs(destX - srcX);
  const distanceY = Math.abs(destY - srcY);
  let current = start;
  const path = new Array(distanceX + distanceY + 1);
  path[0] = current;
  let i = 1;
  const dx = destX >= srcX ? 1 : -1;
  const dy = destY >= srcY ? width : -width;
  let [delta, nextDelta] = distanceX > distanceY ? [dx, dy] : [dy, dx];
  for (let k = 0; k < Math.abs(distanceX - distanceY); k += 1) {
    current += delta;
    path[i] = current;
    i += 1;
  }
  while (current !== end) {
    current += delta;
    path[i] = current;
    i += 1;
    [delta, nextDelta] = [nextDelta, delta];
  }
  return path;
}

export const generatePath = memoize(
  generatePathImpl,
  (...args) => args.join('-'),
);
