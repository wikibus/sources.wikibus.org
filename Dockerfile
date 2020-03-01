FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

RUN mkdir /output

# Copy project and publish

COPY . /app
RUN dotnet publish --configuration Release --output /output

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime

RUN apt-get update -y
RUN apt-get install -y poppler-utils libc6-dev libgdiplus libx11-dev

WORKDIR /app

COPY --from=build-env /output .

CMD dotnet app.dll
