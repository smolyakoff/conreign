FROM microsoft/dotnet-framework:4.6.2
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["C:\app\Conreign.Server.Host.Console.Silo.exe"]
# 40000 is a silo port (for other silos), 22222 is a gateway port (for clients)
EXPOSE 40000 22222
