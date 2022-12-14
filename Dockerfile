FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

RUN mkdir /output

# Copy project and publish

COPY . /app
RUN dotnet publish src/app --configuration Release --output /output

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy

RUN apt-get update
RUN apt-get install -y ghostscript
RUN apt-get install openssl

WORKDIR /app

COPY --from=build-env /output .

CMD dotnet app.dll
