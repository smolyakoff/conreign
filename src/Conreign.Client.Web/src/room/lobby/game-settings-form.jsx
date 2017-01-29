import React from 'react';

import { PropertyTable, Input } from './../../theme';

export default function GameSettingsForm() {
  const fields = [
    {
      name: 'Map Width',
      value: <Input />,
    },
    {
      name: 'Map Height',
      value: <Input />,
    },
    {
      name: 'Neutral Planets',
      value: <Input />,
    },
  ];

  return (
    <div>
      <PropertyTable properties={fields} />
    </div>
  );
}
