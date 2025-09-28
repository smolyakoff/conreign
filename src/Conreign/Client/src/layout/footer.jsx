import React from 'react';

import { Nav, NavContent, Text, ThemeSize } from './../theme';

export default function Footer(props) {
  const now = new Date();
  const year = now.getFullYear();
  return (
    <Nav {...props}>
      <NavContent>
        Conreign &copy; {year}
      </NavContent>
      <NavContent tagName="div" themeSize={ThemeSize.XSmall}>
        <Text emphasize="quiet">
          Icons made by <a href="http://www.freepik.com" title="Freepik">Freepik</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a> is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0" target="_blank" rel="noopener noreferrer">CC 3.0 BY</a>
        </Text>
      </NavContent>
    </Nav>
  );
}
