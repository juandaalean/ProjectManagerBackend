# 🚀 Changelog
All notable changes to this project will be documented in this file.

##  [1.1.2] - 2026-06-09

### 🆕 Added
- Sprint management functionality with dedicated endpoints and persistence.
- ProjectState enum to track the lifecycle state of projects across DTOs, services, and the database.
- StartAt property to TaskItem and its related DTOs, services, and database migration.
- Task assignment feature and related DTOs to support assigning users to tasks.
- New endpoint for listing users to support administrative and collaboration flows.

### ✅ Changed
- Increased maximum length for task descriptions and updated the database schema to use `text` type.
- Updated CORS policy to use the correct frontend/localhost URL for development.
- Improved connection string handling and added normalization for PostgreSQL (Neon-compatible).
- Included the missing ProjectManager.Tests project in the solution configuration.
- Updated README to document Sprints API endpoints and overall improvements.

### 🧰 Fixed
- Fixed formatting of Sprints API endpoints for better readability in README.
- Fixed CORS policy to the correct localhost URL.
- Fixed permissions to write on a pull request.
- Fixed filename for test results in the CodeCoverageSummary action.
- Fixed formatting of filename in the CodeCoverageSummary action.
- Fixed Neon database connection string configuration.

### 🔧 Infrastructure
- Improved CI coverage reporting and pull request workflow.
- Code coverage summary now published correctly on pull requests.

---

##  [1.0.0] - 2026-05-19

### 🆕 Added
- JWT-based authentication with register and login endpoints.
- Role-based authorization for protected resources.
- Clean Architecture structure with separate Domain, Application, Infrastructure, Security, and API layers.
- Full project management support, including create, update, delete, list, and member administration.
- Shared projects with user membership management and role assignment.
- Task management with project-scoped task listing, task creation, update, deletion, and assignee management.
- Task states and priorities to support workflow tracking.
- Task comments with list, create, update, and delete operations.
- Comment ownership rules and project role enforcement for safer collaboration.
- Request validation using FluentValidation across the API.
- Centralized exception handling for consistent API error responses.
- Swagger/OpenAPI documentation for interactive API exploration.
- PostgreSQL persistence with EF Core configurations and migrations.
- Docker-based deployment support.
- Neon-compatible production database configuration.
- Automated tests covering controllers and services.

### ✅ Changed
- Strengthened authorization checks for project-scoped resources.
- Improved API structure and separation of concerns to support long-term maintainability.
- Added CORS support to allow frontend integration during development.

### 🧰 Fixed
- Refined comment permissions so only the comment author can edit comments.
- Refined comment deletion rules so only the author with Admin project role can delete comments.
- Improved access control for tasks and task comments so only project owners or active project members can access them.

### 🔓 Security
- Enforced authenticated access through JWT bearer tokens.
- Restricted sensitive operations through role-based and ownership-based authorization.
- Reduced unauthorized access risk by validating project membership before allowing access to scoped resources.
