import React,{Component, PropTypes} from 'react';
import range from 'lodash/range';
import CSSModules from 'react-css-modules';

import styles from './map.scss';
import {MapTile} from './map-tile';
import {MapRow} from './map-row';

export class MapBase extends Component {
    static get propTypes() {
        return {
            width: PropTypes.number,
            height: PropTypes.number
        }
    }
    render() {
        const rows = range(0, this.props.height)
            .map(x => {
                const tiles = range(0, this.props.width).map(y => (<MapTile key={y}/>));
                return (<MapRow tiles={tiles} key={x}/>);
            });
        return (
            <section styleName="map">
                {rows}
            </section>
        );
    }
}

export const Map = CSSModules(MapBase, styles);