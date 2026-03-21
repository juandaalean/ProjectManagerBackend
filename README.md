# Project Manager Backend

A robust and scalable backend API built with .NET 8 and ASP.NET Core for managing projects, users, tasks, and comments. This project follows a clean architecture pattern with layered separation of concerns, ensuring maintainability and extensibility.

## Features

- **Project Management**: Create, read, update, and delete projects.
- **User Management**: Handle user entities and roles within projects.
- **Task Tracking**: Manage tasks with priorities and states.
- **Comments**: Add comments to projects or tasks.
- **RESTful API**: Fully documented API with Swagger/OpenAPI support.
- **Database Integration**: Uses Entity Framework Core with PostgreSQL for data persistence.
- **Exception Handling**: Custom middleware for consistent error responses.
- **Dependency Injection**: Clean separation of application, infrastructure, and domain layers.

## Tech Stack

- **Framework**: .NET 8.0
- **Web Framework**: ASP.NET Core
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL
- **API Documentation**: Swagger/OpenAPI
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API layers)

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (version 12 or later)
- A code editor like Visual Studio Code or Visual Studio

## Installation

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd ProjectManagerBackend
   ```

2. **Restore NuGet packages**:
   ```bash
   dotnet restore ProjectManagerAPI.sln
   ```

3. **Set up the database**:
   - Ensure PostgreSQL is running.
   - Create a database named `ProjectManagerDB`.
   - Update the connection string in `src/ProjectManagerAPI/appsettings.json` or set environment variables for security.

4. **Run database migrations**:
   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/ProjectManagerAPI
   ```

5. **Build the solution**:
   ```bash
   dotnet build ProjectManagerAPI.sln
   ```

## Usage

1. **Run the application**:
   ```bash
   dotnet run --project src/ProjectManagerAPI
   ```

2. **Access the API**:
   - The API will be available at `https://localhost:5001` (or `http://localhost:5000`).
   - Swagger documentation: `https://localhost:5001/swagger` (in Development environment).

3. **API Endpoints**:
   - `GET /api/projects` - Retrieve a list of projects for the current user.
   - `POST /api/projects` - Create a new project.
   - `PUT /api/projects/{id}` - Update an existing project.
   - `DELETE /api/projects/{id}` - Delete a project.

   Note: Authentication is currently mocked with a hardcoded user ID. Implement proper authentication (e.g., JWT) for production use.

## Project Structure

```
src/
├── Domain/                 # Domain entities, enums, and business rules
│   ├── Entities/           # Core business entities (Project, User, TaskItem, etc.)
│   └── Enum/               # Enumerations (TaskPriority, TaskState, UserRol)
├── Application/            # Application layer with services and DTOs
│   ├── Services/           # Business logic services
│   ├── DTOs/               # Data Transfer Objects
│   ├── Exceptions/         # Custom exceptions
│   └── DependencyInjection/ # Service registrations
├── Infrastructure/         # Infrastructure concerns (EF Core, repositories)
│   ├── Data/               # DbContext and configurations
│   ├── Repositories/       # Data access implementations
│   └── Migrations/         # Database migrations
└── ProjectManagerAPI/      # ASP.NET Core Web API
    ├── Controllers/        # API controllers
    ├── Program.cs          # Application entry point
    └── appsettings.json    # Configuration
```

## Configuration

- **Database**: Configure the connection string in `appsettings.json` or via environment variables.
- **Environment Variables**: For production, use environment variables for sensitive data like database credentials.
- **Logging**: Configured via `appsettings.json` with different levels for development and production.

## Development

- **Build**: `dotnet build ProjectManagerAPI.sln`
- **Test**: No tests implemented yet. Add unit and integration tests in a future `Tests` project.
- **Migrations**: Add new migrations with `dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/ProjectManagerAPI`

## Development Tools and Automation

This project leverages advanced development tools and automation to enhance productivity, code quality, and collaboration:

### Agents and Skills
- **AI Agents**: The project uses specialized AI agents (e.g., GitHub Copilot) configured with custom skills for .NET best practices, design pattern reviews, and effective README crafting. These agents assist in code generation, refactoring, and ensuring adherence to project standards.
- **Skill-Based Guidance**: Skills are stored in `.agents/skills/` and provide domain-specific knowledge. For example:
  - `dotnet-best-practices`: Ensures code meets .NET standards.
  - `dotnet-design-pattern-review`: Reviews and suggests improvements for design patterns.
  - `crafting-effective-readmes`: Guides the creation of professional documentation.

### MCP Servers
- **Microsoft Learn MCP**: Used for accessing up-to-date Microsoft documentation and code samples. This server provides authoritative content for .NET, Azure, and related technologies, ensuring accurate implementation.
- **GitHub MCP**: Facilitates repository management, including issue tracking, pull request handling, and automated code reviews. It supports delegation of tasks to Copilot for code generation and PR creation.

### GitHub Automation
- **Pull Request Automation**: Follows a standardized PR template for consistency. Agents can delegate tasks to GitHub Copilot for automated implementation and PR creation.
- **Code Reviews**: Automated reviews using Copilot to ensure code quality before human review.
- **CI/CD**: Integrated with GitHub Actions (see `.github/workflows/dotnet.yml`) for automated builds, tests, and deployments.
- **Branch Management**: Uses feature branches and follows Git flow for collaborative development.

To learn more about these tools:
- Explore the `.agents/skills/` directory for skill definitions.
- Refer to MCP documentation for server usage.
- Check GitHub repository settings and workflows for automation details.

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/YourFeature`).
3. Commit your changes (`git commit -am 'Add some feature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Create a new Pull Request.

Ensure code follows the project's best practices as defined in `.agents/skills/dotnet-best-practices/SKILL.md`.

## Acknowledgments

- Built following Clean Architecture principles.
- Inspired by domain-driven design and best practices in .NET development.</content>