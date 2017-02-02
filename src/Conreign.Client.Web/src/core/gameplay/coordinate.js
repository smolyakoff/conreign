export function getRowIndex({ position, width }) {
  return position % width;
}

export function getColumnIndex({ position, width }) {
  return Math.floor(position / width);
}
