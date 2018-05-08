import bem from 'bem-cn';
import './theme.scss';

bem.setup({
  mod: '--',
  modValue: '-',
});

export * from './controls';
export { default as mapEnumValueToModifierValue } from './map-enum-value-to-modifier-value';
