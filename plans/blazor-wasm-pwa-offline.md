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
| UI | Bootstrap 5 + `Resume.css` z MVC |
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
Status: Not started

- [ ] `dotnet new blazorwasm -n VRT.Resume.Pwa -o VRT.Resume.Pwa --pwa` (net10.0)
- [ ] Dodaj do `VRT.Resume.sln` i `VRT.Resume.slnx` (folder `/01.Client/`)
- [ ] Referencje: `VRT.Resume.Application`, `VRT.Resume.Resources`
- [ ] Pakiety: `Microsoft.EntityFrameworkCore.Sqlite` + SQLite WASM (wybór w Fazie 2)
- [ ] Usuń `Counter`, `FetchData`
- [ ] Layout: **Profiles** (kontekst), Home, Person, About — Home/Person disabled bez kontekstu
- [ ] `manifest.webmanifest` + service worker

### Verification Plan
- `dotnet build VRT.Resume.sln -c Release` — 0 errors
- `dotnet run --project VRT.Resume.Pwa` — start OK

### Phase Summary
_(write when phase completes)_

---

## Phase 2: Composition root — DI, DbContext, seed (w Pwa)
Status: Not started

- [ ] `DependencyInjection.cs` — `AddApplication()`, serwisy aplikacyjne
- [ ] `DummyCurrentUserService` — **Singleton**, `ICurrentUserService`, `SetContext(userId)`, restore z `localStorage` przy starcie
- [ ] `ProfileContextStorage` — JS interop: get/set `VRT.Resume.ActiveProfileUserId`
- [ ] `LocalProfileService` — Scoped; odczyt listy profili z `AppDbContext`
- [ ] `DependencyInjection.DbContext.cs` — `AddDbContext<AppDbContext>`, Transient (jak MVC), SQLite WASM
- [ ] `DatabaseInitializer` — `InitDatabase()` (SkillType seed only); **nie** seeduj domyślnego użytkownika
- [ ] Zweryfikuj Persistence pod SQLite (`UseCollation`, schema `Auth`)

### Verification Plan
- `EnsureCreated()` OK; `SkillType` = 5; `UserPerson` puste na świeżej bazie
- Po ręcznym insert testowym: `DummyCurrentUserService.SetContext` → `UserId` zwracany poprawnie

### Phase Summary
_(write when phase completes)_

---

## Phase 3: Local profiles — create, list, switch context
Status: Not started

- [ ] `/profiles` — `LocalProfileService.GetAllAsync()`; karty z imieniem/nazwiskiem; przycisk **Wybierz** → `SetContext(userId)` → redirect `/`
- [ ] `/profiles/create` — formularz: FirstName, LastName (opcjonalnie Email); generuj `UserId = $"local:{Guid}"`
- [ ] Submit → `IMediator.Send(new CreatePersonAccountCommand { ... })` → auto-`SetContext` na nowy profil
- [ ] Pusty stan: komunikat „Utwórz pierwszy profil” (brak redirect loop)
- [ ] `AuthorizeRouteView` / custom `ProfileRequiredRouteView` — bez kontekstu → `/profiles`
- [ ] Menu: wyświetl aktywny profil (imię/nazwisko z `GetPersonDataQuery` lub z listy); link „Zmień profil”
- [ ] Odświeżenie stanu UI po `SetContext` (`NavigationManager` + event `OnContextChanged`)
- [ ] **Opcjonalnie v1:** usuwanie profilu (kaskada) — jeśli poza scope, odłożyć na Fazę 11

### Verification Plan
- Utwórz 2 profile → lista pokazuje 2; przełącz kontekst → `GetResumeListQuery` zwraca CV tylko aktywnego
- F5 po wyborze profilu → `localStorage` przywraca kontekst
- `git diff VRT.Resume.Application` — pusty

### Phase Summary
_(write when phase completes)_

---

## Phase 4: MediatR patterns & error handling
Status: Not started

- [ ] Helper `MediatorExtensions.SendAsync` — port obsługi `ValidationException` / `Result` z `ControllerBase`
- [ ] Toast/Alert dla sukcesu i błędu
- [ ] Smoke: `GetPersonDataQuery`, `UpdatePersonDataCommand` w kontekście profilu A vs B — izolacja danych

### Verification Plan
- Profil A ma inne imię niż profil B po `UpdatePersonData`
- Walidacja pustego FirstName przy tworzeniu profilu → błąd w UI

### Phase Summary
_(write when phase completes)_

---

## Phase 5: Profile UI — Person tabs & CRUD
Status: Not started

- [ ] `/person` — zakładki: Profile, Edu, Skills, WorkExp, Contacts
- [ ] CRUD przez MediatR (jak MVC): contacts, education, skills, experience, duties, `MergePersonDutySkills`
- [ ] `EditDeleteToolbar`, `ConfirmDelete`

### Verification Plan
- Dane dodane w profilu A niewidoczne po przełączeniu na profil B

### Phase Summary
_(write when phase completes)_

---

## Phase 6: Resume management & skill merge
Status: Not started

- [ ] `/` — `GetResumeListQuery` + Add/Edit/Delete/Clone
- [ ] `UpsertPersonResume`, `ClonePersonResume`, `MergeResumeSkills`

### Verification Plan
- 2 profile, każdy z własnymi CV; clone działa w obrębie aktywnego kontekstu

### Phase Summary
_(write when phase completes)_

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