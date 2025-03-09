# Flyway Migration Scripts

Este repositório contém scripts para gerenciar migrações de banco de dados utilizando o Entity Framework e o Flyway.

## Requisitos

- Docker instalado e em execução
- .NET SDK instalado
- Entity Framework Core instalado globalmente

## Scripts Disponíveis

### 1. `generate-ef-migration.sh`

**Localização:** `flyway/scripts/generate-ef-migration.sh`

Este script cria uma nova migração no Entity Framework.

#### Uso:
```bash
./generate-ef-migration.sh <MIGRATION_NAME>
```

#### Parâmetros:
- `MIGRATION_NAME`: Nome da migração a ser criada.

#### Funcionamento:
1. Define o diretório do projeto.
2. Verifica se um nome de migração foi fornecido.
3. Acessa o diretório correto do projeto.
4. Executa o comando `dotnet ef migrations add` para criar a migração.

### 2. `generate-migrations.sh`

**Localização:** `flyway/scripts/generate-migrations.sh`

Este script lista todas as migrações existentes no projeto e gera scripts SQL compatíveis com o Flyway.

#### Uso:
```bash
./generate-migrations.sh
```

#### Funcionamento:
1. Define os diretórios do projeto e migrações.
2. Lista todas as migrações existentes com `dotnet ef migrations list`.
3. Cria o diretório `flyway/sql` se não existir.
4. Gera os scripts SQL das migrações usando `dotnet ef migrations script --idempotent`.
5. Gera os scripts reversos para rollback.

### 3. `local-migration.sh`

**Localização:** `flyway/scripts/local-migration.sh`

Este script executa as migrações do Flyway em um contêiner Docker.

#### Uso:
```bash
./local-migration.sh
```

#### Configuração de Ambiente:
Pode-se definir as seguintes variáveis de ambiente para configurar o banco de dados:

- `DB_HOST` (padrão: `localhost`)
- `DB_NAME` (padrão: `postgres`)
- `DB_USERNAME` (padrão: `postgres`)
- `DB_PASSWORD` (padrão: `password`)
- `DB_PORT` (padrão: `5435`)

#### Funcionamento:
1. Verifica se o Docker está instalado e em execução.
2. Obtém o diretório raiz do repositório.
3. Constrói a imagem Docker com os arquivos de migração.
4. Executa um contêiner para rodar as migrações usando o Flyway.

## Exemplo de Fluxo de Trabalho

1. Criar uma nova migração:
```bash
./generate-ef-migration.sh InitialMigration
```

2. Gerar scripts SQL das migrações:
```bash
./generate-migrations.sh
```

3. Aplicar as migrações localmente via Docker:
```bash
./local-migration.sh
```

## Autor
Este conjunto de scripts foi desenvolvido para facilitar a gestão de migrações no projeto Chat, utilizando .NET EF Core e Flyway.

