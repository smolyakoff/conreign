# [Conreign](http://conreign.win)

Conreign is a multiplayer turn-based strategy game. The gameplay was highly inspired by
[Konquest](https://www.kde.org/applications/games/konquest).

The main goal is to conquer all other player's planets on a rectangle map
by sending fleets from one planet to another.

**Try playing [here](http://conreign.win).** Use **[github](http://conreign.win/github)** room when in doubt 🙂.

## Technologies Used

Conreign is built with [Orleans](https://github.com/dotnet/orleans) which is .NET libary for building 
distributed applications. [ASP.NET SignalR](https://www.asp.net/signalr) is used as a proxy to expose 
APIs to a browser client. Browser client is written in JavaScript and uses [React](https://reactjs.org) for UI.

## Development

The following things are needed to build the project:

- Visual Studio 2017 Community Edition with C# and F# installed
- Node.js 8.x
- Windows Azure SDK 2.9 (optional)