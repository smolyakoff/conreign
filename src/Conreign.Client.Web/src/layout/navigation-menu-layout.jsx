import React, { PropTypes, Children, Component } from 'react';
import Measure from 'react-measure';

import { PanelContainer, Panel } from './../theme';
import NavigationMenu from './navigation-menu';
import './navigation-menu-layout.scss';

export default class NavigationMenuLayout extends Component {
  static propTypes = {
    children: PropTypes.node,
  };
  static defaultProps = {
    children: null,
  };
  constructor() {
    super();
    this.state = {};
  }
  onViewMeasure(dimensions) {
    this.setState({ dimensions });
  }
  renderChildren() {
    if (!this.state.dimensions) {
      return null;
    }
    const { children } = this.props;
    return Children.map(children, (child) => {
      const clone = React.cloneElement(
        child,
        { viewDimensions: this.state.dimensions },
      );
      return clone;
    });
  }
  render() {
    return (
      <PanelContainer className="u-full-height">
        <NavigationMenu className="u-centered" />
        <Measure
          whitelist={['width', 'height']}
          onMeasure={e => this.onViewMeasure(e)}
        >
          <Panel className="o-nav-view">
            { this.renderChildren() }
          </Panel>
        </Measure>
      </PanelContainer>
    );
  }
}
