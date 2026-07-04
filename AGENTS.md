# AGENTS.md — VRT.Resume

Guide for AI agents working on this codebase. Read this before making changes.

## Project summary

**VRT.Resume** is a bilingual (PL default, EN) ASP.NET Core MVC resume generator. Users sign in via OAuth (Google/GitHub), maintain a personal profile (contacts, education, work experience, skills, photo), and create multiple tailored resume variants from that data.

- **Repo:** `E:\Projects\.NET\VRT.Resume`
- **Solutions:** `VRT.Resume.sln` (classic), `VRT.Resume.slnx` (folder-organized)
- **Live DEV:** https://vrt-cv.azurewebsites.net/ (may change; testing only)
- **Owner context:** Polish-first UI; Azure App Service deployment documented in `README.md`

## Tech stack

| Component | Version / note |
|-----------|----------------|
| .NET | `net10.0` (`Directory.Build.props`) |
| ASP.NET Core | `10.0.9` |
| EF Core | `10.0.8` (pinned for SqliteWasmBlazor; SqlServer + Sqlite) |
| Blazor WASM PWA | `VRT.Resume.Pwa` — MudBlazor 9.6, SqliteWasmBlazor 0.9.1-pre |
| MediatR | `12.5.0` |
| FluentValidation | `12.1.1` |
| CSharpFunctionalExtensions | `3.7.0` |
| SkiaSharp | `4.148.0` (profile images) |
| xUnit + FluentAssertions + Respawn | integration tests |

Central package versions: `Directory.Packages.props`. Do not add version attributes to individual `PackageReference` entries.

## Solution structure

```
VRT.Resume.Mvc              ← Web UI (controllers, views, auth, middleware)
VRT.Resume.Application      ← CQRS handlers, validators, services
VRT.Resume.Persistence      ← EF Core AppDbContext, fluent configurations
VRT.Resume.Domain           ← Entities (EF-generated + *.ex.cs partials)
VRT.Resume.Resources        ← LabelResource / MessageResource (.resx, PL+EN)
VRT.Resume.Application.Tests.Unit/   ← folder name; csproj: Tests.Integration
VRT.Resume.Pwa                    ← Blazor WASM offline PWA (local profiles, browser SQLite)
```

### Reference graph

```
Mvc → Application → Persistence → Domain
Mvc → Resources
Pwa → Application, Persistence, Resources
Tests.Integration → Application, Persistence, Domain
```

### Agent language

- **Documentation, code comments, and commit messages:** English only.
- **End-user UI strings:** Polish + English via `VRT.Resume.Resources` (.resx / .pl.resx).

## Architecture patterns

### CQRS + MediatR

- **Commands** return `Result` or `Result<T>`; **queries** mostly `Result<T>`, exceptions noted below.
- Handlers are **nested** `internal sealed class` inside command/query files.
- Validators: `{Command}Validator` per command; pipeline `ValidationBehaviour` throws `ValidationException`.

### Railway-oriented errors (CSharpFunctionalExtensions)

- Expected failures → `Result.Failure(Errors.*)`, never throw.
- Chaining: `.Bind()`, `.Map()`, `.Tap()`, `.OnFailureCompensate()`.
- Localized errors in `VRT.Resume.Application/Errors.resx`.

### Handler templates

| Base class | Use for |
|------------|---------|
| `HandlerBase` | Resolves `PersonId` via `ICurrentUserService.UserId` → `UserPerson` |
| `UpsertHandlerBase<TCommand,TDomain>` | Standard create/update (id=0 → create) |
| `DeleteHandlerBase<TCommand,TDomain>` | Standard delete |

Custom handlers: `CreatePersonAccount`, `UpdatePersonData`, `ClonePersonResume`, `MergeResumeSkills`, `MergePersonDutySkills`, `UpsertProfileImage`, `SetUserCulture`.

### MVC controller pattern

- All controllers inherit `ControllerBase` (partial classes for Mediator, redirects, culture).
- `ControllerBase.Send()` wraps MediatR; catches `ValidationException` → `ModelState` + `Result.Failure`.
- **Global auth:** `AuthorizeFilter(RequireAuthenticatedUser)` — use `[AllowAnonymous]` explicitly.
- Person CRUD controllers inherit `PersonEditControllerBase` (Add, Cancel, ConfirmDelete).
- AJAX partial rendering: `AllowPartialRenderingAttribute` returns `PartialViewResult`.

### Application → Persistence (no repository layer)

Handlers use `AppDbContext` directly with LINQ. Do not introduce repositories unless explicitly requested.

### EF provider placement

- **`VRT.Resume.Persistence`** references `Microsoft.EntityFrameworkCore.Relational` only (no SqlServer/Sqlite).
- **Hosts register the provider:** `VRT.Resume.Mvc` → SqlServer + Sqlite; `VRT.Resume.Pwa` → SqliteWasm; `Tests.Integration` → SqlServer.
- Keeps SQL Server client assemblies out of the WASM bundle and avoids mixed EF provider versions at runtime.

## VRT.Resume.Pwa (Blazor WASM offline)

Branch/plan: `feature/blazor-wasm-pwa`, `plans/blazor-wasm-pwa-offline.md`.

### Architecture

- **Feature-oriented layout** (vertical-slice style): group by feature under `Features/{FeatureName}/` (pages, feature services, components). Shared shell stays in `Layout/`; composition root stays at project root (`Program.cs`, `DependencyInjection*.cs`).
- **Reuse Application unchanged** — wire PWA-specific adapters in `VRT.Resume.Pwa` (`DummyCurrentUserService`, `PwaCultureService`, `LocalProfileService`, etc.).
- **Do not run long-lived processes** unless the user asks (`dotnet run` only on request).

### Composition root

| File | Role |
|------|------|
| `Program.cs` | `InitializeSqliteWasmAsync` → `DatabaseInitializer` → `PwaCultureService` → `DummyCurrentUserService` |
| `DependencyInjection.cs` | `AddApplication()`, PWA services |
| `DependencyInjection.DbContext.cs` | `UseSqliteWasm`, `AddSqliteWasm`, `BaseHref` from `HostEnvironment.BaseAddress` |

### csproj essentials

```xml
<WasmBuildNative>true</WasmBuildNative>
<BlazorWebAssemblyLoadAllGlobalizationData>true</BlazorWebAssemblyLoadAllGlobalizationData>
<NoWarn>$(NoWarn);WASM0001</NoWarn>  <!-- SqliteWasmBlazor e_sqlite3 stub -->
```

### Runtime pitfalls (verified)

| Issue | Fix |
|-------|-----|
| `RelationalQueryCompilationContext..ctor` Method not found | Pin **all** EF Core packages to **10.0.8** in `Directory.Packages.props` (SqliteWasmBlazor 0.9.1-pre depends on EF Sqlite 10.0.8). Clean rebuild; hard-refresh browser. |
| Culture change not supported at startup | `BlazorWebAssemblyLoadAllGlobalizationData=true` (`PwaCultureService` restores pl/en from `localStorage`). |
| `mudElementRef.getBoundingClientRect` undefined | Load `_content/MudBlazor/MudBlazor.min.js` **before** `blazor.webassembly.js`; use `MudDrawer` `Variant="DrawerVariant.Persistent"` + `Breakpoint="Breakpoint.Md"`. |
| `sqlite3_config` varargs crash | SqliteWasmBlazor + `WasmBuildNative=true` (not SqliteWasmHelper9). |

### index.html (MudBlazor)

```html
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
<script src="_framework/blazor.webassembly.js"></script>
```

## Domain model

### Core entities (16)

| Entity | Role |
|--------|------|
| `Person` | Root aggregate |
| `UserPerson` | Maps external `UserId` → `PersonId` (schema: `Auth`) |
| `PersonContact`, `PersonEducation`, `PersonExperience`, `PersonSkill`, `PersonImage`, `PersonResume` | Person-owned data |
| `PersonExperienceDuty` | Responsibility under a job |
| `PersonExperienceDutySkill` | Duty ↔ skill junction |
| `ResumePersonSkill` | Resume ↔ skill (relevance, visibility, order) |
| `SkillType`, `Degree`, `EducationField`, `School` | Lookups |

### `IPersonEntity`

Marker in `VRT.Resume.Domain/Abstractions/IPersonEntity.cs`. Implemented via `*.ex.cs` partials on person-owned entities. `UpsertHandlerBase` auto-sets `PersonId`.

### Domain style

**Anemic domain** — EF Power Tools generates entities; business rules live in Application validators/handlers. Manual extensions only in `*.ex.cs`.

### Skill types enum

`SkillTypes`: `Other`, `HumanLanguage`, `Technical`, `Soft`, `Tool`. Seeded on first DB create from enum names.

## Feature inventory

### Persons (profile)

| Operation | Type | Handler base |
|-----------|------|--------------|
| CreatePersonAccount | Command | Custom |
| UpdatePersonData | Command | Custom |
| Upsert/Delete PersonContact | Command | Upsert/Delete base |
| Upsert/Delete PersonEducation | Command | Upsert/Delete base |
| Upsert/Delete PersonExperience | Command | Upsert/Delete base |
| Upsert/Delete PersonExperienceDuty | Command | Upsert/Delete base |
| Upsert/Delete PersonSkill | Command | Upsert/Delete base |
| MergePersonDutySkills | Command | Custom |
| UpsertProfileImage | Command | Custom |
| GetPersonData, contacts, education, experience, duties, skills | Query | — |
| GetProfileImage | Query | Returns `ProfileImageVM` (no Result) |

### Resumes

| Operation | Type | Notes |
|-----------|------|-------|
| Upsert/Delete PersonResume | Command | Upsert/Delete base |
| ClonePersonResume | Command | Custom deep copy |
| MergeResumeSkills | Command | Sync skill selection |
| GetResume, GetFullResume, GetResumeSkillList | Query | `Result<T>` |
| GetResumeList | Query | Returns `IEnumerable<>` — **no Result**; empty on auth fail |

### Common

- `SetUserCultureCommand`, `GetSupportedLanguagesQuery`
- `DateTimeService`, `SkiaProfileImageService` (resize uses `SKSamplingOptions` in SkiaSharp 4)

## MVC controller map

| Controller | Route prefix | Purpose |
|------------|--------------|---------|
| `HomeController` | `/` | Resume list; About is anonymous |
| `AccountController` | `/account` | OAuth login/logout/callback |
| `CultureController` | `/Culture` | Language switch |
| `ResumesController` | `/resumes` | Resume CRUD + clone |
| `PersonController` | `/person` | Profile tab shell |
| `PersonDataController` | — | Name, DOB |
| `PersonEdu/Skills/Contacts/ExpController` | — | CRUD via base |
| `PersonExpDutyController` | — | Job duties |
| `PersonExpDutySkillsController` | — | Duty skill merge |
| `PersonImageController` | — | Photo upload |
| `ResumeSkillsController` | `/ResumeSkills` | Resume skill picker |
| `ImagesController` | `/Images` | Serve profile PNG |

## Authentication

- **Cookie auth** (`VRT.Resume.AuthCookie`), 30 min sliding.
- **Providers:** Google + GitHub from `Auth:Providers` in config/user secrets.
- **Flow:** OAuth callback → `UserLoginViewModel.Create(claims)` → `CreatePersonAccountCommand` → cookie with `PersonId` claim.
- **Current user:** `CurrentUserService` → `User.AsUserLoginViewModel()?.UserId`.
- **Local dev redirect:** `https://localhost:5001/signin-google` (see `README.md`).

Config keys (Azure uses `__` nesting):

- `DbProvider`, `ConnectionStrings__ResumeData__mssql`, `ConnectionStrings__ResumeData__sqlite`
- `Auth__Providers__N__ClientId`, `ClientSecret`, `CallbackPath`

## Database

### Provider selection (`DependencyInjection.DbContext.cs`)

- `DbProvider=mssql` → SQL Server; **default** (anything else) → SQLite.
- Local default: SQLite file `..\..\resumedata.sqlite`.
- **No EF migrations** — `EnsureCreated()` + seed on startup (`UseAppDatabase` middleware).
- DbContext lifetime: **Transient** (unusual; preserve unless refactoring intentionally).

### Tests

- **SQL Server LocalDB only:** `(localdb)\mssqllocaldb;Database=Test.VRT.Resume`
- `ApplicationFixture` + Respawn 7 (uses `SqlConnection`, not connection string overload).
- Fake user: `tester@testing.me` / Tom Tester; fixed date `2020-02-03`.

## Localization

- Resources: `VRT.Resume.Resources` — `LabelResource`, `MessageResource` (+ `.pl.resx`).
- Access: `LabelNames.*.GetLabelText()`, `MsgNames.*.GetMessageText()`.
- Cultures: `pl` (default), `en`; cookie `VRT.Resume.Culture`.
- Date format forced to `yyyy-MM-dd` in `RequestCultureMiddleware`.

## Testing conventions

| Base class | When |
|------------|------|
| `CommandTestBase<T>` | Authenticated commands |
| `AnonymousQueryTestBase<T,TResult>` | No-user flows (CreatePersonAccount) |
| `DeleteCommandTestBase<T,TEntity>` | Delete scenarios |
| `QueryTestBase` | Read models |

- Collection: `[Collection(CollectionNames.Application)]` — shared DB fixture.
- Seeding: `SeedDbContext()`, `LifetimeScopeExtensions` helpers.
- Assertions: FluentAssertions + `ResultExtensions.AssertSuccess/AssertFailure`.
- **Despite folder name `Tests.Unit`, these are integration tests** requiring LocalDB.

## Build & deploy

```powershell
dotnet build VRT.Resume.sln -c Release
dotnet test VRT.Resume.sln -c Release
dotnet publish .\VRT.Resume.Mvc\VRT.Resume.Mvc.csproj -c Release -o .\deploy\web
# or: dotnet cake --target=PublishWeb
```

- Cake default target: `ExecuteBuild` → `PublishWeb` → `./deploy/web`.
- `Publish` (trimmed, MVC-only) exists but is not the default pipeline target.
- Azure steps: see `README.md` section **Publishing to Azure**.

## Naming conventions

| Element | Pattern | Example |
|---------|---------|---------|
| Command | `{Verb}{Entity}Command` | `UpsertPersonContactCommand` |
| Query | `Get{Entity}[List]Query` | `GetResumeListQuery` |
| View model | `*VM` | `ResumeFullVM` |
| DTO | `*Dto` | `SkillDto` |
| Folder | `{Feature}/{Commands\|Queries}/{UseCase}/` | `Persons/Commands/UpsertPersonContact/` |
| Domain extension | `*.ex.cs` | `PersonContact.ex.cs` |

**Verbs:** `Upsert` (create/update), `Delete`, `Merge` (sync children), `Clone`, `Update`, `Create`.

## How to add a new feature (checklist)

1. **Domain:** Add/extend entity in `VRT.Resume.Domain` (regenerate or hand-edit + `*.ex.cs` if `IPersonEntity`).
2. **Persistence:** Add `*Configuration.cs` in `Data/Configurations/`; update `AppDbContext` if new DbSet.
3. **Application:** Create folder under `Persons` or `Resumes` with Command/Query + nested Handler + Validator.
4. **Reuse** `UpsertHandlerBase` / `DeleteHandlerBase` when possible.
5. **Errors:** Add keys to `Errors.resx` if new failure cases.
6. **Mvc:** Controller action → `Send(command)` → `ToActionResult` / redirect helpers.
7. **Views:** Razor under `Views/{Controller}/`; use resource helpers for labels.
8. **Tests:** Add test class in `Application.Tests.Unit`, inherit appropriate base, seed via fixture.

## Known quirks & pitfalls

1. **No EF migrations** — schema changes rely on `EnsureCreated`; production schema drift is risky.
2. **Transient DbContext** — new instance per resolution; do not assume request-scoped sharing.
3. **Application references Persistence** — not strict Clean Architecture; accepted pattern here.
4. **Inconsistent Result on queries** — `GetResumeListQuery`, `GetProfileImageQuery` bypass `Result<T>`.
5. **Handler naming typo** — `UpsertPersonDataCommandHandler` lives inside `UpsertPersonContactCommand.cs`.
6. **Nullable reference warnings** — project uses nullable; prefer `string?` where null is valid (e.g. OAuth claims), `??` only when default is intentional.
7. **SkiaSharp 4** — use `SKSamplingOptions`, not deprecated `SKFilterQuality`.
8. **Respawn 7** — requires open `DbConnection`, not raw connection string.
9. **Build solution file** — folder contains `.sln` and `.slnx`; use `VRT.Resume.sln` for CLI.
10. **InternalsVisibleTo** — test project can access `internal` handlers via `Directory.Build.targets`.
11. **PWA EF versions** — SqliteWasmBlazor requires EF Core 10.0.8; do not mix 10.0.9 relational/sqlite assemblies in the WASM bundle.
12. **PWA providers** — SqlServer package must not flow into `VRT.Resume.Pwa` via Persistence; register providers in host projects only.

## Files to read first

| Task | Start here |
|------|------------|
| New command | `Common/Handlers/UpsertHandlerBase.cs`, nearest existing command |
| New controller action | `Controllers/ControllerBase.Mediator.cs`, similar controller |
| Auth change | `DependencyInjection.Auth.cs`, `AccountController.cs` |
| DB change | `Persistence/Data/AppDbContext.cs`, `AppDbContextExtensions.cs` |
| New test | `Fixtures/ApplicationFixture.cs`, `CommandTestBase.cs` |
| UI label | `VRT.Resume.Resources/LabelResource.resx` (+ `.pl.resx`) |
| Deploy | `README.md`, `build.cake` |
| PWA feature | `plans/blazor-wasm-pwa-offline.md`, `VRT.Resume.Pwa/Program.cs`, `AGENTS.md` → VRT.Resume.Pwa |

## Project skill (`.grok/skills/`)

Only **codebase-specific** skills belong in the repo:

| Skill | Slash | Purpose |
|-------|-------|---------|
| `vrt-resume` | `/vrt-resume` | This codebase — read `AGENTS.md` first |

Grok loads `.grok/skills/` from the repo when working in this project.

### Recommended global skills (per developer machine)

Generic workflow skills are **not** vendored in git — install once per machine:

```powershell
npx skills add elfocrash/skills@real-work -g -y
npx skills add vercel-labs/skills@find-skills -g -y
```

| Skill | Purpose |
|-------|---------|
| `real-work` | Durable plans in `plans/*.md` |
| `find-skills` | Discover/install other skills |

For Grok, also copy `real-work` to `~/.grok/skills/` (or symlink from `~/.agents/skills/real-work`).

## Agent workflow

1. Read this file (and `.grok/skills/vrt-resume/SKILL.md` when using Grok).
2. Identify layer(s) affected — prefer minimal, focused diffs.
3. Match existing patterns (nested handlers, Result, validator per command).
4. Run `dotnet build VRT.Resume.sln` before finishing; run `dotnet test` only when changes touch projects **other than** `VRT.Resume.Pwa`.
5. Do not drive-by refactor unrelated code.
6. Update this `AGENTS.md` if you discover new architectural facts worth persisting.