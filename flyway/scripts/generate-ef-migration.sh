#!/bin/bash

set -e

ROOT_DIR=$PWD
PROJECT_DIR="$ROOT_DIR/Chat.Core"
MIGRATIONS_DIR="$ROOT_DIR/flyway/sql"
DB_CONTEXT="ChatDbContext"

cd "$PROJECT_DIR" || { echo "Project directory not found! Exiting."; exit 1; }

MIGRATIONS=$(dotnet ef migrations list --context "$DB_CONTEXT")

if [ $? -ne 0 ]; then
    echo "Failed to list migrations. Please check the error messages above."
    exit 1
fi

mkdir -p "$MIGRATIONS_DIR"

MIGRATION_NAMES=$(echo "$MIGRATIONS" | grep -E "[0-9]+_\S*" -o)
PREVIOUS_VERSION="0"

for MIGRATION in $MIGRATION_NAMES; do
    OUTPUT_FILE="$MIGRATIONS_DIR/V$MIGRATION.sql"
    
    if [ -e $OUTPUT_FILE ]; then
        PREVIOUS_VERSION="$MIGRATION"
        continue
    fi

    echo "Generating migration script for $MIGRATION..."
    dotnet ef migrations script --idempotent --context "$DB_CONTEXT" "$PREVIOUS_VERSION" "$MIGRATION" -o "$OUTPUT_FILE"

    if [ $? -eq 0 ]; then
        echo "Migration script for $MIGRATION generated successfully and saved to $OUTPUT_FILE"
    else
        echo "Failed to generate migration script for $MIGRATION. Please check the error messages above."
        exit 1
    fi

    PREVIOUS_VERSION="$MIGRATION"
done

echo "All migration scripts generated successfully."