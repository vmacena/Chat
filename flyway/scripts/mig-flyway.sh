#!/bin/bash

ROOT_DIR=$PWD
PROJECT_DIR="$ROOT_DIR/src/Chat.Core"
MIGRATIONS_DIR="$ROOT_DIR/flyway/sql"
DB_CONTEXT="ChatDbContext"

cd "$PROJECT_DIR" || { echo "Project directory not found! Exiting."; exit 1; }

echo "Listing all migrations..."
MIGRATIONS=$(dotnet ef migrations list --context "$DB_CONTEXT")

if [ $? -ne 0 ]; then
    echo "Failed to list migrations. Please check the error messages above."
    exit 1
fi

if [ ! -e $MIGRATIONS_DIR ]; then
  mkdir -p $MIGRATIONS_DIR
fi

MIGRATION_NAMES=$(echo "$MIGRATIONS" | grep -E "[0-9]+_\S*" -o)
PREVIOUS_VERSION="0"

echo $MIGRATION_NAMES

for MIGRATION in $MIGRATION_NAMES; do
    FLYWAY_MIGRATION_NAME=$(echo "$MIGRATION" | sed 's/_/__/g')
    OUTPUT_FILE="$MIGRATIONS_DIR/V$MIGRATION__${FLYWAY_MIGRATION_NAME}.sql"
    
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

    sed -i 's/\b[A-Z][a-zA-Z]*\b/\L&/g' "$OUTPUT_FILE"

    PREVIOUS_VERSION="$MIGRATION"
done

REVERSED_MIGRATION_NAMES=""
for MIGRATION in $MIGRATION_NAMES; do
    REVERSED_MIGRATION_NAMES="$MIGRATION $REVERSED_MIGRATION_NAMES"
done

for MIGRATION in $REVERSED_MIGRATION_NAMES; do
    FLYWAY_MIGRATION_NAME=$(echo "$MIGRATION" | sed 's/_/__/g')
    OUTPUT_FILE="$MIGRATIONS_DIR/R$MIGRATION__${FLYWAY_MIGRATION_NAME}.sql"

    if [ -e $OUTPUT_FILE ]; then
        PREVIOUS_VERSION="$MIGRATION"
        continue
    fi
    
    echo "Generating reverse migration script for $MIGRATION..."
    dotnet ef migrations script --idempotent --context "$DB_CONTEXT" "$PREVIOUS_VERSION" "$MIGRATION" -o "$OUTPUT_FILE"

    if [ $? -eq 0 ]; then
        echo "Migration script for $MIGRATION generated successfully and saved to $OUTPUT_FILE"
    else
        echo "Failed to generate migration script for $MIGRATION. Please check the error messages above."
        exit 1
    fi

    sed -i 's/\b[A-Z][a-zA-Z]*\b/\L&/g' "$OUTPUT_FILE"


    PREVIOUS_VERSION="$MIGRATION"
done

echo "All migration scripts generated successfully."