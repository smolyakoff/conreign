import PropTypes from 'prop-types';

const routeShape = PropTypes.shape({
  component: PropTypes.func,
});

routeShape.routes = PropTypes.arrayOf(routeShape);

export default routeShape;
