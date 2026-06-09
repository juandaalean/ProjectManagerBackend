# Project Manager Backend

A production-style REST API for a collaborative project management platform, built with **.NET 8** and **ASP.NET Core**, and designed around **Clean Architecture** principles. The service powers projects, tasks, sprints, and team discussions for a frontend client, with a clear separation of concerns, JWT-based authentication, and role-aware authorization across shared resources.

> **Status:** Portfolio project · v1.1.2 · actively maintained on the `dev` branch.

---

## Table of Contents

- [Highlights](#highlights)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Features](#features)
- [Authorization Model](#authorization-model)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Local Setup](#local-setup)
  - [Database & Migrations](#database--migrations)
  - [Docker](#docker)
  - [Deployment (Neon)](#deployment-neon)
- [Configuration](#configuration)
- [Running the Tests](#running-the-tests)
- [CI & Code Coverage](#ci--code-coverage)
- [API Reference](#api-reference)
- [Project Structure](#project-structure)
- [AI-Assisted Development](#ai-assisted-development)
- [Roadmap](#roadmap)
- [What I Learned](#what-i-learned)
- [Contributing](#contributing)
- [License](#license)

---

## Highlights

- **Clean Architecture** with explicit `Domain`, `Application`, `Infrastructure`, `Security`, and `ProjectManagerAPI` projects.
- **JWT authentication** with role-based access control on project resources.
- **Project membership** model that gates access to tasks, sprints, and comments.
- **Sprint board** endpoint that returns sprints and their tasks, plus the project backlog, in a single payload.
- **Cross-project task query** to fetch tasks for multiple projects in one call.
- **Container-ready**: multi-stage Dockerfile and `docker-compose.yml` for production.
- **Serverless-ready**: works out of the box with [Neon](https://neon.tech) as a managed PostgreSQL provider.
- **Tested**: xUnit + Moq unit tests for every service and controller, with coverage reported on every PR.

---

## Tech Stack

| Layer            | Technology                                                                 |
|------------------|----------------------------------------------------------------------------|
| Runtime          | .NET 8 (`net8.0`)                                                          |
| Web framework    | ASP.NET Core, MVC Controllers, Swagger / Swashbuckle                        |
| Data access      | Entity Framework Core 8                                                    |
| Database         | PostgreSQL 12+ (Neon-compatible)                                           |
| Validation       | FluentValidation                                                           |
| Auth             | ASP.NET Identity `PasswordHasher`, custom JWT token service                |
| Testing          | xUnit, Moq, FluentAssertions                                               |
| Coverage         | coverlet + ReportGenerator + CodeCoverageSummary (PR comment bot)          |
| Containerization | Docker, Docker Compose                                                     |
| CI               | GitHub Actions (Ubuntu, .NET 8.0.x)                                        |

---

## Architecture

The solution is split into five projects following **Clean Architecture / Onion Architecture** conventions. Dependencies always point inward toward `Domain`.

**Layer responsibilities:**

- **Domain** – Pure C# entities (`User`, `Project`, `TaskItem`, `Comment`, `UserProject`, `Sprint`), enums, and repository abstractions. No external dependencies.
- **Application** – Use cases implemented as services, DTOs, FluentValidation validators, and domain exceptions.
- **Infrastructure** – EF Core `DbContext`, entity configurations, migrations, and concrete repository implementations.
- **Security** – `JwtTokenService` and ASP.NET Identity `PasswordHasher` factored out for reuse and testing.
- **ProjectManagerAPI** – Composition root, controllers, middleware (`ExceptionHandlingMiddleware`, CORS), Swagger, JWT options, and auth setup.

---

## Features

### Authentication & Authorization
- Email/password registration and login, returning a signed JWT.
- Stateless auth via `Authorization: Bearer {token}`.
- Acting user resolved from the `NameIdentifier` claim on every request.

### Projects
- Create, update, delete, and list projects.
- Project membership with `Admin` and `Member` roles.
- Add, remove, and update the role of a project member.

### Tasks
- CRUD over tasks scoped to a project.
- Task assignment to any project member.
- Filtered listing (`state`, `priority`, `assignee`, `sprint`, search term).
- Cross-project task query: `GET /api/task-items/by-projects` returns tasks grouped by project.

### Sprints
- Full CRUD on sprints within a project.
- **Sprint board** view: `GET /api/projects/{id}/sprints/board` returns every sprint with its tasks plus the project backlog.
- Move tasks in and out of sprints; deleting a sprint pushes its tasks back to the backlog.

### Comments
- CRUD on task comments scoped to a project/task.
- Authorization-aware: edit author-only, delete author + project `Admin`.

### User profile
- `GET /api/users/me/stats` returns the current user's project count, plan, and project limit.

### Cross-cutting
- Centralized exception handling that emits `application/problem+json` responses.
- FluentValidation auto-validation for request DTOs.
- CORS preconfigured for the local Vercel-style frontend (`http://localhost:5173`) and the deployed frontend on Vercel.

---

## Authorization Model

The API derives the acting user from the JWT `NameIdentifier` claim on every protected request. Project-scoped resources follow a layered model:

- **Access gate (tasks, sprints, comments):** the actor must be either the project owner or an active member of the project.
- **Task comment write rules:**
  - **Edit** → only the comment author.
  - **Delete** → only the comment author **and** a project member with the `Admin` role.
- **Project member management** → restricted to the project owner (and admins where applicable).

Unauthorized access throws domain exceptions (`NotFoundException`, `UnauthorizedException`, `ForbiddenException`) that the `ExceptionHandlingMiddleware` translates into proper HTTP responses.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/) (or a [Neon](https://neon.tech) connection string)
- [Docker](https://www.docker.com/) (optional, for containerized runs)

### Local Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/juandaalean/ProjectManagerBackend.git
   cd ProjectManagerBackend
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore ProjectManagerAPI.sln
   ```

3. **Configure your environment**

   Copy `.env.example` to `.env` and provide your PostgreSQL connection string:

   ```env
   ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=project_manager;Username=postgres;Password=postgres"
   ```

   The API also reads `appsettings.json` for `Jwt:*` settings. **Never commit real secrets.**

4. **Apply migrations**

   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/ProjectManagerAPI
   ```

5. **Run the API**

   ```bash
   dotnet run --project src/ProjectManagerAPI
   ```

   The API will be available at `https://localhost:5001` and Swagger UI at `https://localhost:5001/swagger`.

### Database & Migrations

- **Add a migration** (after entity changes):

  ```bash
  dotnet ef migrations add <MigrationName> --project src/Infrastructure --startup-project src/ProjectManagerAPI
  ```

- **Apply migrations** to the configured database:

  ```bash
  dotnet ef database update --project src/Infrastructure --startup-project src/ProjectManagerAPI
  ```

### Docker

The image is a multi-stage build that publishes a release build of the API and runs it on `aspnet:8.0`, exposing port `8080`.

1. Make sure `.env` contains a valid `ConnectionStrings__DefaultConnection`.
2. Build and start the container:

   ```bash
   docker compose up -d --build
   ```

3. The API will be available at `http://localhost:8081` (mapped to container port `8080`).

> The compose file does not include a database service — the container is designed to talk to an external PostgreSQL instance (Neon in production).

### Deployment (Neon)

For serverless deployments on Neon, keep the connection string outside source control and inject it as an environment variable:

```bash
export ConnectionStrings__DefaultConnection="<your-neon-connection-string>"
export ASPNETCORE_ENVIRONMENT=Production
```

The API reads the connection string through the standard ASP.NET Core configuration pipeline, so Neon works without any code changes. Apply migrations against Neon with the same `dotnet ef database update` command from the local setup.

---

## Configuration

| Key                                            | Where                  | Notes                                                                  |
|------------------------------------------------|------------------------|------------------------------------------------------------------------|
| `ConnectionStrings:DefaultConnection`          | `appsettings.json`     | Local dev PostgreSQL connection string.                                |
| `ConnectionStrings__DefaultConnection`         | Env var / Docker       | Override in production. Works with Neon.                               |
| `Jwt:Issuer`                                   | `appsettings.json`     | JWT issuer claim.                                                      |
| `Jwt:Audience`                                 | `appsettings.json`     | JWT audience claim.                                                    |
| `Jwt:SecretKey`                                | `appsettings.json`     | HMAC signing key. **Use a long, random value in production.**          |
| `Jwt:AccessTokenExpirationMinutes`             | `appsettings.json`     | Access token lifetime.                                                 |
| CORS allowed origins                           | `Program.cs`           | `http://localhost:5173` and the deployed Vercel frontend.              |

---

## Running the Tests

Unit tests cover every service and controller, using the AAA pattern, Moq for collaborators, and FluentAssertions for readability.

```bash
dotnet test ProjectManagerAPI.sln
```

To collect coverage locally:

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

---

## CI & Code Coverage

GitHub Actions runs on every push and pull request to `dev` and `main` (`.github/workflows/dotnet.yml`):

1. Restore, build, and test the solution on `ubuntu-latest` with .NET 8.0.x.
2. Generate an HTML and Markdown coverage report with **ReportGenerator**.
3. Publish coverage indicators with **CodeCoverageSummary**.
4. **Sticky PR comment bot** posts the coverage summary on every pull request and keeps it in sync across pushes.

See `.github/workflows/dotnet.yml` for the full configuration.

---

## API Reference

All routes are prefixed with `/api`. Most endpoints require `Authorization: Bearer {token}`.

### Auth

| Method | Route                | Description                                  |
|--------|----------------------|----------------------------------------------|
| POST   | `/api/auth/register` | Register a new user, returns an access token |
| POST   | `/api/auth/login`    | Log in, returns an access token              |

### Users

| Method | Route               | Description                                             |
|--------|---------------------|---------------------------------------------------------|
| GET    | `/api/users/me/stats` | Returns current user's project count, plan, and limit |

### Projects

| Method | Route                                       | Description                       |
|--------|---------------------------------------------|-----------------------------------|
| GET    | `/api/projects`                             | List projects for the current user |
| POST   | `/api/projects`                             | Create a project                   |
| PUT    | `/api/projects/{id}`                        | Update a project                   |
| DELETE | `/api/projects/{id}`                        | Delete a project                   |
| GET    | `/api/projects/{id}/members`                | List project members               |
| POST   | `/api/projects/{id}/members`                | Add a project member               |
| DELETE | `/api/projects/{id}/members/{userId}`       | Remove a project member            |
| PUT    | `/api/projects/{id}/members/{userId}/role`  | Update a member's role             |

### Tasks

| Method | Route                                                            | Description                                    |
|--------|------------------------------------------------------------------|------------------------------------------------|
| GET    | `/api/projects/{projectId}/tasks`                                | List tasks in a project (with optional filters)|
| GET    | `/api/projects/{projectId}/tasks/{taskItemId}`                   | Get a task by ID                               |
| POST   | `/api/projects/{projectId}/tasks`                                | Create a task                                  |
| PUT    | `/api/projects/{projectId}/tasks/{taskItemId}`                   | Update a task                                  |
| DELETE | `/api/projects/{projectId}/tasks/{taskItemId}`                   | Delete a task                                  |
| PUT    | `/api/projects/{projectId}/tasks/{taskItemId}/assignee`          | Assign a task to a project member              |
| GET    | `/api/task-items/by-projects`                                    | List tasks grouped by project, with filters    |

### Sprints

| Method | Route                                                                       | Description                                              |
|--------|-----------------------------------------------------------------------------|----------------------------------------------------------|
| GET    | `/api/projects/{projectId}/sprints`                                         | List sprints in a project                                |
| GET    | `/api/projects/{projectId}/sprints/board`                                   | Sprint board: sprints with tasks, plus the backlog       |
| GET    | `/api/projects/{projectId}/sprints/{sprintId}`                              | Get a sprint by ID                                       |
| GET    | `/api/projects/{projectId}/sprints/{sprintId}/tasks`                        | Get a sprint with its tasks                              |
| POST   | `/api/projects/{projectId}/sprints`                                         | Create a sprint                                          |
| PUT    | `/api/projects/{projectId}/sprints/{sprintId}`                              | Update a sprint                                          |
| DELETE | `/api/projects/{projectId}/sprints/{sprintId}`                              | Delete a sprint (tasks move to the backlog)              |
| PUT    | `/api/projects/{projectId}/sprints/{sprintId}/tasks/{taskItemId}`           | Move a task into a sprint                                |
| DELETE | `/api/projects/{projectId}/sprints/{sprintId}/tasks/{taskItemId}`           | Move a task out of a sprint (back to the backlog)        |

### Task Comments

| Method | Route                                                                                | Description                              |
|--------|--------------------------------------------------------------------------------------|------------------------------------------|
| GET    | `/api/projects/{projectId}/tasks/{taskItemId}/comments`                              | List comments on a task                  |
| POST   | `/api/projects/{projectId}/tasks/{taskItemId}/comments`                              | Add a comment                            |
| PUT    | `/api/projects/{projectId}/tasks/{taskItemId}/comments/{commentId}`                  | Update a comment (author only)           |
| DELETE | `/api/projects/{projectId}/tasks/{taskItemId}/comments/{commentId}`                  | Delete a comment (author + Admin)        |

---

## Project Structure

```
ProjectManagerBackend/
├── src/
│   ├── Domain/                 # Entities, enums, repository abstractions
│   ├── Application/            # DTOs, services, validators, exceptions
│   ├── Infrastructure/         # EF Core DbContext, configurations, repositories, migrations
│   ├── Security/               # JWT token service, password hashing
│   └── ProjectManagerAPI/      # ASP.NET Core Web API (controllers, Program.cs, options, middleware)
├── tests/
│   └── ProjectManager.Tests/   # xUnit unit tests for services and controllers
├── .github/workflows/          # CI: build, test, coverage report, PR comment bot
├── docker-compose.yml
├── Dockerfile
├── ProjectManagerAPI.sln
└── README.md
```

---

## AI-Assisted Development

This project was intentionally built with AI tools woven into the day-to-day workflow, not as an afterthought. The goal was to **move faster and learn deeper**, treating AI as a co-pilot for architecture decisions, refactors, and documentation rather than as a black box.

Tools and approaches explored:

- **Microsoft Learn MCP Server** — for canonical .NET and ASP.NET Core references.
- **GitHub MCP Server** — to automate PR creation and review hygiene.
- **OpenCode-driven experimentation** — to iterate on architecture, refactors, and tests faster.

The focus areas were:

- Faster iteration cycles on new features.
- Better understanding of Clean Architecture boundaries.
- Guided learning on EF Core modeling and authorization patterns.

---

## Roadmap

- **Audit log / history** — track who did what and when inside a project.
- **Frontend** — auth, projects, tasks, and comments client.
- **CORS + HTTPS hardening** — production-grade CORS policy and HTTPS termination.
- **WebLLM** — small AI feature for task automatization using open source web models (in Frontend). 
- **Production deployment** — Render (API) + Neon (DB) pipeline.

---

## What I Learned

- Applying **Clean Architecture** in a real-world backend, not just a toy project: keeping `Domain` free of external dependencies and pushing I/O to the edges.
- Designing **authorization for shared resources** — owner vs. member vs. role, and layering rules (e.g. comment edit/delete).
- Modeling **agile concepts** (sprints, backlog, task board) on top of relational data with EF Core.
- Validating inputs consistently with **FluentValidation** and returning `application/problem+json` errors.
- Keeping tests maintainable: **xUnit + Moq + AAA** at the service and controller level.
- Treating AI as a workflow tool, with deliberate choices about when to trust it and when to verify.

---

## Contributing

This is a portfolio project, but contributions are welcome. To get started:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/your-feature`.
3. Commit your changes: `git commit -am 'Add some feature'`.
4. Push the branch: `git push origin feature/your-feature`.
5. Open a Pull Request against `dev`.

Please make sure `dotnet build` and `dotnet test` pass before requesting a review.

---

## License

This project is released for portfolio and educational purposes. You are free to read, learn from, and adapt it. If you plan to redistribute or build on top of it, please add appropriate attribution.

> _Last reviewed: v1.1.2_
