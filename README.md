# Dynamic App Class Proof of Concept

Proof-of-concept web application using .NET 10, Clean Architecture, EF Core SQLite, and Angular. It models configurable "Class Types" with fields, workflow statuses, actions/transitions, and individual Class Instances.

## Folder Structure

- `src/DynamicAppClass.Domain` - core entities and workflow invariants.
- `src/DynamicAppClass.Application` - DTOs, repository interfaces, validation, and workflow orchestration.
- `src/DynamicAppClass.Infrastructure` - EF Core SQLite persistence, repository implementations, and seed data.
- `src/DynamicAppClass.Api` - ASP.NET Core REST API controllers and startup.
- `src/DynamicAppClass.Web` - Angular admin UI.

## Backend Setup

```powershell
dotnet restore
dotnet build
dotnet run --project src\DynamicAppClass.Api\DynamicAppClass.Api.csproj --urls http://localhost:5000
```

The API uses SQLite and calls `Database.EnsureCreated()` on startup. No manual migration step is required for this POC. The database file is created as `src/DynamicAppClass.Api/dynamic-app-class.db` when running from the solution root.

Seed data creates a `Support Ticket` class type with Title, Description, Priority, and Requested By fields; New, In Progress, Resolved, and Closed statuses; and Start Work, Resolve, Close, and Reopen workflow actions.

## Frontend Setup

```powershell
cd src\DynamicAppClass.Web
npm.cmd install
npm.cmd start
```

Open `http://localhost:4200`. The Angular app expects the API at `http://localhost:5000`.

## Example API Calls

```http
GET http://localhost:5000/api/class-types
GET http://localhost:5000/api/class-instances
GET http://localhost:5000/api/class-instances/{id}/available-actions
POST http://localhost:5000/api/class-instances/{id}/actions
Content-Type: application/json

{ "actionId": "{action-guid}" }
```

## Core Files

- Domain model: `src/DynamicAppClass.Domain/Entities/ClassType.cs`, `ClassField.cs`, `ClassStatus.cs`, `ClassAction.cs`, `ClassInstance.cs`.
- Workflow transition rule: `src/DynamicAppClass.Domain/Entities/ClassInstance.cs`.
- Workflow orchestration and validation: `src/DynamicAppClass.Application/Services/ClassWorkflowService.cs`.
- EF persistence and seed data: `src/DynamicAppClass.Infrastructure/Persistence/AppDbContext.cs`, `SeedData.cs`.
- REST API: `src/DynamicAppClass.Api/Controllers/ClassTypesController.cs`, `ClassInstancesController.cs`.

## Known Limitations

- Authentication, authorization, auditing, and multi-user concerns are not implemented.
- `EnsureCreated()` is used instead of versioned EF migrations to keep the POC easy to run.
- Field values are stored as strings; field type validation is intentionally basic.
- Workflow configuration can be changed after instances exist; production rules would likely restrict destructive changes.
- API base URL is hard-coded in the Angular service for local development.
