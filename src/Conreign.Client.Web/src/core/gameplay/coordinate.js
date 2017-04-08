import { memoize } from 'lodash';

export function getRowIndex({ position, width }) {
  return position % width;
}

export function getColumnIndex({ position, width }) {
  return Math.floor(position / width);
}

export function getPosition({ x, y, width }) {
  return x + (y * width);
}

function generatePathImpl({ start, end, width }) {
  let current = start;
  const path = [];
  while (current !== end) {
    let dx = 0;
    let dy = 0;
    const src = { position: current, width };
    const srcX = getRowIndex(src);
    const srcY = getColumnIndex(src);
    const dest = { position: end, width };
    const destX = getRowIndex(dest);
    const destY = getColumnIndex(dest);
    const distanceX = Math.abs(destX - srcX);
    const distanceY = Math.abs(destY - srcY);
    if (distanceX === distanceY) {
      dx += 1;
    } else if (distanceX > distanceY) {
      dx = destX - srcX > 0 ? 1 : -1;
    } else {
      dy = destY - srcY > 0 ? 1 : -1;
    }
    current = getPosition({
      x: srcX + dx,
      y: srcY + dy,
      width,
    });
    path.push(current);
  }
  return path;
}

export const generatePath = memoize(
  generatePathImpl,
  ({ start, end, width }) => [start, end, width].join('-'),
);
