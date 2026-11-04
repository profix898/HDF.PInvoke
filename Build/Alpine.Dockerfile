# SDK version matches global.json's feature band; the tag fixes the Alpine baseline.
FROM mcr.microsoft.com/dotnet/sdk:10.0.403-alpine3.24
RUN apk add --no-cache build-base cmake ninja bash jq git
# .NET named mutexes require the executing UID to have a passwd entry.
ARG BUILD_UID=1000
ARG BUILD_GID=1000
RUN addgroup -g "$BUILD_GID" builder && adduser -D -u "$BUILD_UID" -G builder builder
ENV DOTNET_CLI_HOME=/tmp/dotnet-home NUGET_PACKAGES=/tmp/nuget-packages
WORKDIR /src
ENTRYPOINT ["bash", "Build/alpine.sh"]
