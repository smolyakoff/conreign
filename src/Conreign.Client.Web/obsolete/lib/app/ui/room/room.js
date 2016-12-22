import React, {Component} from 'react';
import CSSModules from 'react-css-modules';

import styles from './room.scss';
import {Map} from './map';

export class RoomBase extends Component {
    render() {
        return (
            <div styleName="container">
                <div styleName="column">
                    <Map width={12} height={10}/>
                </div>
                <div styleName="column">
                    Ahah
                </div>
            </div>
        );
    }
}

export const Room = CSSModules(RoomBase, styles);
export default Room;