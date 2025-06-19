run
dotnet run --project src/Chat.API

migrations
sh flyway/scripts/mig-ef.sh addConversation
sh flyway/scripts/mig-flyway.sh
sh flyway/scripts/mig-local.sh