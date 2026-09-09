# TaskManagement

Initial scaffold for the TaskManagement multi-project ASP.NET Core Web API solution following a CQRS-oriented enterprise structure.

## Solution Structure

- `TaskManagement.Domain`: core domain entities, enums, and domain contracts.
- `TaskManagement.DTO`: request and response models shared across application boundaries.
- `TaskManagement.AppServices`: cross-cutting application contracts, services, and registration extensions.
- `TaskManagement.Command`: write-side CQRS handlers and command models.
- `TaskManagement.Queries`: read-side CQRS handlers and query models.
- `TaskManagement.Infrastructure`: persistence and external implementation details.
- `TaskManagement.Resources`: lightweight shared resources and utilities.
- `TaskManagement.WebAPI`: API entry point, HTTP configuration, middleware, and controllers.

## Current Scope

This branch contains only the initial solution setup:

- Multi-project solution scaffold
- Baseline project references
- Web API host project without demo endpoints
- Folder skeleton for planned feature areas
- `.gitignore` for .NET build outputs and local secrets

Feature implementation is intentionally deferred to follow-up branches such as database setup, authentication, user management, project management, task management, comments, dashboard, and testing.