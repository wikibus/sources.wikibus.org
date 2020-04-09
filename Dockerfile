FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

RUN mkdir /output

# Copy project and publish

COPY . /app
RUN dotnet publish src/app --configuration Release --output /output

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime

RUN apt-get update
RUN apt-get install -y ghostscript

WORKDIR /app

COPY --from=build-env /output .

CMD dotnet app.dll
