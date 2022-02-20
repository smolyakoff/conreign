FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
RUN apk add --no-cache --update npm   
WORKDIR /workspace
COPY *.sln ./
COPY src/Conreign/*.csproj ./src/Conreign/
RUN dotnet restore --runtime linux-musl-x64
COPY . ./
RUN dotnet publish --runtime linux-musl-x64 --configuration Release --no-restore --self-contained false --output build

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
COPY --from=build /workspace/build ./
ENTRYPOINT ["dotnet", "Conreign.dll"]