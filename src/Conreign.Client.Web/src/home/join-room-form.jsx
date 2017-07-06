import React, { Component, PropTypes } from 'react';

import {
  Grid,
  GridCell,
  Button,
  InputContainer,
  Input,
  ThemeSize,
  ThemeColor,
} from './../theme';
import sanitizeRoomId from './sanitize-room-id';

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
              themeSize={ThemeSize.Large}
              className="u-mb-medium"
            >
              <span className="c-icon">#</span>
              <Input
                placeholder="your-galaxy-hashtag"
                onChange={e => this.onRoomIdChange(e.target.value)}
                value={roomId}
              />
            </InputContainer>
            <Button
              themeSize={ThemeSize.Large}
              themeColor={ThemeColor.Brand}
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
