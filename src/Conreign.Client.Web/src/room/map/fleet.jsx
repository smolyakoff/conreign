import React, { PropTypes } from 'react';
import bem from 'bem-cn';

import './fleet.scss';
import { Icon } from './../../theme';
import ufo from './../icons/ufo.svg';

const block = bem('c-fleet');

function Fleet({
  ships,
}) {
  return (
    <div className={block()}>
      <div className={block('icon')()}>
        <Icon name={ufo} />
      </div>
      <div className={block('text')()}>
        {ships}
      </div>
    </div>
  );
}

Fleet.propTypes = {
  ships: PropTypes.number.isRequired,
};

export default Fleet;
