FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build-env
WORKDIR /app

RUN mkdir /output

# Copy project and publish

COPY . /app
RUN dotnet publish src/app --configuration Release --output /output

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:2.1-focal

RUN apt-get update
RUN apt-get install -y ghostscript

WORKDIR /app

COPY --from=build-env /output .

CMD dotnet app.dll
