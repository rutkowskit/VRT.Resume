# Resume print templates (PWA)

Add multiple CV **print/preview styles** per resume variant in `VRT.Resume.Pwa`, keeping browser print → PDF as the primary export path. Screen preview on `/resumes/show/{id}` switches templates live; each template can differ in **HTML structure** and **print CSS**, not only colors.

**In scope:** PWA only (MVC legacy unchanged unless a shared Application field is added).  
**Out of scope (v1):** QuestPDF / Playwright / server PDF in WASM; full WYSIWYG theme editor; per-section drag-and-drop layout.

## Feasibility verdict

| Approach | Verdict |
|----------|---------|
| Multiple Razor components + per-template CSS + `@media print` | **Recommended** — fits WASM, reuses `ResumeFullVM`, moderate effort |
| CSS-only themes on one DOM | **Limited** — OK for typography/colors; insufficient for sidebar vs single-column ATS layouts |
| Native PDF library in Blazor WASM | **Defer** — heavy bundle, duplicate rendering pipeline |

**Conclusion:** Implement **template registry + 2–3 components** first; add DB persistence in a later phase.

## Assumptions (defaults unless user overrides)

| Topic | Decision |
|-------|----------|
| Export path | Browser **Print** / Save as PDF (existing `PrintAsync`) |
| Template storage | **`localStorage`** key `VRT.Resume.PrintTemplate.{resumeId}` (per resume, per browser) |
| DB column | **Not used** — avoids `EnsureCreated` schema drift; not required for offline PWA |
| Default template | `classic` — current `ResumeDocument` layout |
| Second template (MVP) | `linear` — single column, ATS-friendly, no timeline graphics |
| Third template (optional) | `compact` — dense typography, 1-page oriented |
| MVC | No new templates (legacy host unchanged) |
| Localization | Template names in `LabelResource` (PL+EN) |

## Architecture sketch

```
ResumeShowPage
  └─ MudSelect TemplateId (preview + save on resume)
  └─ DynamicComponent / switch on TemplateId
        ├─ ResumeDocumentClassic.razor   (rename/move current ResumeDocument)
        ├─ ResumeDocumentLinear.razor
        └─ ResumeDocumentCompact.razor   (phase 3)

ResumeTemplateRegistry (PWA)
  └─ Id, ComponentType, LabelNames key, CssPath[]

wwwroot/css/resume/
  ├─ _print-base.css          (@page, hide Mud chrome — shared)
  ├─ classic.css              (screen + print)
  ├─ linear.css
  └─ compact.css
```

**Shared markup (optional refactor, phase 2):** extract section fragments under `Features/Resumes/Components/Sections/` (`ResumeContactSection`, `ResumeExperienceSection`, …) to avoid copy-paste across templates.

**Persistence:** `ResumePrintTemplateStorage` in PWA only (`localStorage`). Editor and show page read/write the same key. Invalid slug → `classic` via `ResumeTemplateRegistry.Normalize`.

## Template catalog (target)

| Slug | Screen | Print / PDF | Notes |
|------|--------|-------------|-------|
| `classic` | Two-column sidebar (current) | Optimized print CSS | Default |
| `linear` | Single column | Plain sections, comma skills, no icons in print | HR + ATS |
| `compact` | Single column, smaller type | Tighter margins, RODO inline footer | Long CVs → 2 pages — **implemented** |

Future (not v1): `modern` (accent color), `europass`-like — only if demand.

## Adding a new print template (step by step)

Use this checklist when introducing slug `mytemplate` (example name). **PWA only** — do not extend MVC `Resume.css` unless explicitly requested.

Preview and print share the **same DOM** on `/resumes/show/{id}` (WYSIWYG). Template choice is stored in **`localStorage`** (`VRT.Resume.PrintTemplate.{resumeId}`) — no database or Application layer changes.

### 1. Choose slug and layout

| Decision | Guidance |
|----------|----------|
| **Slug** | Lowercase ASCII, max ~16 chars, e.g. `modern`. Becomes `ResumeTemplateIds.Modern` and CSS class `resume-template-modern`. |
| **Layout** | New HTML structure → new Razor component. Colors/typography only → still prefer a dedicated CSS file; CSS-only on one DOM is insufficient for sidebar vs single-column. |
| **Reuse** | Prefer shared sections under `Features/Resumes/Components/Sections/` (`ResumeExperienceSection`, `ResumeEducationSection`, `ResumeContactSection`, `ResumeInlineContactLine`) before copy-pasting blocks. |
| **Data** | All templates use `ResumeFullVM` + optional `ProfileImageUrl` via `ResumeDocumentBase` — no new queries. |

### 2. Constants and registry

1. Add slug to `Features/Resumes/Templates/ResumeTemplateIds.cs`:

   ```csharp
   public const string Modern = "modern";
   ```

2. Register in `Features/Resumes/Templates/ResumeTemplateRegistry.cs`:

   ```csharp
   new(
       ResumeTemplateIds.Modern,
       typeof(ResumeDocumentModern),
       LabelNames.ResumeTemplateModern,
       "css/resume/modern.css"),
   ```

   `ResumeShowPage` and `ResumeEditorDialog` iterate `ResumeTemplateRegistry.All` — **no page changes** if the registry entry exists.

3. `Normalize()` falls back to `classic` for unknown slugs; old `localStorage` values with removed slugs also resolve to classic.

### 3. Razor component

1. Create `Features/Resumes/Components/ResumeDocumentModern.razor` + `.razor.cs`.
2. Code-behind: `public partial class ResumeDocumentModern : ResumeDocumentBase;` (ordering of experience/education is inherited).
3. Markup pattern:

   ```razor
   @inherits ResumeDocumentBase
   <div class="resume page-a4 resume-template-modern">
       <!-- sections; use shared *Section components where possible -->
   </div>
   ```

4. Root classes **`resume`** and **`page-a4`** are required (shared print chrome in `_print-base.css`). Template-specific root: **`resume-template-{slug}`** — all template CSS must scope under it.

5. Use `ResumeDisplayHelpers` for dates, skills, contacts. Use `LabelNames.*.GetLabelText()` for section titles (PL/EN).

### 4. CSS

1. Add `wwwroot/css/resume/modern.css` — **screen + `@media print`** rules in one file.
2. **Do not** add the file to `index.html`. `ResumeShowPage` injects the active template via `HeadContent`:

   ```razor
   <link rel="stylesheet" href="@_activeTemplate.CssPath" />
   ```

3. `wwwroot/css/resume/_print-base.css` stays in `index.html` (Mud chrome hidden, `@page`, `.page-a4` shell). Template file handles layout only.

4. Prefix every rule with `.resume-template-modern` (avoid leaking styles into other templates when switching).

5. Print tips (from existing templates):

   - Hide contact icons in print if needed: `svg, img { display: none; }` under contact blocks.
   - ATS-friendly skills: comma-separated inline text, not chip boxes (see `linear.css` / `compact.css`).
   - Page breaks: `break-inside: avoid` on **`.resume-work-item` / `.resume-edu-item`**, not on whole `.resume-section` (see `compact.css`).
   - RODO: compact footer with small `font-size` in print; `page-break-before: avoid` on footer block.

### 5. Localization

1. `VRT.Resume.Resources/LabelResource.resx` — e.g. `ResumeTemplateModern` → `Modern`.
2. `LabelResource.pl.resx` — e.g. `Nowoczesny`.
3. `VRT.Resume.Pwa/LabelNames.cs` — `public const string ResumeTemplateModern = "ResumeTemplateModern";`

Picker label `ResumeTemplateLabel` (“Print template” / “Szablon druku”) already exists.

### 6. Persistence (no code unless new UX)

- **Show page:** `ResumeShowPage` → `ResumePrintTemplateStorage` on load/save — works for any registered slug.
- **Editor:** `ResumeEditorDialog` saves template to `localStorage` when `ResumeId > 0` on Save; new resume (`ResumeId == 0`) defaults to classic until user opens show page.
- **Not in DB backup:** export/import `.db` does not include template preference.

### 7. Tests

1. Add `VRT.Resume.Pwa.Tests/Components/ResumeDocumentModernTests.cs`:

   ```csharp
   [Fact]
   public async Task RendersSeededResumeFullVM()
   {
       string markup;
       using (var ctx = new PwaTestContext())
       {
           var cut = ctx.RenderWithMudProviders<ResumeDocumentModern>(p =>
               p.Add(c => c.Model, ResumeTestData.CreateSampleResume()));
           markup = cut.Markup;
       }
       await Verify(markup);
   }
   ```

2. First run creates `.received.txt`; accept as `.verified.txt` (or `dotnet test -- Verify.AcceptChanges`).

3. Reuse `ResumeTestData.CreateSampleResume()` or extend fixture for edge cases (empty sections, long summary).

### 8. Verification

```powershell
dotnet build VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Debug
dotnet test VRT.Resume.Pwa.Tests/VRT.Resume.Pwa.Tests.csproj -c Debug
```

Manual:

1. `/resumes/show/{id}` → picker lists new template → preview updates.
2. Reload page → choice restored from `localStorage`.
3. **Ctrl+P** / Save as PDF — layout matches preview; test **Chrome and Edge**.
4. Switch back to **classic** — classic CSS/layout unaffected.

After CSS publish: hard-refresh or wait for service worker update (`pwa-boot.js`).

### 9. Optional follow-ups

| Item | When |
|------|------|
| Extract new shared section component | Same block used in 2+ templates |
| Home list badge for non-default template | Needs async `localStorage` per row — deferred |
| DB column for template | Only if cross-device or backup must carry preference — currently **avoid** |

### 10. Files touched (checklist)

| File | Action |
|------|--------|
| `ResumeTemplateIds.cs` | Add constant |
| `ResumeTemplateRegistry.cs` | Add descriptor |
| `ResumeDocument{Name}.razor` (+ `.cs`) | New component |
| `wwwroot/css/resume/{slug}.css` | New styles |
| `LabelResource.resx` / `.pl.resx` | New label key |
| `LabelNames.cs` | New constant |
| `ResumeDocument{Name}Tests.cs` | Verify snapshot |
| `Sections/*.razor` | Only if extracting shared markup |

**Do not change:** `VRT.Resume.Application`, `PersonResume`, `UpsertPersonResumeCommand`, MVC views.

## For Future Agents

Mark checkboxes `- [x]` as complete; set phase `Status: Complete` + **Phase Summary**; run **Verification Plan** before next phase.

---

## Adding a new print template (step by step)

Use this checklist when introducing slug `mytemplate` (example). **PWA only** — do not extend MVC or the database.

### 1. Design and pick a base

| Question | Guidance |
|----------|----------|
| Layout | Sidebar (copy **classic**), single column (copy **linear**), or dense single column (copy **compact**)? |
| Slug | Lowercase ASCII, max ~16 chars, e.g. `modern` — becomes CSS class `resume-template-modern` |
| Data | All templates use the same `ResumeFullVM` from `GetFullResumeQuery` — no new Application queries |
| Storage | **No DB** — picker on show page and editor already persist via `ResumePrintTemplateStorage` once the slug is registered |

### 2. Constants and registry

1. Add slug to `VRT.Resume.Pwa/Features/Resumes/Templates/ResumeTemplateIds.cs`:
   ```csharp
   public const string MyTemplate = "mytemplate";
   ```
2. Register in `ResumeTemplateRegistry.cs`:
   ```csharp
   new(
       ResumeTemplateIds.MyTemplate,
       typeof(ResumeDocumentMyTemplate),
       LabelNames.ResumeTemplateMyTemplate,
       "css/resume/mytemplate.css"),
   ```
   `Normalize()` and the show-page picker pick up new entries automatically — **no changes** to `ResumeShowPage`, `DependencyInjection`, or storage.

### 3. Localization (PL + EN)

1. `VRT.Resume.Resources/LabelResource.resx` — e.g. `ResumeTemplateMyTemplate` → `My template`
2. `LabelResource.pl.resx` — e.g. `Kompaktowy` style caption in Polish
3. `VRT.Resume.Pwa/LabelNames.cs` — `public const string ResumeTemplateMyTemplate = "ResumeTemplateMyTemplate";`

### 4. Razor component

Create under `Features/Resumes/Components/`:

| File | Content |
|------|---------|
| `ResumeDocumentMyTemplate.razor` | Markup only (`@inherits ResumeDocumentBase`) |
| `ResumeDocumentMyTemplate.razor.cs` | `public partial class ResumeDocumentMyTemplate : ResumeDocumentBase;` |

**Markup rules:**

- Root: `<div class="resume page-a4 resume-template-mytemplate">` — **must** include `resume-template-{slug}` for CSS isolation.
- Reuse shared sections when possible (`Features/Resumes/Components/Sections/`):
  - `ResumeContactSection` — vertical contact block (classic sidebar)
  - `ResumeInlineContactLine` — one-line header contact (linear / compact)
  - `ResumeExperienceSection`, `ResumeEducationSection` — same HTML; style via template CSS
- Ordering / filtering: `OrderedExperience`, `OrderedEducation` on `ResumeDocumentBase` — do not duplicate.
- Labels: `LabelNames.*.GetLabelText()`; data helpers: `ResumeDisplayHelpers`.
- Photo: optional per template (classic uses `MudImage` + `ShowProfilePhoto`; linear/compact omit for ATS).

### 5. Template CSS

Create `wwwroot/css/resume/mytemplate.css`.

**Scope every rule** under `.resume-template-mytemplate` (and descendants) so templates do not leak into each other when switching on the show page.

| Layer | File | Responsibility |
|-------|------|----------------|
| Shared | `_print-base.css` (loaded in `index.html`) | Hide Mud chrome, `@page`, `.page-a4` shell, show-page toolbar |
| Per template | `mytemplate.css` (loaded via `HeadContent` on show page) | Layout, typography, screen + `@media print` for this template only |

**Print checklist:**

- Hide contact icons in print if needed: `svg, img { display: none; }` under contact blocks.
- Skills as comma-separated text for ATS: inline spans + `::after { content: ", "; }` on non-last items (see `linear.css`).
- Page breaks: prefer `break-inside: avoid` on **`.resume-work-item` / `.resume-edu-item`**, not on whole `.resume-section` (see `compact.css`).
- Do **not** use `visibility: hidden` print hacks — use `_print-base.css` to hide app chrome only.

`index.html` loads only `_print-base.css`. Per-template CSS is injected in `ResumeShowPage.razor`:

```razor
<HeadContent>
    <link rel="stylesheet" href="@_activeTemplate.CssPath" />
</HeadContent>
```

### 6. Optional: editor dialog

`ResumeEditorDialog` already lists `ResumeTemplateRegistry.All`. New templates appear in the picker when editing an existing resume (`ResumeId > 0`); save writes `localStorage` only. **No** `UpsertPersonResumeCommand` changes.

### 7. Tests

1. Add `VRT.Resume.Pwa.Tests/Components/ResumeDocumentMyTemplateTests.cs`:
   - Render with `PwaTestContext` + `ResumeTestData.CreateSampleResume()`
   - Capture `markup` before disposing the context
   - `await Verify(markup);` (first run: accept `.verified.txt`)
2. Run:
   ```powershell
   dotnet test VRT.Resume.Pwa.Tests/VRT.Resume.Pwa.Tests.csproj -c Debug
   ```

### 8. Manual verification

1. `dotnet build VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Debug`
2. Open `/resumes/show/{id}` → select new template → preview updates live.
3. Ctrl+P / Save as PDF — layout matches preview (same DOM).
4. Reload page — choice persists (`localStorage`).
5. Second CV with another template — choices are independent per `ResumeId`.
6. After publish: hard-refresh or wait for service-worker update if CSS looks stale.

### 9. Do not change (unless explicitly requested)

| Layer | Reason |
|-------|--------|
| `VRT.Resume.Application` / `PersonResume` | Template is UI-only; DB column rejected to avoid `EnsureCreated` drift |
| `VRT.Resume.Mvc` | Legacy host; separate `Resume.css` |
| `UpsertPersonResumeCommand` | Template not stored on resume entity |
| `index.html` per-template `<link>` | Use `HeadContent` + registry `CssPath` |

### 10. File checklist (copy for PR description)

- [ ] `ResumeTemplateIds.cs` — new constant
- [ ] `ResumeTemplateRegistry.cs` — descriptor entry
- [ ] `ResumeDocumentMyTemplate.razor` + `.razor.cs`
- [ ] `wwwroot/css/resume/mytemplate.css`
- [ ] `LabelResource.resx` + `LabelResource.pl.resx` + `LabelNames.cs`
- [ ] `ResumeDocumentMyTemplateTests.cs` + `.verified.txt`
- [ ] Manual print smoke on Chrome/Edge

---

## Phase 1: Registry and show-page preview (no DB)

Status: Complete

- [x] Add `ResumeTemplateIds` constants (`classic`, `linear`)
- [x] Add `ResumeTemplateRegistry` + `ResumeTemplateDescriptor` (id, component type, label resource key, css paths)
- [x] Rename `ResumeDocument` → `ResumeDocumentClassic` (or keep file, add wrapper class `resume-template-classic`)
- [x] Split `Resume.css`: move shared print chrome to `wwwroot/css/resume/_print-base.css`; keep classic rules in `classic.css`
- [x] Load template CSS from `index.html` (or dynamic link in `ResumeShowPage` when template changes)
- [x] `ResumeShowPage`: `MudSelect` template picker; bind `_selectedTemplateId`; render chosen component via `DynamicComponent` or `switch`
- [x] Persist picker in `localStorage` per `ResumeId`
- [x] Print toolbar: show selected template name; print uses active template (same DOM as preview)

### Verification Plan

- `dotnet build VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Debug`
- Manual: open resume show → switch template → preview updates; Ctrl+P shows correct layout
- `dotnet test VRT.Resume.Pwa.Tests` — add smoke test rendering show page with `classic` (optional in this phase)

### Phase Summary

Registry + `ResumeDocumentClassic` + split CSS (`_print-base.css`, `classic.css`). `ResumeShowPage` uses `MudSelect`, `DynamicComponent`, `HeadContent` for per-template CSS, and `ResumePrintTemplateStorage` (`localStorage` per resume). Only `classic` is registered until Phase 2 adds `linear`.

---

## Phase 2: Linear template + shared sections

Status: Complete

- [x] Implement `ResumeDocumentLinear.razor` — order: header (name, position, contact line) → summary → experience → education → skills (text lists) → languages → RODO
- [x] Add `linear.css` — screen + `@media print`; no timeline `::before/::after`; skills as comma-separated text (reuse print rules from classic)
- [x] Extract 2–3 shared section components used by classic + linear (contact, experience list minimum)
- [x] Add `LabelResource` / `LabelResource.pl.resx` keys: `ResumeTemplateClassic`, `ResumeTemplateLinear`, `ResumeTemplateLabel`
- [x] bUnit: `ResumeDocumentLinear` renders with seeded `ResumeFullVM` fixture

### Verification Plan

- `dotnet test VRT.Resume.Pwa.Tests/VRT.Resume.Pwa.Tests.csproj -c Debug`
- Manual: linear template PDF text layer is continuous (no per-character spacing); one-column order readable

### Phase Summary

`ResumeDocumentLinear` (single-column ATS layout) + `linear.css`. Shared sections: `ResumeContactSection`, `ResumeExperienceSection`, `ResumeEducationSection`, `ResumeInlineContactLine`; `ResumeDocumentBase` holds ordering logic. Classic refactored to use sections. Registry registers `linear`. bUnit smoke test in `ResumeDocumentLinearTests`.

---

## Phase 3: Persist template per resume (DB + editor)

Status: Cancelled — **localStorage only** (no DB column)

Decision: per-resume print template stays in `localStorage` only. No `PersonResume.PrintTemplate`, no schema changes, no migrations / `EnsureCreated` drift.

- [x] `ResumeShowPage` + `ResumeEditorDialog`: template via `ResumePrintTemplateStorage` (`localStorage` per `ResumeId`)
- [x] Editor template picker when editing existing resume (`ResumeId > 0`); new resume → default on show page
- [ ] ~~DB column / upsert / integration test~~ — intentionally not implemented

### Phase Summary

Reverted DB persistence. Template choice is browser-local per resume ID; export/import of `.db` does not include template preference (acceptable for PWA).

---

## Phase 4: Compact template + polish (optional)

Status: Complete

- [x] `ResumeDocumentCompact` + `compact.css`
- [x] Tune page breaks (`break-inside` on experience items only, not whole sections)
- [ ] Home list: optional icon/badge for non-default template (low priority) — deferred

### Verification Plan

- Manual: long seeded resume prints ≤2 pages on A4 with compact; classic unchanged

### Phase Summary

`ResumeDocumentCompact` — dense single-column layout (combined skills block, inline RODO). `compact.css` with smaller type/spacing; `break-inside: avoid` on work/education items only. Registered in `ResumeTemplateRegistry`; bUnit Verify snapshot. Home list badge deferred (localStorage-only template metadata).

---

## Risks and mitigations

| Risk | Mitigation |
|------|------------|
| EF `EnsureCreated` — no migration for existing PWA DB | Default `classic` when column missing; document export/import; optional startup `ALTER TABLE` only if explicitly approved |
| CSS duplication across templates | `_print-base.css` + shared section components |
| Browser print inconsistencies | Keep visibility-free print isolation; test Chrome + Edge; document “use Save as PDF” |
| WASM bundle size (extra CSS) | Small CSS files (<5 KB each); no extra JS PDF libs |
| MVC drift | Do not port templates to MVC; shared column only if MVC reads resume metadata later |

## What we explicitly will NOT do (unless requested later)

- User-uploaded custom CSS
- Template marketplace / JSON layout DSL
- Pixel-perfect match to Word exports
- Server-generated PDF attached to email

## Final Recap

Three print templates (`classic`, `linear`, `compact`) on PWA resume show page with `MudSelect`, `DynamicComponent`, and per-template CSS. Template choice stored in **`localStorage` only** (no DB schema change). Shared section components for contact, experience, education. bUnit + Verify snapshots for linear and compact.

## Deployment Plan

Normal PWA publish (`deploy-pwa-cloudflare.ps1` or local publish). After CSS changes, hard-refresh or wait for service-worker update (`pwa-boot.js`). No backend or database migration steps.