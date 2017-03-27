import React, { PropTypes } from 'react';
import Measure from 'react-measure';

import {
  Grid,
  GridCell,
  GridMode,
  Deck,
  DeckItem,
  Widget,
  ThemeSize,
} from './../../theme';

import { Map } from './../map';
import { PLANET_SHAPE } from './../room-schemas';
import GameStatusBoard from './game-status-board';

const WIDGET_HEADER_HEIGHT = 35;

function calculateMapViewDimensions(viewDimensions) {
  const {
    width: fullVw,
    height: fullVh,
  } = viewDimensions;
  const vw = fullVw / 2;
  const vh = fullVh - WIDGET_HEADER_HEIGHT;
  return { width: vw, height: vh };
}

function GamePage({
  map,
  turn,
}) {
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
                header="Control Panel"
                className="u-full-height u-higher"
                bodyClassName="u-window-box--none"
              >
                <Deck>
                  <DeckItem>
                    <GameStatusBoard
                      turn={turn}
                      turnSeconds={turn}
                    />
                  </DeckItem>
                  <DeckItem stretch>
                    <Map
                      {...map}
                      viewWidth={mapViewWidth}
                      viewHeight={mapViewHeight}
                    />
                  </DeckItem>
                  <DeckItem>
                    Hello
                  </DeckItem>
                </Deck>
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
  turn: PropTypes.number.isRequired,
};

export default GamePage;
