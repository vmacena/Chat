#!/bin/sh
FLYWAY_URL="jdbc:postgresql://$DB_HOST:$DB_PORT/$DB_NAME"
FLYWAY_USER="$DB_USERNAME"
FLYWAY_PASSWORD="$DB_PASSWORD"
flyway migrate -url="$FLYWAY_URL" -user="$FLYWAY_USER" -password="$FLYWAY_PASSWORD" -outputType=json -outOfOrder=true