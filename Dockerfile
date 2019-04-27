FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

RUN mkdir /output

# Copy project and publish

COPY . /app
RUN dotnet publish --configuration Debug --output /output

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime

WORKDIR /app

COPY --from=build-env /output .

ENV FOO=BAR

CMD dotnet app.dll
