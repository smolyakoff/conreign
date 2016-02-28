'use strict';
import React, {Component} from 'react';
import { Link } from 'react-router';

export class StartGamePage extends Component {
    componentDidMount() {
        console.log('mount');
    }
    render() {
        return (
            <div>
                <p>Start game goes here!</p>
                <Link to="/game">Go play</Link>
            </div>
        )
    }
}


export default StartGamePage;