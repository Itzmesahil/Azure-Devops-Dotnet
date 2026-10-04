# .NET DevOps Visitor Wall — Azure DevOps + Azure App Service + MySQL

A production-style learning project built to practice **Azure DevOps CI/CD with a real .NET 8 application and a MySQL database**.

The application provides a polished visitor-wall experience: anyone can enter their name, check in, and immediately see the visitor list and live statistics.

---

## 1. Project Overview

### What this project demonstrates

- ASP.NET Core 8 Web API + static frontend
- Responsive interactive UI
- Entity Framework Core
- Pomelo MySQL provider
- Azure App Service
- Azure Database for MySQL Flexible Server
- Azure DevOps YAML pipelines
- CI/CD stages
- Pipeline artifacts
- Azure DevOps Variable Groups
- Secret pipeline variables
- App Service application settings
- Cloud Shell / MySQL administration
- Automated application deployment
- Basic application health endpoint

### High-level architecture

```text
                         Developer
                             |
                             | git push
                             v
                    Azure Repos / Git
                             |
                             v
                  +---------------------+
                  | Azure DevOps YAML    |
                  |       Pipeline      |
                  +----------+----------+
                             |
                 +-----------v-----------+
                 |          CI           |
                 | Restore               |
                 | Build                 |
                 | Test                  |
                 | Publish               |
                 | Pipeline Artifact     |
                 +-----------+-----------+
                             |
                             v
                 +---------------------+
                 |          CD           |
                 | Download Artifact     |
                 | Configure App Settings|
                 | Deploy App Service    |
                 +-----------+-----------+
                             |
                             v
                 +---------------------+
                 | Azure App Service    |
                 | Linux / .NET 8       |
                 +-----------+----------+
                             |
                         EF Core
                             |
                         Pomelo
                             |
                             v
                 +---------------------+
                 | Azure Database for   |
                 | MySQL Flexible Server|
                 |      visitordb       |
                 +-----------+----------+
                             |
                             v
                         Visitors
```

---

# 2. Application Features

## Visitor Check-in

A user enters their name and selects **Check In**.

The browser sends:

```http
POST /api/visitors
```

The application validates the name and stores:

- `Name`
- `VisitedAtUtc`

in MySQL.

## Visitor Wall

The application loads:

```http
GET /api/visitors
```

and displays the latest visitors.

The UI supports:

- All visitors
- Today's visitors
- Search
- Relative timestamps
- Local date/time
- Visitor initials
- Responsive layout

## Live Statistics

The application provides:

```http
GET /api/visitors/stats
```

Example response:

```json
{
  "total": 10,
  "today": 3,
  "latestVisitor": "Sahil Shaikh"
}
```

The UI uses this to display visitor metrics.

## Application Health

```http
GET /api/health
```

Example:

```json
{
  "status": "healthy",
  "application": "DotNet DevOps Demo",
  "version": "3.0.0"
}
```

This endpoint can later be used by:

- Azure App Service health checks
- Load balancers
- Monitoring systems
- Availability tests
- CI/CD smoke tests

## Application Information

```http
GET /api/info
```

Returns application version, environment and .NET runtime information.

---

# 3. Technology Stack

| Layer | Technology |
|---|---|
| Frontend | HTML, CSS, JavaScript |
| Backend | ASP.NET Core 8 |
| Language | C# |
| ORM | Entity Framework Core |
| MySQL Provider | Pomelo.EntityFrameworkCore.MySql |
| Database | Azure Database for MySQL Flexible Server |
| Hosting | Azure App Service Linux |
| Source Control | Azure Repos |
| CI/CD | Azure DevOps YAML |
| Build Agent | Microsoft-hosted Ubuntu |
| Artifact | Azure Pipeline Artifact |

---

# 4. Repository Structure

```text
.
├── Data/
│   └── AppDbContext.cs
│
├── Models/
│   └── Visitor.cs
│
├── Tests/
│   ├── DotNetDevOpsDemo.Tests.csproj
│   └── HealthTests.cs
│
├── wwwroot/
│   └── index.html
│
├── Program.cs
├── DotNetDevOpsDemo.csproj
├── appsettings.json
└── README.md
```

### Important files

### `Program.cs`

Contains:

- ASP.NET Core application configuration
- MySQL connection configuration
- API endpoints
- Visitor validation
- Visitor database operations
- Health endpoint
- Info endpoint
- Statistics endpoint
- Static frontend configuration

### `Data/AppDbContext.cs`

Entity Framework Core database context.

### `Models/Visitor.cs`

Represents the visitor database entity.

### `wwwroot/index.html`

Contains the interactive visitor-wall frontend.

### `Tests/`

Contains automated .NET tests used by the CI pipeline.

---

# 5. Database Design

Database:

```text
visitordb
```

Table:

```text
Visitors
```

Expected structure:

```text
Visitors
├── Id
├── Name
└── VisitedAtUtc
```

Conceptually:

```sql
Id             INT             PRIMARY KEY
Name           VARCHAR(100)    NOT NULL
VisitedAtUtc   DATETIME        NOT NULL
```

Example record:

```text
1 | Sahil Shaikh | 2026-10-04 08:30:00
```

---

# 6. Azure Database for MySQL

Example configuration used for the learning project:

```text
Server:
dotnet-mysql-server

Database:
visitordb

Admin user:
sahiladmin

MySQL:
8.4

Region:
South India

Workload:
Dev/Test

Compute:
Burstable B1ms

Storage:
20 GiB
```

Do not commit the database password to Git.

---

# 7. Creating the Database

Create an Azure Database for MySQL Flexible Server from the Azure Portal.

Create a database:

```text
visitordb
```

The application expects the database to exist.

The application does **not** automatically create the database.

---

# 8. Creating the Visitors Table

For the initial learning setup, the table can be created from MySQL.

Connect using the MySQL client:

```bash
mysql -h dotnet-mysql-server.mysql.database.azure.com \
-u sahiladmin \
-p \
--ssl=true
```

Select the database:

```sql
USE visitordb;
```

Create the table:

```sql
CREATE TABLE Visitors (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    VisitedAtUtc DATETIME NOT NULL
);
```

Verify:

```sql
SHOW TABLES;
```

Expected:

```text
Visitors
```

Verify the structure:

```sql
DESCRIBE Visitors;
```

---

# 9. Application Configuration

The application intentionally does not store database credentials in source code.

The application reads these environment variables:

```text
MYSQL_SERVER
MYSQL_DATABASE
MYSQL_USERNAME
MYSQL_PASSWORD
```

Example:

```text
MYSQL_SERVER=dotnet-mysql-server.mysql.database.azure.com
MYSQL_DATABASE=visitordb
MYSQL_USERNAME=sahiladmin
MYSQL_PASSWORD=<secret>
```

The application builds the MySQL connection at runtime.

### Important

Never commit:

```text
MYSQL_PASSWORD
```

to Git.

Never place a real password inside:

- `Program.cs`
- `appsettings.json`
- YAML source
- README
- Git commits

---

# 10. Azure App Service Configuration

The target application is an Azure App Service:

```text
App Service:
dotnet-mysql

OS:
Linux

Runtime:
.NET 8

Region:
South India
```

The application settings are injected by the Azure DevOps pipeline.

The flow is:

```text
Azure DevOps Variable Group
            |
            v
      Pipeline Variables
            |
            v
AzureAppServiceSettings@1
            |
            v
Azure App Service Settings
            |
            v
.NET Environment Variables
            |
            v
          MySQL
```

---

# 11. Azure DevOps Variable Group

Create a Variable Group:

```text
Dotnet Variable Group
```

Variables:

```text
MYSQL_SERVER
MYSQL_DATABASE
MYSQL_USERNAME
MYSQL_PASSWORD
```

Recommended configuration:

```text
MYSQL_SERVER    = dotnet-mysql-server.mysql.database.azure.com
MYSQL_DATABASE  = visitordb
MYSQL_USERNAME  = sahiladmin
MYSQL_PASSWORD  = <secret>
```

Mark:

```text
MYSQL_PASSWORD
```

as a secret variable.

---

# 12. CI/CD Pipeline

The pipeline follows:

```text
Git Push
   |
   v
CI
   |
   +--> Restore
   |
   +--> Build
   |
   +--> Test
   |
   +--> Publish
   |
   +--> Pipeline Artifact
   |
   v
CD
   |
   +--> Download Artifact
   |
   +--> Configure App Settings
   |
   +--> Deploy App Service
```

---

# 13. CI Stage

The CI stage performs:

### 1. Install .NET

```yaml
- task: UseDotNet@2
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Build

```bash
dotnet build --configuration Release --no-restore
```

### 4. Test

```bash
dotnet test --configuration Release --no-restore
```

### 5. Publish

```bash
dotnet publish --configuration Release
```

The published output is placed in:

```text
$(Build.ArtifactStagingDirectory)/publish
```

### 6. Remove unnecessary test files

The published artifact should contain application deployment files, not test build output.

### 7. Publish Pipeline Artifact

```yaml
- task: PublishPipelineArtifact@1
```

Artifact name:

```text
dotnet-app
```

---

# 14. Build.ArtifactStagingDirectory

`$(Build.ArtifactStagingDirectory)` is a predefined Azure DevOps variable.

It represents a temporary staging directory on the build agent.

Example:

```text
$(Build.ArtifactStagingDirectory)/publish
```

The application is prepared there before being published as a pipeline artifact.

It is:

- not the Git repository
- not the Azure App Service
- not the MySQL server
- not permanent storage

---

# 15. Pipeline Artifact

The CI stage publishes:

```text
dotnet-app
```

The CD stage downloads it.

Conceptually:

```text
Build Agent
    |
    | Publish
    v
Pipeline Artifact
    |
    | Download
    v
Deployment Agent
```

The artifact allows CI and CD to remain separate.

---

# 16. CD Stage

The CD stage depends on CI:

```yaml
dependsOn: CI
condition: succeeded()
```

This means:

```text
CI succeeds
     |
     v
CD starts
```

If CI fails, CD does not deploy.

---

# 17. Downloading the Artifact

The CD stage uses:

```yaml
- task: DownloadPipelineArtifact@2
```

The artifact is downloaded to:

```text
$(Pipeline.Workspace)/dotnet-app
```

`$(Pipeline.Workspace)` is different from:

```text
$(Build.ArtifactStagingDirectory)
```

### Simple distinction

```text
Build.ArtifactStagingDirectory
        |
        | prepare artifact
        v
     Artifact
        |
        | download
        v
Pipeline.Workspace
```

---

# 18. Injecting App Service Variables

The pipeline uses:

```yaml
- task: AzureAppServiceSettings@1
```

It injects:

```text
MYSQL_SERVER
MYSQL_DATABASE
MYSQL_USERNAME
MYSQL_PASSWORD
```

into the App Service.

This avoids putting the actual database password into the application source code.

---

# 19. Deploying to App Service

The pipeline uses:

```yaml
- task: AzureWebApp@1
```

Configuration:

```yaml
appType: 'webAppLinux'
```

`webAppLinux` means the target is a **Linux App Service Web App**.

It does not mean that the application must be a particular programming language.

The application itself is:

```text
ASP.NET Core 8
```

running on:

```text
Linux App Service
```

---

# 20. Azure Service Connection

The Azure DevOps pipeline uses an Azure Resource Manager service connection.

Example:

```text
Azure subscription 1
```

The service connection allows Azure DevOps to authenticate to Azure and perform deployment operations.

Conceptually:

```text
Azure DevOps
     |
     | Service Connection
     v
Azure Subscription
     |
     v
Resource Group
     |
     v
App Service
```

---

# 21. Trigger

The current learning pipeline can trigger from:

```yaml
trigger:
- dev/feature
```

So:

```text
git push
   |
   v
dev/feature
   |
   v
Azure DevOps Pipeline
```

---

# 22. Recommended Git Flow

For a more realistic DevOps workflow:

```text
feature branch
      |
      | Pull Request
      v
     dev
      |
      v
     CI
      |
      v
 Deploy DEV
      |
      v
 Validation
      |
      v
 Production Approval
      |
      v
    main
      |
      v
 Deploy PROD
```

The current project can be extended toward this architecture.

---

# 23. Pull Request Validation

Azure DevOps Branch Policies can protect `main`.

Recommended policies:

```text
Minimum reviewers:
1
```

Build validation:

```text
Dotnet-Project
```

Set the build validation requirement to:

```text
Required
```

The result is:

```text
Feature Branch
      |
      | Pull Request
      v
     main
      |
      +--> Build Validation
      |
      +--> Reviewer Approval
      |
      v
    Merge
```

A normal YAML trigger and PR Build Validation are separate concepts.

---

# 24. Local Development

Requirements:

- .NET 8 SDK
- MySQL
- Git

Clone the repository:

```bash
git clone <repository-url>
cd <repository-directory>
```

Restore:

```bash
dotnet restore
```

Build:

```bash
dotnet build
```

Run tests:

```bash
dotnet test
```

Run:

```bash
dotnet run
```

The application exposes the frontend and APIs locally.

---

# 25. Local Database Environment Variables

For local development, configure:

```text
MYSQL_SERVER
MYSQL_DATABASE
MYSQL_USERNAME
MYSQL_PASSWORD
```

Do not commit local credentials.

A safer local approach is to use:

```text
dotnet user-secrets
```

or environment variables.

---

# 26. API Reference

## Health

```http
GET /api/health
```

Purpose:

```text
Application health check
```

---

## Application Info

```http
GET /api/info
```

Purpose:

```text
Runtime and environment information
```

---

## Get Visitors

```http
GET /api/visitors
```

Purpose:

```text
Return latest visitors
```

---

## Add Visitor

```http
POST /api/visitors
Content-Type: application/json
```

Request:

```json
{
  "name": "Sahil Shaikh"
}
```

The name must:

- exist
- not be empty
- be 100 characters or fewer

---

## Visitor Statistics

```http
GET /api/visitors/stats
```

Returns:

- total visitors
- today's visitors
- latest visitor

---

# 27. Testing the API

Health:

```bash
curl https://<app-name>.azurewebsites.net/api/health
```

Visitors:

```bash
curl https://<app-name>.azurewebsites.net/api/visitors
```

Statistics:

```bash
curl https://<app-name>.azurewebsites.net/api/visitors/stats
```

Create visitor:

```bash
curl -X POST \
  https://<app-name>.azurewebsites.net/api/visitors \
  -H "Content-Type: application/json" \
  -d '{"name":"Sahil Shaikh"}'
```

---

# 28. Troubleshooting

## Application loads but visitors fail

Check:

```text
/api/health
/api/visitors
```

If health works but visitors fail, investigate the database path.

---

## `Table 'visitordb.visitors' doesn't exist`

The application can reach MySQL, but the table has not been created.

Run:

```sql
USE visitordb;

SHOW TABLES;
```

Then create:

```sql
CREATE TABLE Visitors (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    VisitedAtUtc DATETIME NOT NULL
);
```

---

## Database authentication failure

Verify:

```text
MYSQL_SERVER
MYSQL_DATABASE
MYSQL_USERNAME
MYSQL_PASSWORD
```

Check the App Service:

```text
Azure Portal
→ App Service
→ Environment variables
```

Do not print the password in pipeline logs.

---

## Pipeline succeeds but application is old

Check:

1. Correct branch was pushed.
2. Pipeline actually ran.
3. CI succeeded.
4. CD succeeded.
5. Correct App Service name is configured.
6. Browser cache is not showing an old frontend.
7. App Service restarted after deployment if required.

---

## `Unexpected end of JSON input`

This generally means the frontend expected JSON but received an empty/non-JSON error response.

Check the API directly:

```text
/api/visitors
```

Then inspect:

```text
App Service
→ Monitoring
→ Log stream
```

---

# 29. Security Notes

Never commit:

```text
MYSQL_PASSWORD
```

Never hard-code:

```text
Password=...
```

into source code.

For this learning project, the current approach is:

```text
Azure DevOps Variable Group
        |
        v
Secret Pipeline Variable
        |
        v
App Service Environment Variable
```

A more production-oriented architecture is:

```text
Azure Key Vault
       |
       v
Managed Identity
       |
       v
App Service
       |
       v
MySQL
```

---

# 30. Current DevOps Architecture vs Future Production Architecture

### Current learning architecture

```text
Azure Repos
    |
    v
Azure DevOps YAML
    |
    +--> CI
    |
    +--> Artifact
    |
    +--> CD
           |
           v
     App Service
           |
           v
        MySQL
```

### Future production architecture

```text
Developer
   |
   v
Pull Request
   |
   v
Branch Policy
   |
   v
CI
   |
   +--> Build
   +--> Unit Tests
   +--> Security Scan
   +--> SAST
   +--> Dependency Scan
   +--> Artifact
   |
   v
DEV Environment
   |
   v
Integration / Smoke Tests
   |
   v
Approval
   |
   v
PRODUCTION
   |
   v
Monitoring
```

---

# 31. Future Improvements

This project is intentionally designed to become a larger DevOps portfolio project.

Recommended progression:

### Phase 1 — Current

- .NET 8
- MySQL
- App Service
- Azure DevOps CI/CD
- Variable Group
- Pipeline Artifact

### Phase 2 — Database CI/CD

Introduce:

```text
EF Core migrations
```

Instead of manually creating database schema.

Target flow:

```text
Build
  |
  v
Test
  |
  v
EF Migration
  |
  v
Artifact
```

### Phase 3 — Secrets

Move database credentials to:

```text
Azure Key Vault
```

Use:

```text
Managed Identity
```

to access secrets.

### Phase 4 — Environments

Create:

```text
DEV
QA
PRODUCTION
```

with environment-specific configuration.

### Phase 5 — Approvals

Add:

```text
Production approval
```

before production deployment.

### Phase 6 — Deployment Slots

When the App Service plan supports slots:

```text
Production
     |
     +---- Staging
```

Deploy to staging first:

```text
CI
 |
 v
Staging
 |
 v
Validation
 |
 v
Approval
 |
 v
Swap
 |
 v
Production
```

### Phase 7 — DevSecOps

Add:

```text
Trivy
SonarQube
SAST
Dependency scanning
SBOM
Container scanning
```

### Phase 8 — Monitoring

Add:

```text
Azure Monitor
Log Analytics
Application Insights
Alerts
```

---

# 32. Useful DevOps Concepts Practiced

This project provides practical examples of:

```text
Repository
   ↓
Branch
   ↓
Trigger
   ↓
Pipeline
   ↓
Stage
   ↓
Job
   ↓
Step
   ↓
Task / Script
   ↓
Artifact
   ↓
Deployment
```

Important Azure DevOps concepts:

- YAML pipelines
- Stages
- Jobs
- Steps
- Tasks
- Scripts
- Variables
- Variable Groups
- Secret variables
- Pipeline artifacts
- Service connections
- Branch policies
- Build validation
- Pull requests
- CI
- CD
- `dependsOn`
- Environment-based deployment

---

# 33. Portfolio Value

This is more representative of a DevOps Engineer project than a simple "Hello World" deployment.

It demonstrates:

```text
Application
+
Database
+
Cloud Hosting
+
CI/CD
+
Secrets
+
Testing
+
Artifacts
+
Deployment
+
Monitoring-ready APIs
```

A strong resume description could be:

> Built and deployed a .NET 8 database-backed visitor application on Azure App Service with Azure Database for MySQL, implementing Azure DevOps YAML CI/CD, automated build/test/artifact workflows, secure Variable Group-based configuration, and production-oriented application health and monitoring endpoints.

---

# 34. Final Deployment Checklist

Before considering the project complete:

- [ ] Azure Repos repository created
- [ ] `dev/feature` branch configured
- [ ] Application builds successfully
- [ ] Unit tests pass
- [ ] MySQL Flexible Server created
- [ ] `visitordb` created
- [ ] `Visitors` table created
- [ ] App Service created
- [ ] Variable Group created
- [ ] Password marked secret
- [ ] Azure service connection configured
- [ ] CI stage succeeds
- [ ] Artifact published
- [ ] CD downloads artifact
- [ ] App Service settings injected
- [ ] Application deployed
- [ ] `/api/health` returns healthy
- [ ] Visitor can be created
- [ ] Visitor appears in the visitor wall
- [ ] Visitor statistics update
- [ ] Existing visitors persist after deployment

---

## Project Status

```text
Application       ✅
Frontend          ✅
.NET 8            ✅
MySQL             ✅
Database          ✅
CI Pipeline       ✅
Pipeline Artifact ✅
CD Pipeline       ✅
Variable Group    ✅
Secret Variable   ✅
App Service       ✅
Visitor APIs      ✅
Health API        ✅
Statistics API    ✅
Interactive UI    ✅
```

### Next major DevOps milestone

**Replace manual database schema creation with EF Core migrations executed through the Azure DevOps pipeline.**
