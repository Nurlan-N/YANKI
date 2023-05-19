FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0
COPY YankiApi.csproj /build/
RUN dotnet restore ./build/YankiApi.csproj
COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./YankiApi.csproj -c ${BUILDCONFIG} -o out /p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
EXPOSE 80
WORKDIR /app
COPY --from=build /build/out .
RUN apk add --no-cache tzdata
RUN ln -s /usr/share/zoneinfo/Asia/Baku /etc/localtime
ENTRYPOINT ["dotnet", "YankiApi.dll"]
