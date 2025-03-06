#!/bin/bash

set -e

ROOT_DIR=$(git rev-parse --show-toplevel)
CONTEXT="$ROOT_DIR/flyway"
CONTAINER_NAME="flyway-migrate"

DB_HOST="localhost"
DB_NAME="postgres"
DB_USERNAME="postgres"
DB_PASSWORD="root"
DB_PORT="5435"

if ! command -v docker &> /dev/null; then
    echo "Docker is not installed. Please install Docker and try again."
    exit 1
fi

if ! docker info &> /dev/null; then
    echo "Docker daemon is not running. Please start Docker and try again."
    exit 1
fi

echo "Building Docker image..."
docker build "$CONTEXT" -t "$CONTAINER_NAME"

echo "Running Flyway migrations..."
docker run --rm --network="host" \
    -e "DB_HOST=$DB_HOST" \
    -e "DB_NAME=$DB_NAME" \
    -e "DB_USERNAME=$DB_USERNAME" \
    -e "DB_PASSWORD=$DB_PASSWORD" \
    -e "DB_PORT=$DB_PORT" \
    "$CONTAINER_NAME" sh /flyway/migrate.sh

echo "Flyway migrations completed successfully."