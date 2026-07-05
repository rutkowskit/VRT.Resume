# VRT.Resume.Mvc

Legacy ASP.NET Core MVC host for the CV generator. Users sign in via OAuth (Google / GitHub), store data in SQL Server or SQLite, and edit resumes in the browser.

> **Status:** maintenance-only. The active UI is [`VRT.Resume.Pwa`](../VRT.Resume.Pwa/Readme.md). Change MVC only when required (bugfix, security, shared-layer dependency).

Solution overview: [`../README.md`](../README.md). Architecture: [`../AGENTS.md`](../AGENTS.md).

## Supported auth providers

### Sign in with Google

To add OAuth 2.0 client IDs:

1. Open https://console.cloud.google.com/apis/credentials
1. Log in to your Google account
1. Click **+ Create Credentials** → **OAuth client ID**
1. **Application type:** Web application
1. Name e.g. **Local-VRT-Resume**
1. **Authorized redirect URIs** → **+ Add URI**
1. Add local URI `https://localhost:5001/signin-google` (must match `Properties/launchSettings.json`)
1. Add other URIs if you use IIS or IIS Express instead of Kestrel
1. Click **Save** (keep **Client ID** and **Client secret** for configuration)
1. Open the solution in Visual Studio
1. Right-click `VRT.Resume.Mvc` → **Manage User Secrets**
1. Add Google provider configuration:

```json
"Auth": {
  "Providers": [
    {
      "Name": "Google",
      "ClientId": "<your client id>",
      "ClientSecret": "<your client secret>",
      "CallbackPath": "/signin-google"
    }
  ]
}
```

You can now sign in with Google credentials.

### Sign in with GitHub

1. Create a GitHub OAuth App: **Settings → Developer settings → OAuth Apps**
1. Set **Authorization callback URL** to `https://localhost:5001/signin-github` (local) or your production URL (see Azure section below)
1. Add a second entry under `Auth:Providers` in user secrets or App Service settings (same shape as Google), with `Name`: `Github`, `CallbackPath`: `/signin-github`, and your client ID / secret

## Publishing to Azure

The MVC app is hosted on **Azure App Service** (`VRT.Resume.Mvc`).

### 1. Create Azure resources

1. Sign in to the [Azure Portal](https://portal.azure.com/)
1. Create an **App Service** (Windows or Linux) with a runtime that supports **.NET 10**
1. *(Recommended for production)* Create **Azure SQL Database** and note the connection string
1. *(Optional, testing only)* Use **SQLite** with a file on the App Service file system

### 2. Configure OAuth redirect URIs

Add production callback URLs to your OAuth applications:

| Provider | Redirect URI |
|----------|--------------|
| Google   | `https://<your-app-name>.azurewebsites.net/signin-google` |
| GitHub   | `https://<your-app-name>.azurewebsites.net/signin-github` |

Replace `<your-app-name>` with your App Service host name.

### 3. Configure App Service application settings

**App Service → Settings → Environment variables** (or **Configuration → Application settings**):

| Setting | Example value | Notes |
|---------|---------------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Required for production error handling |
| `DbProvider` | `mssql` or `sqlite` | Must match the connection string you configure |
| `ConnectionStrings__ResumeData__mssql` | `Server=tcp:...` | When `DbProvider` is `mssql` |
| `ConnectionStrings__ResumeData__sqlite` | `Data Source=D:\home\site\data\resumedata.sqlite` | When `DbProvider` is `sqlite` |
| `Auth__Providers__0__Name` | `Google` | First auth provider |
| `Auth__Providers__0__ClientId` | `<client id>` | From Google Cloud Console |
| `Auth__Providers__0__ClientSecret` | `<client secret>` | Secret / Key Vault reference |
| `Auth__Providers__0__CallbackPath` | `/signin-google` | Must match OAuth app |
| `Auth__Providers__1__Name` | `Github` | Second provider (optional) |
| `Auth__Providers__1__ClientId` | `<client id>` | From GitHub OAuth app |
| `Auth__Providers__1__ClientSecret` | `<client secret>` | Secret / Key Vault reference |
| `Auth__Providers__1__CallbackPath` | `/signin-github` | Must match OAuth app |

The application creates and seeds the database on startup (`EnsureCreated`).

### 4. Build and publish locally

From the solution root:

```powershell
dotnet test --configuration Release
dotnet publish .\VRT.Resume.Mvc\VRT.Resume.Mvc.csproj --configuration Release --output .\deploy\web
```

Or use Cake (same output folder):

```powershell
dotnet tool restore
dotnet cake --target=PublishWeb
```

Publish output: `./deploy/web`.

### 5. Deploy to App Service

**Visual Studio**

1. Right-click `VRT.Resume.Mvc` → **Publish**
1. **Azure** → **Azure App Service (Windows/Linux)**
1. Select the target App Service and publish

**Azure CLI (zip deploy)**

```powershell
Compress-Archive -Path .\deploy\web\* -DestinationPath .\deploy\web.zip -Force
az webapp deploy --resource-group <resource-group> --name <app-name> --src-path .\deploy\web.zip --type zip
```

**GitHub Actions / CI/CD**

Add a deploy step after `dotnet publish` that uploads `./deploy/web` to App Service (e.g. `azure/webapps-deploy`).

### 6. Verify the deployment

1. Open `https://<your-app-name>.azurewebsites.net/`
1. Confirm sign-in with Google and/or GitHub
1. Create or edit a resume to verify database connectivity
1. Check **App Service → Log stream** or **Application Insights** if the app fails to start

## DEV environment (Azure)

Testing only — may be removed or changed at any time:

https://vrt-cv.azurewebsites.net/