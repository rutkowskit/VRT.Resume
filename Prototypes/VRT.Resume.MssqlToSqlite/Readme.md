# VRT.Resume.MssqlToSqlite

Prototype console tool: export resume data from **SQL Server** to **SQLite** using the shared `AppDbContext` model (`VRT.Resume.Persistence`).

Use cases:

- Migrate MVC/Azure (MSSQL) data into a SQLite file for local inspection or PWA import experiments
- Export a single `PersonId` (one profile graph) or the full database

Vibecoded with **Grok (xAI)**.

## Run

From the repository root:

```powershell
dotnet run --project Prototypes/VRT.Resume.MssqlToSqlite
```

Defaults come from `appsettings.json` in this folder. Override via CLI:

```powershell
dotnet run --project Prototypes/VRT.Resume.MssqlToSqlite -- `
  --mssql "Server=(localdb)\mssqllocaldb;Database=ResumeData;Trusted_Connection=True;TrustServerCertificate=true" `
  --sqlite "E:\temp\resumedata-export.sqlite" `
  --person-id 1
```

| Option | Description |
|--------|-------------|
| `--mssql` | SQL Server connection string |
| `--sqlite` | Output SQLite file path (created or overwritten) |
| `--person-id` | Optional — export one person and related rows only |
| `--keep-sqlite` | Skip deleting an existing target file (may fail on duplicate keys) |

## Behaviour

1. Creates the SQLite schema via `EnsureCreated()` (same EF model as MVC/PWA).
2. Copies lookup tables (`SkillType`, `Degree`, `School`, `EducationField`) and person-owned data in FK order.
3. Preserves primary keys so relationships stay intact.
4. `UserPerson` rows are included (maps OAuth `UserId` → `PersonId` for PWA profile selection).

This is a **prototype** — not wired into the PWA import UI. Validate exported files before replacing production browser databases.