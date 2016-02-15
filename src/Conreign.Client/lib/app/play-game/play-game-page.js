'use strict';
import Promise from 'bluebird';
import React, {Component} from 'react';

import {resolve, connect, compose} from './../core/core';
import {fetchGame} from './actions';

function load({dispatch, text}) {
    if (text) {
        return Promise.resolve();
    }
    return dispatch(fetchGame({id: 123}));
}

export class PlayGamePageBase extends Component {
    componentWillMount() {
        load(this.props);
    }
    render() {
        return (
            <div>
                <p>Play game goes here!</p>
                <div>{this.props.text}</div>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return state.game || {};
}

const decorate = compose(
    connect(mapStateToProps),
    resolve(load)
);

export const PlayGamePage = decorate(PlayGamePageBase);

export default PlayGamePage;