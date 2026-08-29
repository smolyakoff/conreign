# [Conreign](https://conreign.smolyakov.uk)

Conreign is a multiplayer turn-based strategy game. The gameplay was highly inspired by
[Konquest](https://www.kde.org/applications/games/konquest).

The main goal is to conquer all other player's planets on a rectangle map
by sending fleets from one planet to another.

**Try playing [here](https://conreign.smolyakov.uk).** Use **[github](https://conreign.smolyakov.uk/github)** room when in doubt 🙂.

## Technologies Used

Conreign is built with [Orleans](https://github.com/dotnet/orleans) which is .NET libary for building 
distributed applications. [ASP.NET SignalR](https://www.asp.net/signalr) is used as a proxy to expose 
APIs to a browser client. Browser client is written in JavaScript and uses [React](https://reactjs.org) for UI.

## Deploy (Uncloud)

```sh
uc deploy -f tools/compose.yaml        # build, push via unregistry, deploy to oracle
# or without rebuild:
uc deploy -f tools/compose.yaml --no-build
```