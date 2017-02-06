import { defaults, flatMap, fromPairs } from 'lodash';

function createTransitionName(
  name,
  stages,
) {
  stages = defaults(stages || {}, {
    appear: false,
    enter: true,
    leave: true,
  });
  const className = `u-animation-${name}`;
  const props = flatMap(stages, (enabled, stage) => {
    if (!enabled) {
      return [];
    }
    return [
      [
        stage,
        `${className}--${stage}`,
      ],
      [
        `${stage}Active`,
        `${className}--${stage}-active`,
      ],
    ];
  });
  return fromPairs(props);
}

function createAnimation(name, props) {
  props = defaults(props, {
    transitionEnterTimeout: 500,
    transitionLeaveTimeout: 300,
  });
  return {
    transitionName: createTransitionName(name),
    ...props,
  };
}

export const Side = {
  Top: 'top',
  Right: 'right',
  Bottom: 'bottom',
  Left: 'left',
};

function createSlideAnimation({
  side = Side.Top,
  ...props
}) {
  return createAnimation(`slide-${side}`, props);
}

export default {
  slide: createSlideAnimation,
};
