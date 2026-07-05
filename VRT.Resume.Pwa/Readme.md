# VRT.Resume.Pwa

Offline Blazor WebAssembly PWA for generating CVs from local profiles. No OAuth, no backend — all data lives in the browser (SQLite WASM / OPFS). UI: MudBlazor, languages: PL (default) and EN.

Architecture and pitfalls: [`../AGENTS.md`](../AGENTS.md) (section **VRT.Resume.Pwa**), plan: [`../plans/blazor-wasm-pwa-offline.md`](../plans/blazor-wasm-pwa-offline.md).

## Prerequisites

- .NET 10 SDK
- `wasm-tools` workload: `dotnet workload restore` (from repo root)
- Modern Chromium-based browser (Chrome, Edge) for OPFS and PWA features

## Run locally (development)

```powershell
dotnet run --project VRT.Resume.Pwa/VRT.Resume.Pwa.csproj
```

Open the URL from the console (e.g. `http://localhost:5176`). Create a profile at `/profiles`, then use **Resumes** and **Person** tabs.

On `/profiles` you can **export** or **import** the full SQLite database (all local profiles) as a `.db` file.

### Offline (dev)

1. Load the app **online** and wait until it fully starts (service worker caches the WASM shell).
2. DevTools → **Application** → **Service Workers** — worker should be *activated*.
3. DevTools → **Network** → **Offline** → refresh (F5).

User data remains in browser SQLite (OPFS). When online, `pwa-boot.js` checks for a new service worker and reloads automatically after publish.

## Publish locally (smoke test)

```powershell
dotnet publish VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Release -o ./deploy/pwa
./VRT.Resume.Pwa/serve-published.ps1
```

Open **`http://127.0.0.1:8080`** (script sets COOP/COEP headers required by SqliteWasm). Use **one browser tab** per origin.

### Lighthouse PWA audit

```powershell
./VRT.Resume.Pwa/run-lighthouse.ps1
```

Requires Node.js and Chrome or Edge. Target: PWA score ≥ 90.

## Tests

```powershell
dotnet test VRT.Resume.Pwa.Tests/VRT.Resume.Pwa.Tests.csproj -c Debug
```

## Deploy to Cloudflare Pages (static, ZIP upload)

Cloudflare serves the published **`wwwroot`** folder as a static site. Upload a ZIP whose **root contains `index.html`** (not the parent `deploy/pwa` folder).

### Checklist

#### 1. Build the app

From the repository root:

```powershell
dotnet publish VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Release -o ./deploy/pwa
```

Output: `./deploy/pwa/wwwroot/` (HTML, `_framework/`, `_content/`, `service-worker.js`, etc.).

#### 2. Cloudflare helper files (included in publish output)

`wwwroot/_headers` and `wwwroot/_redirects` ship with the project and are copied into `./deploy/pwa/wwwroot/` on publish — no manual step before zipping.

- **`_headers`** — OPFS / SharedArrayBuffer need cross-origin isolation (COOP/COEP).
- **`_redirects`** — SPA deep links (`/profiles`, `/person`, `/resumes`, etc.) return `index.html` with HTTP 200.

Do **not** redirect `/_framework/*`, `/_content/*`, or other static assets — only app routes listed in `_redirects`.

#### 3. Verify locally (recommended)

Re-run the publish server against the folder that includes `_headers` / `_redirects`, or deploy to a preview URL first. Confirm:

- [ ] App loads over **HTTPS** (Cloudflare provides this automatically).
- [ ] Create profile, add data, refresh — data persists.
- [ ] Offline refresh works after one online visit (service worker + cache).
- [ ] Only **one tab** open per site (OPFS single-writer lock).

#### 4. Create the ZIP

The archive root must be the contents of `wwwroot`, not the `wwwroot` directory itself.

**PowerShell:**

```powershell
$root = Resolve-Path ./deploy/pwa/wwwroot
$zip  = Resolve-Path ./deploy -ErrorAction SilentlyContinue
if (-not $zip) { New-Item -ItemType Directory -Path ./deploy | Out-Null }
Compress-Archive -Path "$root\*" -DestinationPath ./deploy/vrt-resume-pwa.zip -Force
```

**Manually:** open `deploy/pwa/wwwroot`, select all files and folders inside, compress to `vrt-resume-pwa.zip`. After unzip, `index.html` must be at the top level.

#### 5. Cloudflare dashboard

1. [Cloudflare Dashboard](https://dash.cloudflare.com) → **Workers & Pages** → **Create** → **Pages** → **Upload assets**.
2. Project name (e.g. `vrt-resume-pwa`).
3. Upload `vrt-resume-pwa.zip`.
4. Wait for the deployment to finish; note the `*.pages.dev` URL.
5. (Optional) **Custom domains** → add your domain; HTTPS is automatic.

#### 6. Post-deploy smoke test

- [ ] Open the production URL — landing / profile selection works.
- [ ] Deep link `/profiles` and `/person` load (no 404 from the server).
- [ ] PWA install prompt or “Install app” in browser menu (manifest + service worker).
- [ ] Export database on `/profiles` as a backup.

### Cloudflare notes

| Topic | Detail |
|--------|--------|
| **Build command** | None — deploy pre-built `wwwroot` only (direct upload). |
| **Build output directory** | N/A for ZIP upload. |
| **Data** | Stays in each user’s browser; no server database. Users should use **Export database** on `/profiles`. |
| **Updates** | Re-run publish, create a new ZIP, upload a new deployment. Online clients auto-reload when a new service worker is detected (`pwa-boot.js`). |
| **CI alternative** | Same `wwwroot` artifact can be produced in GitHub Actions and connected via Cloudflare Pages Git integration instead of ZIP. |

## Limitations

- **One browser tab** per origin (OPFS / SqliteWasm).
- **First visit must be online** so the service worker can cache the WASM shell.
- **No server-side auth or sync** — use export/import for backup and migration between devices.