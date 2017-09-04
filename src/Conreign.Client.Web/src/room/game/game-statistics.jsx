import React from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router';
import { connect } from 'react-redux';
import { compose, withHandlers } from 'recompose';
import { createSelector } from 'reselect';
import { values, identity, keyBy, mapValues, sortBy } from 'lodash';
import {
  ResponsiveContainer,
  BarChart,
  YAxis,
  Legend,
  CartesianGrid,
  Bar,
} from 'recharts';

import {
  Box,
  H1,
  H3,
  BoxType,
  Table,
  InputGroup,
  THead,
  TBody,
  TR,
  TD,
  TH,
  Grid,
  GridCell,
  Button,
  ThemeSize,
  ThemeColor,
  HorizontalAlignment,
} from './../../theme';
import { Nickname } from './../text';
import { GameStatistics as Statistics } from './game';
import { getRoomState } from './../../api';
import { GAME_STATISTICS } from './game-schemas';
import { PLAYER } from './../room-schemas';

const STATISTIC_LABELS = {
  [Statistics.ShipRatio]: 'Ships Destroyed / Ships Lost',
  [Statistics.BattleRatio]: 'Battles Won / Battles Lost',
  [Statistics.ShipsProduced]: 'Ships Produced',
};

const selectChartStatistics = createSelector(identity, (playerStatistics) => {
  const statisticNames = values(Statistics);
  const playersByName = keyBy(playerStatistics, p => p.nickname);
  return statisticNames
    .map(statisticName => ({
      label: STATISTIC_LABELS[statisticName],
      ...mapValues(playersByName, p => p[statisticName]),
    }));
});

const selectLegendItems = createSelector(
  identity,
  playerStatistics => playerStatistics.map(stats => ({
    value: stats.nickname,
    color: stats.color,
    type: 'square',
    id: stats.userId,
  })),
);

function GameStatistics({
  playerStatistics,
  onNewRoundClick,
}) {
  const chartStatistics = selectChartStatistics(playerStatistics);
  return (
    <Box themeSize={ThemeSize.Medium} type={BoxType.Pillar}>
      <H1 className="u-centered u-pt-zero">
        Game Over
      </H1>
      <Table>
        <colgroup>
          <col span={1} style={{ width: '1px' }} />
          <col span={1} style={{ width: '1px' }} />
        </colgroup>
        <THead>
          <TR
            heading
            textAlignment={HorizontalAlignment.Right}
          >
            <TH>#</TH>
            <TH textAlignment={HorizontalAlignment.Left}>
              Nickname
            </TH>
            <TH>Death Turn</TH>
            <TH>Ships Produced</TH>
            <TH>Battles Won</TH>
            <TH>Battles Lost</TH>
            <TH>Ships Destroyed</TH>
            <TH>Ships Lost</TH>
          </TR>
        </THead>
        <TBody striped>
          {
            playerStatistics.map((player, index) => (
              <TR
                key={player.userId}
                textAlignment={HorizontalAlignment.Right}
              >
                <TD>{index + 1}</TD>
                <TD textAlignment={HorizontalAlignment.Left}>
                  <Nickname {...player} />
                </TD>
                <TD>{player.deathTurn || '-'}</TD>
                <TD>{player.shipsProduced}</TD>
                <TD>{player.battlesWon}</TD>
                <TD>{player.battlesLost}</TD>
                <TD>{player.shipsDestroyed}</TD>
                <TD>{player.shipsLost}</TD>
              </TR>
            ))
          }
        </TBody>
      </Table>
      <Grid className="u-mt-large">
        {
          chartStatistics.map(stat => (
            <GridCell key={stat.label}>
              <H3 className="u-pt-zero u-centered">
                {stat.label}
              </H3>
              <ResponsiveContainer width="100%" height={300}>
                <BarChart
                  data={[stat]}
                  height={300}
                  margin={{ left: -10, right: 30 }}
                >
                  <YAxis />
                  <CartesianGrid strokeDasharray="3 3" />
                  {
                    sortBy(playerStatistics, p => stat[p.nickname])
                      .reverse()
                      .map(player => (
                        <Bar
                          key={player.userId}
                          dataKey={player.nickname}
                          fill={player.color}
                        />
                      ))
                  }
                </BarChart>
              </ResponsiveContainer>
            </GridCell>
          ))
        }
      </Grid>
      <Legend
        wrapperStyle={{ position: 'static', marginTop: 10 }}
        payload={selectLegendItems(playerStatistics)}
      />
      <InputGroup
        className="u-mt-xlarge"
        themeSize={ThemeSize.XLarge}
        style={{ display: 'flex', justifyContent: 'center' }}
      >
        <Button
          themeColor={ThemeColor.Brand}
          onClick={onNewRoundClick}
        >
          New Round
        </Button>
        <Button to="/" component={Link}>Go Home</Button>
      </InputGroup>
    </Box>
  );
}

GameStatistics.propTypes = {
  playerStatistics: PropTypes.arrayOf(PropTypes.shape({
    ...GAME_STATISTICS,
    ...PLAYER,
    maxBattleRatio: PropTypes.bool.isRequired,
    maxShipRatio: PropTypes.bool.isRequired,
    maxShipProduction: PropTypes.bool.isRequired,
  })).isRequired,
  onNewRoundClick: PropTypes.func.isRequired,
};

const onNewRoundClick =
  ({ onNewRound, roomId }) => () => onNewRound({ roomId });

const enhance = compose(
  connect(null, {
    onNewRound: getRoomState,
  }),
  withHandlers({
    onNewRoundClick,
  }),
);

export default enhance(GameStatistics);
