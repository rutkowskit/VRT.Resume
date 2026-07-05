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
| MVC | No new templates; optional read of `PrintTemplate` if column exists |
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
| `compact` | Single column, smaller type | Tighter margins, RODO inline footer | Long CVs → 2 pages |

Future (not v1): `modern` (accent color), `europass`-like — only if demand.

## For Future Agents

Mark checkboxes `- [x]` as complete; set phase `Status: Complete` + **Phase Summary**; run **Verification Plan** before next phase.

---

## Phase 1: Registry and show-page preview (no DB)

Status: Complete

- [x] Add `ResumeTemplateIds` constants (`classic`, `linear`)
- [x] Add `ResumeTemplateRegistry` + `ResumeTemplateDescriptor` (id, component type, label resource key, css paths)
- [x] Rename `ResumeDocument` → `ResumeDocumentClassic` (or keep file, add wrapper class `resume-template-classic`)
- [x] Split `Resume.css`: move shared print chrome to `wwwroot/css/resume/_print-base.css`; keep classic rules in `classic.css`
- [x] Load template CSS from `index.html` (or dynamic link in `ResumeShowPage` when template changes)
- [x] `ResumeShowPage`: `MudSelect` template picker; bind `_selectedTemplateId`; render chosen component via `DynamicComponent` or `switch`
- [x] Persist picker in `localStorage` per `ResumeId` until DB field exists
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

Status: Not started

- [ ] `ResumeDocumentCompact` + `compact.css`
- [ ] Tune page breaks (`break-inside` on experience items only, not whole sections)
- [ ] Home list: optional icon/badge for non-default template (low priority)

### Verification Plan

- Manual: long seeded resume prints ≤2 pages on A4 with compact; classic unchanged

### Phase Summary

_(write when phase completes)_

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

_(write when all phases complete)_

## Deployment Plan

_(write when all phases complete: no special deploy beyond normal PWA publish; mention hard-refresh after CSS changes)_