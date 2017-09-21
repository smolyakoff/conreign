FROM microsoft/dotnet-framework:4.6.2
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["C:\app\Conreign.Server.Host.Console.Api.exe"]
EXPOSE 3000
