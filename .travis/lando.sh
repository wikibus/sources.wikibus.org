#!/usr/bin/env bash

sudo apt-get -y update || true
sudo apt-get -y install cgroup-bin curl
curl -fsSL -o /tmp/lando-latest.deb https://github.com/lando/lando/releases/download/v3.0.0-rc.23/lando-v3.0.0-rc.23.deb
sudo dpkg -i /tmp/lando-latest.deb
lando version
