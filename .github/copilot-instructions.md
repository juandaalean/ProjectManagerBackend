# Project Guidelines

# Project Context
This project use external skills for backend development and architecture guidance.

## Active Skills & MCP Usage
- **Primary Source:** For any backend or architecture task, ALWAYS consult the local skill at `./.agents/skills/dotnet-best-practices/SKILL.md`. These rules take precedence over any other documentation.
- **Secondary Source (MCP):** Only if the local Skill is insufficient, invoke the **Microsoft Learn MCP**.
- **Efficiency Rule:** Do not use MCP tools if the solution can be derived from the local Skill or the current workspace context. This saves tokens and ensures consistency with our project standards.

## Task Workflow
Whenever I ask you for new functionality, follow these steps:
1. **Exploration:** Review existing files to ensure consistency.

2. **Planning:** Propose changes and wait for my approval.

3. **Implementation:** Write clean, asynchronous, and well-documented code.

4. **Validation:** Suggest a test command or Test endpoint to verify the implementation.

## Code Style
- Target framework is .NET 8 with nullable reference types enabled across projects.
- Follow existing naming patterns: PascalCase for types and members, singular entity names, and primary keys named `<Entity>Id` as already used in the codebase (for example `UserId`, `ProjectId`).
- Keep namespaces and formatting consistent with nearby files instead of reformatting unrelated code.
- Domain models should stay persistence-agnostic and avoid EF Core attributes in entity classes.

## Architecture
- The solution follows a layered layout:
  - Domain: entities and enums only.
  - Application: application/use-case layer (currently minimal, expand here for services/DTOs/handlers).
  - Infrastructure: EF Core `DbContext`, entity configurations, and database/provider integrations.
  - ProjectManagerAPI: ASP.NET Core startup and dependency wiring.
- Respect dependency direction: API can reference other layers; Infrastructure depends on Domain; Domain should not depend on Infrastructure or API.
- Put EF mappings in Infrastructure configuration classes implementing `IEntityTypeConfiguration<T>` and register them through `ApplyConfigurationsFromAssembly` in `ProjectManagerContext`.

## Build and Test
- Restore packages: `dotnet restore ProjectManagerAPI.sln`
- Build solution: `dotnet build ProjectManagerAPI.sln`
- Run API locally: `dotnet run --project src/ProjectManagerAPI`
- Swagger is enabled in Development and available at `/swagger`.
- There is no test project yet. If adding tests, create a dedicated test project and run with `dotnet test`.

## Data and Environment
- PostgreSQL is configured through `ConnectionStrings:DefaultConnection` in API settings.
- Do not commit real credentials. Prefer environment variables or user secrets for connection strings and secrets.
- For schema changes, create EF Core migrations in Infrastructure and execute update using API as startup project:
  - `dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/ProjectManagerAPI`
  - `dotnet ef database update --project src/Infrastructure --startup-project src/ProjectManagerAPI`

## Conventions
- Keep entity relationships expressed with navigation properties and explicit configuration classes.
- Keep startup wiring in Program.cs focused on registration and middleware; place business logic outside startup.

## Specialized Reviewers (Skills)
- **Code Style:** Use `./.agents/skills/dotnet-best-practices/SKILL.md`.
- **Architecture & Patterns:** Use `./.agents/skills/dotnet-design-pattern-review/SKILL.md` to review any new service or complex logic.