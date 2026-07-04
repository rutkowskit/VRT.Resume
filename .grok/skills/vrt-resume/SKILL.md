---
name: vrt-resume
description: >
  Work on the VRT.Resume .NET 10 ASP.NET Core MVC resume generator codebase.
  Use when editing VRT.Resume, adding features to this repo, fixing bugs here,
  running tests/build, deploying to Azure, or when the user mentions VRT.Resume,
  CV generator, or this solution. Always read AGENTS.md at repo root first.
---

# VRT Resume Codebase

## Before any work

1. Read **`AGENTS.md`** at the repository root — it is the canonical knowledge base.
2. Identify affected layer: Domain → Persistence → Application → Mvc → Tests.
3. Prefer existing patterns over new abstractions.

## Quick context

- **Stack:** .NET 10, MediatR CQRS, FluentValidation, CSharpFunctionalExtensions `Result`, EF Core (SQLite dev / SQL Server prod), OAuth cookie auth.
- **Build:** `dotnet build VRT.Resume.sln -c Release`
- **Test:** `dotnet test VRT.Resume.sln -c Release` (requires SQL Server LocalDB)
- **Publish:** `dotnet publish VRT.Resume.Mvc/VRT.Resume.Mvc.csproj -c Release -o ./deploy/web`

## Adding a feature (short path)

1. Entity + EF config in Domain/Persistence (if new data).
2. Command/Query + nested Handler + Validator in Application (`Persons/` or `Resumes/`).
3. Reuse `UpsertHandlerBase` / `DeleteHandlerBase` / `HandlerBase` when possible.
4. Controller action in Mvc calling `Send()`.
5. Razor view + resource strings in `VRT.Resume.Resources`.
6. Integration test in `VRT.Resume.Application.Tests.Unit/`.

## Critical rules

- **No repository layer** — handlers use `AppDbContext` directly.
- **No EF migrations** — `EnsureCreated()` only; warn user on schema-breaking changes.
- **Global auth** — `[AllowAnonymous]` required for public endpoints.
- **Transient DbContext** — do not change lifetime without explicit request.
- **Central packages** — versions in `Directory.Packages.props` only.
- **Tests are integration tests** despite `Tests.Unit` folder name.

## Common pitfalls

- Build with `VRT.Resume.sln` (not bare `dotnet build` in repo root).
- Respawn 7 needs `SqlConnection`, not connection string.
- SkiaSharp 4 uses `SKSamplingOptions` for resize.
- `GetResumeListQuery` / `GetProfileImageQuery` do not use `Result<T>`.

## Verification checklist

```powershell
dotnet build VRT.Resume.sln -c Release
dotnet test VRT.Resume.sln -c Release
```

Fix nullable warnings properly (`string?`, `??` with intentional defaults) — do not suppress without reason.

## Extended reference

Full inventory, entity map, controller routes, auth flow, Azure deploy: **`AGENTS.md`**

When you learn something new about this codebase that future agents should know, append it to `AGENTS.md`.