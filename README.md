# VRT.Resume

Open-source CV generator (.NET 10). Maintain a personal profile (contacts, education, experience, skills, photo) and create multiple tailored resume variants.

**Repository:** [github.com/rutkowskit/VRT.Resume](https://github.com/rutkowskit/VRT.Resume)  
**Live (PWA):** [cv.rutus.work](https://cv.rutus.work)

## Active UI — Blazor WASM PWA (offline)

**`VRT.Resume.Pwa`** is the current product: local profiles, browser SQLite (OPFS), PL/EN UI (MudBlazor), no OAuth, static hosting (e.g. Cloudflare Pages).

**Full documentation:** [`VRT.Resume.Pwa/Readme.md`](VRT.Resume.Pwa/Readme.md) — run locally, publish, offline testing, Lighthouse, Cloudflare deploy, limitations.

Quick start:

```powershell
dotnet run --project VRT.Resume.Pwa/VRT.Resume.Pwa.csproj
```

## Deprecated UI — ASP.NET Core MVC

**`VRT.Resume.Mvc`** is **deprecated** and will **not** receive further development. It remains in the repository for existing deployments (OAuth Google/GitHub, SQL Server or SQLite, Azure App Service) and may get critical bugfixes or security patches only. All new features and UX work go to **`VRT.Resume.Pwa`**.

**Documentation:** [`VRT.Resume.Mvc/Readme.md`](VRT.Resume.Mvc/Readme.md) — auth setup, Azure publish, DEV environment.

## Solution

| Project | Role |
|---------|------|
| `VRT.Resume.Pwa` | Blazor WASM PWA (active UI) |
| `VRT.Resume.Mvc` | ASP.NET Core MVC (**deprecated**, no new features) |
| `VRT.Resume.Application` | CQRS handlers (MediatR), shared by both hosts |
| `VRT.Resume.Persistence` / `Domain` | EF Core data model |
| `VRT.Resume.Resources` | UI strings (PL + EN) |

Build and test the whole solution:

```powershell
dotnet build VRT.Resume.sln -c Release
dotnet test VRT.Resume.sln -c Release
```

Integration tests require SQL Server LocalDB. PWA UI tests: `dotnet test VRT.Resume.Pwa.Tests/VRT.Resume.Pwa.Tests.csproj -c Debug`.

## Further reading

- [`AGENTS.md`](AGENTS.md) — architecture, conventions, pitfalls (for contributors and AI agents)
- [`plans/blazor-wasm-pwa-offline.md`](plans/blazor-wasm-pwa-offline.md) — PWA implementation plan