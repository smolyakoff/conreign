import React from 'react';
import { defaults, flatMap, fromPairs, values, invert } from 'lodash';
import { CSSTransition } from 'react-transition-group';
import PropTypes from 'prop-types';

export const Side = {
  Top: 'top',
  Right: 'right',
  Bottom: 'bottom',
  Left: 'left',
};

const ReactTransitionState = {
  Enter: 'enter',
  Exit: 'exit',
  Appear: 'appear',
};

const AnimateCssState = {
  Enter: ReactTransitionState.Enter,
  Exit: 'leave',
  Appear: ReactTransitionState.Appear,
};

const ReactTransitionStateNames = invert(ReactTransitionState);

function createAnimateCssClassNames(
  name,
  states,
) {
  states = defaults(states || {}, {
    [ReactTransitionState.Appear]: false,
    [ReactTransitionState.Enter]: true,
    [ReactTransitionState.Exit]: true,
  });
  const className = `u-animation-${name}`;
  const props = flatMap(states, (enabled, state) => {
    if (!enabled) {
      return [];
    }
    const stateName = ReactTransitionStateNames[state];
    const animateCssState = AnimateCssState[stateName];
    return [
      [
        state,
        `${className}--${animateCssState}`,
      ],
      [
        `${state}Active`,
        `${className}--${animateCssState}-active`,
      ],
    ];
  });
  return fromPairs(props);
}

export function SlideTransition({ side, ...props }) {
  const classNames = createAnimateCssClassNames(`slide-${side}`);
  return (
    <CSSTransition
      {...props}
      classNames={classNames}
      timeout={{ enter: 500, exit: 300 }}
    />
  );
}

SlideTransition.propTypes = {
  side: PropTypes.oneOf(values(Side)),
};

SlideTransition.defaultProps = {
  side: Side.Top,
};
