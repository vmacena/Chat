# Chat Project

## Running the Application

```bash
dotnet run --project src/Chat.API
```

## Database Migrations

- Add a new conversation migration:
    ```bash
    sh flyway/scripts/mig-ef.sh addConversation
    ```
- Run Flyway migrations:
    ```bash
    sh flyway/scripts/mig-flyway.sh
    ```
- Run local migrations:
    ```bash
    sh flyway/scripts/mig-local.sh
    ```