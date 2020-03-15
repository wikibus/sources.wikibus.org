#!/usr/bin/env bash

echo "deb [arch=amd64] https://packages.microsoft.com/repos/azure-cli/ wheezy main" | sudo tee /etc/apt/sources.list.d/azure-cli.list
curl -L https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
sudo apt-get install -y apt-transport-https
sudo apt-get update && sudo apt-get install -y azure-cli
npm i -g azure-functions-core-tools@3 --unsafe-perm true
