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


## Deployed versions

### DEV version on Azure 
This version can be removed/changed at any time so use it for testing purposes only:<br/>
https://vrt-cv.azurewebsites.net/

