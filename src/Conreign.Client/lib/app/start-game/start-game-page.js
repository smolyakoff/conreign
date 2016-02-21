'use strict';
import React, {Component} from 'react';
import { Link } from 'react-router';

//import {api} from './../api';

export class StartGamePage extends Component {
    componentDidMount() {
        const api = require('./../api');
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