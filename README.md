# Simple resume generator

The generator enables you to generate multiple versions of your CV to suit your specific needs.

# Supported Auth providers

## Sign in with Gmail
To add OAuth 2.0 Client Ids:

1. Open https://console.cloud.google.com/apis/credentials,
1. Log in to your google account,
1. Click **```+ Create Credentials```** menu option and select **OAuth client ID**,
1. In the **Application type** field, select **Web application**,
1. Give it a name e.g. **Local-VRT-Resume**,
1. In the **Authorized redirect URIs** section click ```+ Add URI``` button,
1. Add the local URI ```https://localhost:5001/signin-google``` (it should be the same URI which is set in the ```Properties\launchSettings.json``` project file),
1. Add other URI's if you want to use IIS or IIS express instead of Kestrel (the above option),
1. Click ```Save``` button (do not close this page yet, you will need **ClientId** and **ClientSecret** during configuration)
1. Open VRT.Resume Solution in Visual Studio,
1. Right Click on the ```VRT.Resume.Mvc``` Project and select **Manage User Secrets** option,
1. Add the section with google client configuration:
    ```json
    "Auth": {
        "Providers": [
        {
            "Name": "Google",
            "ClientId": "<your client id that google generates for your web application>",
            "ClientSecret": "<your user secret>",
            "CallbackPath": "/signin-google"
        }         
        ]
    }
    ```

Now you can Log in to the application using your Google credentials. 

## Sign in with Github


## Publishing to Azure

The web application is hosted in **Azure App Service** (`VRT.Resume.Mvc`). Use the steps below to publish a new version.

### 1. Create Azure resources

1. Sign in to the [Azure Portal](https://portal.azure.com/).
1. Create an **App Service** (Windows or Linux) with a runtime that supports **.NET 10**.
1. *(Recommended for production)* Create an **Azure SQL Database** and note the connection string.
1. *(Optional, for testing only)* Skip the database service and use **SQLite** with a file stored on the App Service file system.

### 2. Configure OAuth redirect URIs

Add the production callback URLs to your OAuth applications (the same providers described above):

| Provider | Redirect URI |
|----------|--------------|
| Google   | `https://<your-app-name>.azurewebsites.net/signin-google` |
| GitHub   | `https://<your-app-name>.azurewebsites.net/signin-github` |

Replace `<your-app-name>` with your App Service host name.

### 3. Configure App Service application settings

In the Azure Portal, open **App Service → Settings → Environment variables** (or **Configuration → Application settings**) and add:

| Setting | Example value | Notes |
|---------|---------------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Required for production error handling |
| `DbProvider` | `mssql` or `sqlite` | Must match the connection string you configure |
| `ConnectionStrings__ResumeData__mssql` | `Server=tcp:...` | Use when `DbProvider` is `mssql` |
| `ConnectionStrings__ResumeData__sqlite` | `Data Source=D:\home\site\data\resumedata.sqlite` | Use when `DbProvider` is `sqlite` |
| `Auth__Providers__0__Name` | `Google` | First auth provider |
| `Auth__Providers__0__ClientId` | `<client id>` | From Google Cloud Console |
| `Auth__Providers__0__ClientSecret` | `<client secret>` | Store as a secret / Key Vault reference |
| `Auth__Providers__0__CallbackPath` | `/signin-google` | Must match OAuth app settings |
| `Auth__Providers__1__Name` | `Github` | Second auth provider (optional) |
| `Auth__Providers__1__ClientId` | `<client id>` | From GitHub OAuth app |
| `Auth__Providers__1__ClientSecret` | `<client secret>` | Store as a secret / Key Vault reference |
| `Auth__Providers__1__CallbackPath` | `/signin-github` | Must match OAuth app settings |

The application creates and seeds the database automatically on startup (`EnsureCreated`).

### 4. Build and publish locally

From the solution root, run tests and publish the MVC project:

```powershell
dotnet test --configuration Release
dotnet publish .\VRT.Resume.Mvc\VRT.Resume.Mvc.csproj --configuration Release --output .\deploy\web
```

Alternatively, use the Cake build script (same output folder):

```powershell
dotnet tool restore
dotnet cake --target=PublishWeb
```

The publish output is written to `./deploy/web`.

### 5. Deploy to App Service

Choose one deployment option:

**Visual Studio**

1. Right-click `VRT.Resume.Mvc` → **Publish**.
1. Select **Azure** → **Azure App Service (Windows/Linux)**.
1. Choose the target App Service and publish.

**Azure CLI (zip deploy)**

```powershell
Compress-Archive -Path .\deploy\web\* -DestinationPath .\deploy\web.zip -Force
az webapp deploy --resource-group <resource-group> --name <app-name> --src-path .\deploy\web.zip --type zip
```

**GitHub Actions / CI/CD**

Add a deploy step after `dotnet publish` that uploads `./deploy/web` to the App Service (for example with `azure/webapps-deploy`).

### 6. Verify the deployment

1. Open `https://<your-app-name>.azurewebsites.net/`.
1. Confirm that sign-in with Google and/or GitHub works.
1. Create or edit a resume to verify database connectivity.
1. Check **App Service → Log stream** or **Application Insights** if the app fails to start.


## Blazor WASM PWA (offline)

`VRT.Resume.Pwa` is an offline CV generator: local profiles (no OAuth), SQLite WASM in the browser (OPFS), PL/EN UI (MudBlazor).

### Run locally (development)

```powershell
dotnet run --project VRT.Resume.Pwa/VRT.Resume.Pwa.csproj
```

Open the URL from the console (e.g. `http://localhost:5176`). Create a profile at `/profiles`, then use Resumes and Person tabs. On `/profiles` you can **export** or **import** the full SQLite database (all local profiles) as a `.db` file.

**Offline:** the service worker caches the WASM shell after the first online visit. To test: load the app online → wait until it fully starts → DevTools → Network → Offline → refresh. Data stays in browser SQLite (OPFS). For OPFS, prefer `pwsh ./VRT.Resume.Pwa/serve-published.ps1` on `http://127.0.0.1:8080` when testing a published build.

### Publish and serve statically

```powershell
dotnet publish VRT.Resume.Pwa/VRT.Resume.Pwa.csproj -c Release -o ./deploy/pwa
pwsh ./VRT.Resume.Pwa/serve-published.ps1
```

Use **`http://127.0.0.1:8080`** (script sets COOP/COEP for OPFS). One browser tab per origin.

### Lighthouse PWA audit

```powershell
pwsh ./VRT.Resume.Pwa/run-lighthouse.ps1
```

Requires Node.js and Chrome or Edge. Target score: PWA ≥ 90. Alternatively: Chrome DevTools → Lighthouse on `http://127.0.0.1:8080` after `pwsh ./VRT.Resume.Pwa/serve-published.ps1`.

### Tests

```powershell
dotnet test VRT.Resume.Pwa.Tests/VRT.Resume.Pwa.Tests.csproj -c Debug
```

Plan and architecture: `plans/blazor-wasm-pwa-offline.md`, `AGENTS.md` (section **VRT.Resume.Pwa**).

## Deployed versions

### DEV version on Azure 
This version can be removed/changed at any time so use it for testing purposes only:<br/>
https://vrt-cv.azurewebsites.net/

