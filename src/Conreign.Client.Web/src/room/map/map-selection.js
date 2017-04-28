import { isNumber, findKey, parseInt } from 'lodash';

export function updateMapSelection({
  mapSelection,
  cellIndex,
  planets,
  currentUserId,
}) {
  const planet = planets[cellIndex];
  if (!planet) {
    return mapSelection;
  }
  const { start, end } = mapSelection;
  const endPlanet = isNumber(end) ? planets[end] : null;

  // No destination planet selected
  if (endPlanet === null) {
    if (cellIndex === start) {
      return mapSelection;
    }
    return {
      ...mapSelection,
      end: cellIndex,
    };
    // Destination planet is mine
  } else if (endPlanet.ownerId === currentUserId) {
    if (planet.ownerId === currentUserId) {
      return {
        start: cellIndex,
        end: null,
      };
    }
    return {
      ...mapSelection,
      end: cellIndex,
    };
  }
  // Destination planet is foreign
  if (planet.ownerId === currentUserId) {
    return {
      start: cellIndex,
      end: null,
    };
  }
  if (end === cellIndex) {
    return mapSelection;
  }
  return {
    ...mapSelection,
    end: cellIndex,
  };
}

function moveCoordinate(previousPlanets, previousCoordinate, currentPlanets) {
  if (!isNumber(previousCoordinate)) {
    return null;
  }
  const planet = previousPlanets[previousCoordinate];
  return planet ? findKey(currentPlanets, v => v.name === planet.name) : null;
}

const NULL_SELECTION = Object.freeze({ start: null, end: null });

export function resetMapSelection(previousSelection, previousMap, currentMap) {
  if (!previousSelection) {
    return NULL_SELECTION;
  }
  return {
    start: moveCoordinate(
      previousMap,
      previousSelection.start,
      currentMap,
    ),
    end: moveCoordinate(
      previousMap,
      previousSelection.end,
      currentMap,
    ),
  };
}

export function ensureMapSelection(mapSelection, planets, currentUser) {
  if (isNumber(mapSelection.start)) {
    return mapSelection;
  }
  const currentUserPlanetPosition = findKey(
    planets,
    p => p.ownerId === currentUser.id,
  );
  return { start: parseInt(currentUserPlanetPosition) };
}
