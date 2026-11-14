# One Dockerfile for all three services: pass the project name as a build argument.
#   docker build --build-arg PROJECT=PriceFlow.Api -t priceflow-api .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG PROJECT
WORKDIR /repo

COPY Directory.Build.props Directory.Packages.props ./
COPY src/ src/
RUN dotnet publish src/${PROJECT} -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
ARG PROJECT
WORKDIR /app
COPY --from=build /app .

ENV APP_DLL=${PROJECT}.dll
# The base image ships a non-root user, no reason to run as root.
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["sh", "-c", "exec dotnet $APP_DLL"]
