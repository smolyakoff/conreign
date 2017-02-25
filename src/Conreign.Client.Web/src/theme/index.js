import bem from 'bem-cn';
import './theme.scss';

bem.setup({
  mod: '--',
  modValue: '-',
});

export * from './controls';
export * from './animation';
export { default as animation } from './animation';
