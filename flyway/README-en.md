# Flyway Migration Scripts

This repository contains scripts to manage database migrations using Entity Framework and Flyway.

## Requirements

- Docker installed and running
- .NET SDK installed
- Entity Framework Core installed globally

## Available Scripts

### 1. `generate-ef-migration.sh`

**Location:** `flyway/scripts/generate-ef-migration.sh`

This script creates a new migration in Entity Framework.

#### Usage:
```bash
./generate-ef-migration.sh <MIGRATION_NAME>
```

#### Parameters:
- `MIGRATION_NAME`: The name of the migration to be created.

#### How It Works:
1. Sets the project directory.
2. Checks if a migration name is provided.
3. Navigates to the correct project directory.
4. Executes the `dotnet ef migrations add` command to create the migration.

### 2. `generate-migrations.sh`

**Location:** `flyway/scripts/generate-migrations.sh`

This script lists all existing migrations in the project and generates SQL scripts compatible with Flyway.

#### Usage:
```bash
./generate-migrations.sh
```

#### How It Works:
1. Defines the project and migrations directories.
2. Lists all existing migrations using `dotnet ef migrations list`.
3. Creates the `flyway/sql` directory if it does not exist.
4. Generates SQL scripts from the migrations using `dotnet ef migrations script --idempotent`.
5. Generates reverse scripts for rollback purposes.

### 3. `local-migration.sh`

**Location:** `flyway/scripts/local-migration.sh`

This script runs Flyway migrations inside a Docker container.

#### Usage:
```bash
./local-migration.sh
```

#### Environment Configuration:
The following environment variables can be set to configure the database:

- `DB_HOST` (default: `localhost`)
- `DB_NAME` (default: `postgres`)
- `DB_USERNAME` (default: `postgres`)
- `DB_PASSWORD` (default: `password`)
- `DB_PORT` (default: `5435`)

#### How It Works:
1. Checks if Docker is installed and running.
2. Retrieves the root directory of the repository.
3. Builds the Docker image containing the migration files.
4. Runs a container to apply migrations using Flyway.

## Example Workflow

1. Create a new migration:
```bash
./generate-ef-migration.sh InitialMigration
```

2. Generate SQL scripts from migrations:
```bash
./generate-migrations.sh
```

3. Apply migrations locally using Docker:
```bash
./local-migration.sh
```

## Author
This set of scripts was developed to facilitate migration management in the Chat project, using .NET EF Core and Flyway.

