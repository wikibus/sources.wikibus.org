#!/bin/sh

cp -r /lando/certs/lndo.site.pem /usr/local/share/ca-certificates/lndo.site.pem
cp -r /lando/certs/lndo.site.crt /usr/local/share/ca-certificates/lndo.site.crt
update-ca-certificate
