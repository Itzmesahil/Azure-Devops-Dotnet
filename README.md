# .NET DevOps Demo

Simple ASP.NET Core .NET 8 application for Azure DevOps YAML CI/CD practice.

Endpoints: `/` and `/health`.

## Local
```bash
dotnet restore
dotnet build
dotnet test
dotnet run
```

## Publish
```bash
dotnet publish -c Release -o publish
```

The `publish/` directory is the deployable output and will be used to practice Pipeline Artifacts.
