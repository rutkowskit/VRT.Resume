# VRT.Resume — Blazor WASM PWA (offline, lokalne profile)

Nowy projekt **Blazor WebAssembly PWA** z pełną funkcjonalnością obecnego generatora CV, bez OAuth i bez backendu. Dane w **SQLite WASM** (przeglądarka). **`VRT.Resume.Application` nietknięty** — Pwa jest composition root (jak MVC).

**Zamiast logowania:** użytkownik tworzy **lokalne profile** (osoby) i **wybiera aktywny kontekst**. `DummyCurrentUserService` przechowuje `UserId` wybranego profilu — `HandlerBase` w Application działa bez zmian.

## Assumptions (confirmed / defaults)

| Topic | Decyzja |
|-------|---------|
| Lokalizacja | `VRT.Resume.Pwa` w **tym samym solution** (`VRT.Resume.sln` + `.slnx`) |
| Architektura | **Jeden projekt hosta** — bez `Pwa.Infrastructure` / `Pwa.Application` |
| Storage | **SQLite WASM** + EF Core (`Microsoft.EntityFrameworkCore.Sqlite`) |
| Application | **`VRT.Resume.Application` — nietknięty** |
| Persistence | Współdzielony; ewentualne drobne poprawki pod SQLite WASM |
| „Auth” w PWA | **Lokalne profile** — tworzenie + wybór kontekstu (zastępuje OAuth) |
| Kontekst użytkownika | `DummyCurrentUserService` (**Singleton**) — mutowalny `UserId` + `localStorage` |
| Tworzenie profilu | `CreatePersonAccountCommand` (MediatR) — bez zmian w Application |
| UI | **MudBlazor 9.x** (PWA); MVC pozostaje Bootstrap + `Resume.css` |
| Zakres v1 | Pełny parytet MVC + zarządzanie profilami (create, switch; delete opcjonalnie) |
| .NET | `net10.0` |

**Open (non-blocking):**
- Usuwanie profilu (kaskada danych) — v1 czy później?
- Eksport/backup całej bazy SQLite (wszystkie profile)?

## For Future Agents

Mark checkboxes `- [x]` as complete; set phase `Status: Complete` + **Phase Summary**; run **Verification Plan** before next phase.

**Composition root (mirror MVC + profile layer):**

| MVC | PWA |
|-----|-----|
| `CurrentUserService` (cookie claims) | `DummyCurrentUserService` (**wybrany profil**) |
| `AccountController` (OAuth) | `/profiles` — lista, tworzenie, wybór kontekstu |
| — | `LocalProfileService` (Pwa only — lista profili z DB) |
| — | `ProfileContextStorage` — `localStorage` klucz `VRT.Resume.ActiveProfileUserId` |

**Nietknięte:** `VRT.Resume.Application`, `VRT.Resume.Domain`.

---

## Local profiles — architecture

```
┌─────────────────────────────────────────────────────────────┐
│  /profiles          Lista profili + „Utwórz nowy”           │
│  /profiles/create   Formularz → CreatePersonAccountCommand │
└──────────────────────────┬──────────────────────────────────┘
                           │ po wyborze / utworzeniu
                           ▼
              DummyCurrentUserService.SetUserId(userId)
              localStorage ← ActiveProfileUserId
                           │
                           ▼
              Wszystkie strony app (Home, Person, CV…)
              IMediator → HandlerBase.GetCurrentUserPersonId()
                        → UserPerson WHERE UserId = aktywny kontekst
```

**Model danych (bez zmian w Application):**

- Każdy lokalny profil = wiersz `UserPerson` (PK: `UserId`) + powiązany `Person`
- `UserId` generowany w Pwa: np. `local:{guid}` (unikalny, bez emaila)
- `CreatePersonAccountCommand`: `UserId`, `FirstName`, `LastName`; `Email` opcjonalnie null
- Wiele profili w **jednej** bazie SQLite WASM na urządzeniu

**`DummyCurrentUserService` (Pwa):**

```csharp
// Singleton — implementuje ICurrentUserService
public string? UserId { get; private set; }
public void SetContext(string userId) { ... persist localStorage ... notify UI ... }
public bool HasActiveContext => !string.IsNullOrEmpty(UserId);
```

**`LocalProfileService` (Pwa only — nie w Application):**

- `GetAllAsync()` — `UserPerson` ⋈ `Person` (imię, nazwisko, UserId)
- Nie zastępuje handlerów Application; tylko UI wyboru profilu

**Guard routingu:**

- Brak aktywnego kontekstu → redirect `/profiles`
- Menu: nazwa aktywnego profilu + link „Zmień profil”

**Seed przy starcie:**

- Tylko `SkillType` (via `InitDatabase`) — **bez** domyślnego `Person` / `UserPerson`
- Pierwszy użytkownik tworzy profil ręcznie w UI

---

## Solution sketch

```
VRT.Resume.Pwa/
  DependencyInjection.cs
  DependencyInjection.DbContext.cs
  Services/
    DummyCurrentUserService.cs      ← ICurrentUserService, aktywny kontekst
    LocalProfileService.cs        ← lista profili (AppDbContext, Pwa only)
    ProfileContextStorage.cs      ← localStorage interop
    PwaCultureService.cs
    DatabaseInitializer.cs        ← EnsureCreated + SkillType seed only
  Pages/
    Profiles.razor                ← wybór kontekstu
    ProfilesCreate.razor
    … (Home, Person, Resumes — wymagają kontekstu)
```

---

## Phase 1: Solution scaffold & PWA shell
Status: Complete

- [x] `dotnet new blazorwasm -n VRT.Resume.Pwa -o VRT.Resume.Pwa --pwa` (net10.0)
- [x] Dodaj do `VRT.Resume.slnx` (folder `/01.Client/`)
- [x] Referencje: `VRT.Resume.Application`, `VRT.Resume.Resources`
- [x] Pakiety WASM w `Directory.Packages.props` (EF/SQLite WASM — Faza 2)
- [x] Usuń `Counter`, `Weather`
- [x] Layout: **Profiles**, Resumes (Home), Person, About — Resumes/Person disabled bez kontekstu (`StubActiveProfileContext`)
- [x] **MudBlazor 9.6** — `AddMudServices()`, `MudLayout`/`MudNavMenu`, providers w `App.razor`; usunięto `wwwroot/lib/bootstrap`
- [x] `manifest.webmanifest` (VRT Resume) + service worker (szablon PWA)

### Verification Plan
- `dotnet build VRT.Resume.slnx -c Release` — 0 errors ✅
- `dotnet run --project VRT.Resume.Pwa` — start OK

### Phase Summary
Utworzono `VRT.Resume.Pwa` (Blazor WASM PWA, net10.0). Central package management: `Microsoft.AspNetCore.Components.WebAssembly` 10.0.9, `MudBlazor` 9.6.0. Solution: `VRT.Resume.slnx` (/01.Client/). Referencje do Application + Resources. UI: MudBlazor (`MudLayout`, drawer, `MudNavMenu`). Strony: `/profiles`, `/`, `/person`, `/about`. NavMenu z disabled Resumes/Person gdy brak kontekstu (`IActiveProfileContext` stub). Usunięto szablon Counter/Weather i Bootstrap z layoutu.

---

## Phase 2: Composition root — DI, DbContext, seed (w Pwa)
Status: Complete

- [x] `DependencyInjection.cs` — `AddApplication()`, `DateTimeService`, `ProfileImageService`
- [x] `DummyCurrentUserService` — **Singleton**, `ICurrentUserService` + `IActiveProfileContext`, `SetContext` / `SetContextAsync`, restore z `localStorage` przy starcie
- [x] `ProfileContextStorage` — JS interop: get/set `VRT.Resume.ActiveProfileUserId`
- [x] `LocalProfileService` — Scoped; odczyt listy profili z `AppDbContext`
- [x] `DependencyInjection.DbContext.cs` — `SqliteWasmBlazor` (OPFS + Web Worker) + `AppDbContext` Transient (jak MVC)
- [x] `DatabaseInitializer` — `InitDatabaseAsync()` (SkillType seed only); **nie** seeduj domyślnego użytkownika
- [x] Persistence pod SQLite: `UseCollation` tylko dla SQL Server; `InitDatabase` seeduje gdy `SkillType` puste (nie po `EnsureCreated()`)

### Verification Plan
- `dotnet build VRT.Resume.slnx -c Release` — 0 errors ✅
- `EnsureCreated()` + seed: `SkillType` = 5 na świeżej bazie (runtime w przeglądarce — Faza 3 UI)
- `DummyCurrentUserService.SetContext` — gotowe; test E2E w Fazie 3

### Phase Summary
Composition root w `VRT.Resume.Pwa`: MediatR (`AddApplication`), SQLite WASM (`SqliteWasmBlazor` 0.9.1-pre, plik `vrt-resume.db`, OPFS). **Nie** `SqliteWasmHelper9` / `SQLitePCLRaw` native — powoduje błąd `sqlite3_config` varargs w WASM. `DummyCurrentUserService` (Singleton) implementuje `ICurrentUserService` i `IActiveProfileContext`; `ProfileContextStorage` (Scoped, via `IServiceScopeFactory`). `LocalProfileService` — lista `UserPerson`⋈`Person`. Startup: `DatabaseInitializer` + restore kontekstu z `localStorage`. Persistence: collation warunkowa (SQLite); `InitDatabase`/`InitDatabaseAsync` seeduje `SkillType` gdy tabela pusta. Wymaga workload `wasm-tools` (`dotnet workload restore`).

---

## Phase 3: Local profiles — create, list, switch context
Status: Complete

- [x] `/profiles` — `LocalProfileService.GetAllAsync()`; karty z imieniem/nazwiskiem; przycisk **Wybierz** → `SetContext(userId)` → redirect `/`
- [x] `/profiles/create` — formularz: FirstName, LastName (opcjonalnie Email); generuj `UserId = $"local:{Guid}"`
- [x] Submit → `IMediator.Send(new CreatePersonAccountCommand { ... })` → auto-`SetContext` na nowy profil
- [x] Pusty stan: komunikat „Utwórz pierwszy profil” (brak redirect loop)
- [x] `ProfileRequiredRouteView` + `IProfileExemptPage` — bez kontekstu → `/profiles`
- [x] Menu: wyświetl aktywny profil (`GetPersonDataQuery`); link „Change profile”
- [x] Odświeżenie stanu UI po `SetContext` (`ContextChanged` + `NavMenu`)
- [ ] **Opcjonalnie v1:** usuwanie profilu (kaskada) — odłożone na Fazę 11

### Verification Plan
- Utwórz 2 profile → lista pokazuje 2; przełącz kontekst → `GetResumeListQuery` zwraca CV tylko aktywnego
- F5 po wyborze profilu → `localStorage` przywraca kontekst
- `git diff VRT.Resume.Application` — pusty

### Phase Summary
Feature-oriented `Features/Profiles/`: lista profili (`ProfilesPage`), tworzenie (`ProfilesCreatePage` → `CreatePersonAccountCommand`, `UserId = local:{guid}`), `ProfileRequiredRouteView` + `IProfileExemptPage` (profiles/about exempt). `NavMenu` pokazuje aktywny profil i „Change profile”. Build Release OK; `VRT.Resume.Application` nietknięty.

---

## Phase 4: MediatR patterns & error handling
Status: Complete

- [x] `MediatorSender.SendAsync` — port obsługi `ValidationException` / `Result` z `ControllerBase`
- [x] `UserNotificationService` (MudSnackbar) + field errors (`MediatorSendOutcome`)
- [x] Smoke: `Features/Person/PersonDataPage` — `GetPersonDataQuery`, `UpdatePersonDataCommand`, izolacja per profil

### Verification Plan
- Profil A ma inne imię niż profil B po `UpdatePersonData`
- Walidacja pustego FirstName przy tworzeniu profilu → błąd w UI

### Phase Summary
`Features/Mediator/`: `MediatorSender` (ValidationException → field errors + snackbar), `UserNotificationService`, `MediatorSendOutcome`. `ProfilesCreatePage` i `NavMenu` używają sendera. `PersonDataPage` — podstawowy edytor imienia/nazwiska pod smoke test izolacji profili.

---

## Phase 5: Profile UI — Person tabs & CRUD
Status: Complete

- [x] `/person` — zakładki: Profile, Edu, Skills, WorkExp, Contacts
- [x] CRUD przez MediatR (jak MVC): contacts, education, skills, experience, duties, `MergePersonDutySkills`
- [x] `EditDeleteToolbar`, `ConfirmDelete`

### Verification Plan
- Dane dodane w profilu A niewidoczne po przełączeniu na profil B

### Phase Summary
`Features/Person/PersonPage.razor` — MudTabs shell (Profile, Education, Skills, Work experience, Contacts). Shared `EditDeleteToolbar`, `ConfirmDeleteDialog`, `TimeRangeDisplay`. Tab listy + MudDialog edytory dla każdej encji; WorkExp zagnieżdżone duties + `DutySkillsEditorDialog` (`MergePersonDutySkillsCommand`). Usunięto smoke `PersonDataPage` — profil w zakładce Profile. Build Release Pwa OK; Application nietknięty.

---

## Phase 6: Resume management & skill merge
Status: Complete

- [x] `/` — `GetResumeListQuery` + Add/Edit/Delete/Clone
- [x] `UpsertPersonResume`, `ClonePersonResume`, `MergeResumeSkills`

### Verification Plan
- 2 profile, każdy z własnymi CV; clone działa w obrębie aktywnego kontekstu

### Phase Summary
`Pages/Home` — lista CV (`GetResumeListQuery` via `MediatorSender.SendQueryAsync`). `Features/Resumes/Editors/`: `ResumeEditorDialog` (`UpsertPersonResume`), `ResumeSkillsEditorDialog` (`MergeResumeSkills`). Akcje: edit, skills, clone, delete (reuse `ConfirmDeleteDialog`). `Routes.Resumes.Show` zarezerwowany na Fazę 8. Build Release Pwa OK.

---

## Phase 7: Profile image
Status: Not started

- [ ] `ProfileImageService` (SkiaSharp) — test WASM; fallback w Pwa jeśli potrzeba
- [ ] `/person/image` — `UpsertProfileImageCommand`
- [ ] Zdjęcie per `Person` — izolowane między profilami

### Verification Plan
- Różne zdjęcia dla profilu A i B

### Phase Summary
_(write when phase completes)_

---

## Phase 8: Resume view, print & CSS
Status: Not started

- [ ] `/resumes/show/{id}` — `GetFullResumeQuery` + A4 layout
- [ ] `Resume.css`, `window.print()`

### Verification Plan
- Print preview OK

### Phase Summary
_(write when phase completes)_

---

## Phase 9: Localization (PL/EN)
Status: Not started

- [ ] `PwaCultureService`, Resources, przełącznik języka
- [ ] Labelki na `/profiles` (PL: „Wybierz profil”, „Utwórz nowy profil”)

### Verification Plan
- PL ↔ EN na ekranie profili i formularzach

### Phase Summary
_(write when phase completes)_

---

## Phase 10: PWA offline hardening
Status: Not started

- [ ] Service worker, offline CRUD + przełączanie profili
- [ ] Lighthouse PWA ≥ 90

### Verification Plan
- Offline: utwórz profil, dodaj CV, przełącz profil, F5 — wszystko zachowane

### Phase Summary
_(write when phase completes)_

---

## Phase 11: Tests, docs & optional features
Status: Not started

- [ ] bUnit: wybór kontekstu, tworzenie profilu, izolacja danych
- [ ] Zaktualizuj `AGENTS.md` + `README.md`
- [ ] **Opcjonalnie:** usuwanie profilu (kaskada EF)
- [ ] **Opcjonalnie:** eksport/import całego pliku SQLite (wszystkie profile)

### Verification Plan
- `dotnet test` + `dotnet build` green

### Phase Summary
_(write when phase completes)_

---

## Final Recap
_(write when all phases complete)_

---

## Deployment Plan
_(write when all phases complete)_

```powershell
dotnet publish VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Release -o ./deploy/pwa
```

Statyczny hosting; dane (wszystkie lokalne profile) w SQLite WASM w przeglądarce użytkownika.

---

## Risk register

| Risk | Mitigation |
|------|------------|
| `CreatePersonAccount` ustawia `UserPerson.UserId = Email ?? UserId` | Przy braku email używaj unikalnego `local:{guid}` jako `UserId` |
| Singleton `DummyCurrentUserService` + Transient `DbContext` | Wzorzec jak MVC; kontekst tylko w Singleton, DB per operation |
| Brak kontekstu → 401 w handlerach | Route guard + zawsze `SetContext` przed wejściem w app |
| Usuwanie profilu — kaskada | Odłożyć lub transakcja DELETE Person + powiązania (Pwa service, nie Application) |
| `localStorage` wyczyszczone — utrata „ostatniego profilu” | Redirect `/profiles`; dane w SQLite zostają |
| Persistence / SQLite collation | Minimal fix w Persistence only |

---

## Effort estimate

| Phase | Duration (1 dev) |
|-------|------------------|
| 1 Scaffold | 2–3 dni |
| 2 DI + SQLite WASM | 4–6 dni |
| 3 **Local profiles** | 3–5 dni |
| 4 MediatR patterns | 2 dni |
| 5 Profile UI | 7–10 dni |
| 6 Resume mgmt | 3–5 dni |
| 7 Image | 2–4 dni |
| 8 Print/view | 3–5 dni |
| 9 i18n | 2–3 dni |
| 10 PWA offline | 2–3 dni |
| 11 Tests/docs | 3–5 dni |
| **Total** | **~8–11 tygodni** |

---

## What requires manual action from user

- Czy **usuwanie profilu** wchodzi do v1?
- Hosting statyczny przed deployem
- Biblioteka SQLite WASM — wybór po smoke teście (Faza 2)