#!/usr/bin/env bash

set -euo pipefail
IFS=$'\n\t'

# -----------------------------------------------------------
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
SOLUTION_ROOT="$PWD/src"
PROJECT_DIR="$SOLUTION_ROOT/Chat.Core"
DB_CONTEXT="ChatDbContext"
MIGRATIONS_DIR="Persistence/Migrations"
# -----------------------------------------------------------


usage() {
  echo "Uso: $(basename "$0") <NomeDaMigration>"
  echo "Example: $(basename "$0") CreateUsuarioTable"
  exit 1
}

if [ $# -ne 1 ]; then
  echo "Error: missing migration name."
  usage
fi

MIGRATION_NAME="$1"

if [ ! -d "$PROJECT_DIR" ]; then
  echo "Error: Project directory not found."
  echo "       $PROJECT_DIR"
  exit 1
fi

cd "$PROJECT_DIR"
mkdir -p "$MIGRATIONS_DIR"

echo "Adding migration '$MIGRATION_NAME' to DbContext '$DB_CONTEXT'..."
dotnet ef migrations add "$MIGRATION_NAME" \
  --output-dir "$MIGRATIONS_DIR" \
  --context "$DB_CONTEXT"

echo "Successfully added migration '$MIGRATION_NAME' to DbContext '$DB_CONTEXT'."
