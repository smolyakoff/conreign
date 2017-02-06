import block from 'bem-cn';
import './theme.scss';

block.setup({
  mod: '--',
  modValue: '-',
});

export * from './controls';
export * from './animation';
export { default as animation } from './animation';
