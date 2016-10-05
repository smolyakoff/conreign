import React, {Component} from 'react';
import CSSModules from 'react-css-modules';

import styles from './map-row.scss';

export class MapRowBase extends Component {
    render() {
        return (
            <div styleName="map-row">
                {this.props.tiles}
            </div>
        );
    }
}

export const MapRow = CSSModules(MapRowBase, styles);