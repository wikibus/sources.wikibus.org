#!/usr/bin/env bash

lando start
npx wait-on --timeout 30000 http://wikibus-sources.lndo.site
