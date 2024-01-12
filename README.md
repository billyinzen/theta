# Project Theta

## Database Migrations

The Theta.Data project contains data-layer entities, repositories, and the database context.  This is separate from the Theta.Api startup project, and so necessitates the inclusion of additional switches in the EF Core CLI:

### Add a migration

```shell
dotnet ef migrations add MIGRATION_NAME -s ./Theta.Api/ -p ./Theta.Data/ -o ./Context/Migrations
```

### Remove the last migration

```shell
dotnet ef migrations remove -s ./Theta.Api/ -p ./Theta.Data/
```

### Apply migrations to database

```shell
dotnet ef database update -s ./Theta.Api/ -p ./Theta.Data/
```

### Clear all migrations from database

```shell
dotnet ef database update 0 -s ./Theta.Api/ -p ./Theta.Data/
```
