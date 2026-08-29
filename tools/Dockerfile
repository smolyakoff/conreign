FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG TARGETARCH
RUN apt-get update && apt-get install -y nodejs npm python3 make g++
WORKDIR /workspace
COPY *.sln ./
COPY src/Conreign/*.csproj ./src/Conreign/
RUN if [ "$TARGETARCH" = "arm64" ]; then RID=linux-arm64; else RID=linux-x64; fi \
    && dotnet restore --runtime $RID
COPY . ./
RUN if [ "$TARGETARCH" = "arm64" ]; then RID=linux-arm64; else RID=linux-x64; fi \
    && dotnet publish --runtime $RID --configuration Release --no-restore --self-contained false --output build

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /workspace/build ./
ENTRYPOINT ["dotnet", "Conreign.dll"]