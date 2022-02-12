import {
  BEGIN_ROUTE_TRANSITION,
  END_ROUTE_TRANSITION,
} from './route-transition-actions';

export default function countRouteActions(state = false, action) {
  switch (action.type) {
    case BEGIN_ROUTE_TRANSITION:
      return true;
    case END_ROUTE_TRANSITION:
      return false;
    default:
      return state;
  }
}
