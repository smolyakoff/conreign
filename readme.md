# Conreign

Conreign is a multiplayer turn-based galaxy conquest strategy. 
The gameplay was highly inspired by
[Konquest](https://www.kde.org/applications/games/konquest).

The main goal is to conquer all other player's planets on a rectangle map
by sending fleets from one planet to another.

## Architecture

Conreign API uses [Orleans](https://github.com/dotnet/orleans) and 
[SignalR](https://www.asp.net/signalr) for communication with a browser game client.

## Development

The following things are needed to build the project:

- Visual Studio 2015 Community Edition with C# and F# installed
- Windows Azure SDK 2.9 (optional)
- Node.js 6.x