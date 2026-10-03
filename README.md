# .NET DevOps GUI Demo
ASP.NET Core 8 application for Azure DevOps multi-stage YAML CI/CD practice.

Endpoints:
- `/` responsive DevOps dashboard
- `/api/health` health check
- `/api/info` application/runtime information

Pipeline:
Git → Restore → Build → Test → Publish → Artifact → Deploy
