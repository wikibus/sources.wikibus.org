FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

RUN apt-get update && apt-get install --reinstall ca-certificates -y

RUN mkdir /output

# Copy project and publish

COPY . /app
RUN dotnet publish src/app --configuration Release --output /output

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy

RUN apt-get update
RUN apt-get install -y ghostscript
RUN apt-get install openssl

WORKDIR /app

COPY --from=build-env /output .

CMD dotnet app.dll
