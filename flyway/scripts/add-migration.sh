#!/bin/bash

set -e

SOLUTION_ROOT="$PWD/src"
PROJECT_DIR="$SOLUTION_ROOT/Chat.Core"
DB_CONTEXT="ChatDbContext"
MIGRATIONS_DIR="Persistence/Migrations"

MIGRATION_NAME=$1

if [ -z "$MIGRATION_NAME" ]; then
  echo "Migration name is missing"
  echo "Example: $0 [MIGRATION_NAME]"
  exit 1
fi

cd "$PROJECT_DIR" || { echo "Project directory not found! Exiting."; exit 1; }

echo "Adding new migration: $MIGRATION_NAME"
dotnet ef migrations add "$MIGRATION_NAME" --output-dir "$MIGRATIONS_DIR"

echo "Migration $MIGRATION_NAME added successfully."