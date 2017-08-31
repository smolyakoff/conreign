import React from 'react';

import './rules-page.scss';
import {
  Box,
  H1,
  P,
  Text,
  ThemeColor,
  List,
  ListItem,
  Table,
  TD,
  TR,
  TBody,
} from './../theme';

export default function RulesPage() {
  return (
    <Box>
      <H1 className="u-pt-zero">
        Rules
      </H1>
      <P>
        <Text themeColor={ThemeColor.Brand}>Conreign</Text> is a turn-based tactical
        galaxy conquest game. The main idea is to conquer planets of all other players
        on a rectangular map by sending spaceships from one planet to another.
      </P>
      <P>Planets are described by the following characteristics:</P>
      <Table className="c-planet-info-table">
        <TBody>
          <TR>
            <TD>Production Rate</TD>
            <TD>Number of ships the planet produces at each new turn.</TD>
          </TR>
          <TR>
            <TD>Power</TD>
            <TD>
              The power of ships launched from this planet. The higher the power - the
              more chances is to win the battle. Number of ships in the fleet counts as
              well, of course.
            </TD>
          </TR>
        </TBody>
      </Table>
      <List>
        <ListItem>
          Player who first entered a galaxy will be able to configure map size and
          number of neutral planets randomly generated on the map.
        </ListItem>
        <ListItem>
          Each player owns a single planet at start.
        </ListItem>
        <ListItem>
          A number of neutral planets are placed on the map as well.
        </ListItem>
        <ListItem>
          Maximal turn duration is 60 seconds.
        </ListItem>
        <ListItem>
          You can also send reinforcements to your own planets.
        </ListItem>
      </List>
    </Box>
  );
}
