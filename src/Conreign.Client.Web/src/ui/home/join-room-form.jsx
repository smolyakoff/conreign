import React, { Component, PropTypes } from 'react';

import {
  Grid,
  GridCell,
  Button,
  InputContainer,
  Input,
  Icon,
} from './../theme';
import { sanitizeRoomId } from './join-room-form-util';

export default class JoinRoomForm extends Component {
  static propTypes = {
    onJoin: PropTypes.func.isRequired,
  };
  constructor() {
    super();
    this.state = {
      roomId: '',
    };
  }
  onRoomIdChange(roomId) {
    this.setState({
      roomId: sanitizeRoomId(roomId),
    });
  }
  onFormSubmit(e) {
    e.preventDefault();
    this.props.onJoin(this.state);
    return false;
  }
  render() {
    const { roomId } = this.state;
    return (
      <Grid>
        <GridCell className="u-centered" width={80} offset={10}>
          <form onSubmit={e => this.onFormSubmit(e)}>
            <InputContainer
              iconLeft
              themeSize="large"
              className="u-mb-medium"
            >
              <Icon tagName="span">#</Icon>
              <Input
                placeholder="your-galaxy-hastag"
                onChange={e => this.onRoomIdChange(e.target.value)}
                value={roomId}
              />
            </InputContainer>
            <Button
              themeSize="large"
              themeColor="brand"
              disabled={!roomId}
              type="submit"
            >
              Join
            </Button>
          </form>
        </GridCell>
      </Grid>
    );
  }
}
