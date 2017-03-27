import React, { PropTypes } from 'react';
import Measure from 'react-measure';

import {
  Grid,
  GridCell,
  GridMode,
  Box,
  Widget,
  ThemeSize,
} from './../../theme';

import { Map } from './../map';
import { PLANET_SHAPE, PLAYER_SHAPE } from './../room-schemas';
import GameStatusBoard from './game-status-board';

const WIDGET_HEADER_HEIGHT = 35;
const CONTROLS_HEIGHT = 78;

function calculateMapViewDimensions(viewDimensions) {
  const {
    width: fullVw,
    height: fullVh,
  } = viewDimensions;
  const vw = fullVw / 2;
  const vh = fullVh - WIDGET_HEADER_HEIGHT - CONTROLS_HEIGHT;
  return { width: vw, height: vh };
}

function GamePage({
  map,
  turn,
  players,
  currentUser,
}) {
  const currentPlayer = players[currentUser.id];
  return (
    <Measure>
      {(dimensions) => {
        const {
          width: mapViewWidth,
          height: mapViewHeight,
        } = calculateMapViewDimensions(dimensions);
        return (
          <Grid
            className="u-full-height"
            gutter={ThemeSize.Small}
            responsiveness={{
              [ThemeSize.Small]: GridMode.Full,
              [ThemeSize.Medium]: GridMode.Full,
            }}
          >
            <GridCell fixedWidth>
              <Widget
                header="Map"
                className="u-higher"
                bodyClassName="u-window-box--none"
              >
                <Box themeSize={ThemeSize.Medium}>
                  <GameStatusBoard
                    turn={turn}
                    turnSeconds={turn}
                    player={currentPlayer}
                  />
                </Box>
                <Map
                  {...map}
                  viewWidth={mapViewWidth}
                  viewHeight={mapViewHeight}
                />
              </Widget>
            </GridCell>
            <GridCell />
          </Grid>
        );
      }}
    </Measure>
  );
}

GamePage.propTypes = {
  map: PropTypes.shape({
    width: PropTypes.number.isRequired,
    height: PropTypes.number.isRequired,
    planets: PropTypes.objectOf(PLANET_SHAPE).isRequired,
  }).isRequired,
  players: PropTypes.objectOf(PLAYER_SHAPE).isRequired,
  currentUser: PropTypes.shape({
    id: PropTypes.string.isRequired,
  }).isRequired,
  turn: PropTypes.number.isRequired,
};

export default GamePage;
