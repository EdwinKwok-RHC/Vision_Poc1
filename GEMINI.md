# GEMINI.md - .NET Vertical Slice Minimal API Context

## Context
- Primary language: C#
- Framework: .NET (latest LTS; prefer .NET 8+)
- ORM: Use Dapper for all database access unless specifically told to use EF Core
- API Style: Minimal API implemented with FastEndpoints (preferred over standard Minimal API templates)
- Coding paradigm: Clean code, test-driven development
- Main design patterns and architecture:
  - Repository Pattern (encapsulate all data access using repository interfaces with Dapper)
  - Factory Pattern (for instantiation of complex/configurable dependencies)
  - Dependency Injection (all services and repositories registered and consumed via DI)
  - REPR (Request-Endpoint-Response) Pattern for clear separation of concerns
  - Vertical Slice Architecture (organize by feature/slice, not traditional layer)
  - Options Pattern for settings/configuration binding
- Testing: Every endpoint, service, and repository must be covered by unit and (where possible) integration tests. Mock dependencies in unit tests.

## Project Standards
- Organize code by feature slice:
  - Each slice (e.g., `/Features/Orders`) contains request/response DTOs, endpoints, related services, and tests.
  - Every endpoint has a Request DTO, Response DTO, and logic class (inherits from FastEndpoint).

- Dapper Guidelines:
  - Write only async repository methods.
  - SQL must be parameterized.
  - Keep SQL queries in the repository and expose only methods on the interface.
  - No direct use of Dapper or SQL outside repository classes.

- FastEndpoints Usage:
  - All endpoints are FastEndpoint classes. No standard Minimal API endpoint delegates.
  - Use constructor injection for all dependencies.
  - Return strongly typed responses only.

- Options Pattern:
  - For config, define strongly-typed classes bound via `IOptions<T>`.
  - Do not use `IConfiguration` or string-based config access in business/domain code.

- General C# Best Practices:
  - Use nullable reference types, pattern matching, and latest C# syntax.
  - XML docs for all public APIs.
  - PascalCase for types/properties, camelCase for locals/fields.
  - Prefer records for immutable DTOs.
  - Services and repositories must have interfaces; register all with DI.
  - Use file-scoped namespace declaration.
  - Use explicit using.

## AI Prompt Instructions
- Adhere strictly to the context and standards above.
- Ignore or override conflicting requirements unless explicitly instructed otherwise.
- All code, pseudocode, and explanations should be context-compliant.
- If asked to use EF, confirm specific instruction before switching from Dapper.
- Always structure generated code to maximize maintainability, clarity, and testability.