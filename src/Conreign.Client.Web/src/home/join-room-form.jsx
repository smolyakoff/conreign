import React, { Component } from 'react';
import PropTypes from 'prop-types';

import {
  Grid,
  GridCell,
  Button,
  InputContainer,
  Input,
  P,
  Text,
  ValidationState,
  ThemeSize,
  ThemeColor,
} from './../theme';
import sanitizeRoomId from './sanitize-room-id';

export default class JoinRoomForm extends Component {
  static propTypes = {
    isSubmitting: PropTypes.bool,
    busyRoomId: PropTypes.string,
    onSubmit: PropTypes.func.isRequired,
  };
  static defaultProps = {
    isSubmitting: false,
    busyRoomId: null,
  };
  constructor() {
    super();
    this.state = {
      roomId: '',
    };
  }
  onInputChange = (event) => {
    const roomId = sanitizeRoomId(event.target.value);
    this.setState({ roomId });
  };
  onFormSubmit = (e) => {
    e.preventDefault();
    const { onSubmit } = this.props;
    onSubmit(this.state);
    return false;
  };
  render() {
    const { busyRoomId, isSubmitting } = this.props;
    const { roomId } = this.state;
    const showError = busyRoomId === roomId;
    return (
      <Grid>
        <GridCell
          className="u-centered"
          width={90}
          offset={5}
        >
          <form onSubmit={this.onFormSubmit}>
            <div style={{ minHeight: '5em' }}>
              {
                showError ? (
                  <P themeColor={ThemeColor.Warning}>
                    Galaxy
                    {' '}
                    <Text themeColor={ThemeColor.Brand}>{busyRoomId}</Text>
                    {' '}
                    is currently unreachable. Try to pick another one!
                  </P>
                ) : (
                  <P themeColor={ThemeColor.Default}>
                    Choose any hash tag for the galaxy and battle will begin!
                  </P>
                )
              }
            </div>
            <InputContainer
              iconLeft
              themeSize={ThemeSize.XLarge}
              className="u-mb-large"
            >
              <span className="c-icon">#</span>
              <Input
                placeholder="your-galaxy-hashtag"
                onChange={this.onInputChange}
                value={roomId}
                validationState={showError ? ValidationState.Error : null}
              />
            </InputContainer>
            <Button
              themeSize={ThemeSize.XLarge}
              themeColor={ThemeColor.Brand}
              disabled={!roomId || isSubmitting}
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
