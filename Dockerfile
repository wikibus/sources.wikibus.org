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

ENV wikibus:sources:sql=$wikibus:sources:sql

RUN echo "ASPNETCORE_URLS=http://0.0.0.0:\$PORT\nDOTNET_RUNNING_IN_CONTAINER=true" > /app/setup_heroku_env.sh && chmod +x /app/setup_heroku_env.sh

CMD /bin/bash -c "source /app/setup_heroku_env.sh && dotnet app.dll"