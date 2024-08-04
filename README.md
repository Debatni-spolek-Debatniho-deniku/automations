# Automations

This repository contains .NET applications to automate processes in DSDD.

## DSDD.Automations.Payments

Application for fee payments administration.

### Run locally

Create `local.settings.json`.
```json
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",

    "AZURE_TENANT_ID": "From Entra: automations-dev",
    "AZURE_CLIENT_ID": "From Entra: automations-dev",
    "AZURE_CLIENT_SECRET": "Emit your own in Entra: automations-dev",

    "COSMOS_DB_ACCOUNT_ENDPOINT": "From payments-db-dev",

    "RBCZ_CLIENT_ID": "Copy from production Functions App in Azure Portal",
    "RBCZ_AUDIT_IP_ADDRESS": "255.255.255.255",
    "RBCZ_ACCOUNT_NUMBER": "1899051002",
    "RBCZ_ACCOUNT_CURRENCY": "CZK",
    "RBCZ_USE_SANDBOX_API": true,

    "RBCZ_BLOB_STORAGE_ACCOUNT": "automations1",
    "RBCZ_ACCOUNT_CERTIFICATE_PATH": "certificates/account_sandbox.p12",
    "RBCZ_ACCOUNT_CERTIFICATE_PASSWORD": "test12345678"
  }
```
For local development the [EnvironmentCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.environmentcredential) is used. In order to access Azure resource settings must contain `AZURE_TENANT_ID`, `AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET`. This service principal represents enterprise application.

### Deploy

Application is automatically deployed on every push to main branch.

For production the [ManagedIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.managedidentitycredential) is used. Production application is assigned it's own service principal in the Azure Portal that is different to enterprise application service principal and gets injected via `MSI_SECRET` env variable.

## DSDD.Automations.Reports

Application for generating user reports.

### Run locally

Create `local.settings.json`.
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    "AZURE_TENANT_ID": "From Entra: automations-dev",
    "AZURE_CLIENT_ID": "From Entra: automations-dev",
    "AZURE_CLIENT_SECRET": "Emit your own in Entra: automations-dev",

    "COSMOS_DB_ACCOUNT_ENDPOINT": "From payments-db-dev",

    "SPO_SITE_URL": "https://debatnispolek.sharepoint.com/",
    "SPO_MEMBERS_DOCUMENT_ID": "1F2E4D5C-11FB-4B2C-AC80-65853C589D78",

    "DEV_CERTIFICATE_THUMBPRINT": "see below"
  }
}
```

For local development the [EnvironmentCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.environmentcredential) is used. In order to access Azure resource settings must contain `AZURE_TENANT_ID`, `AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET`. This service principal represents enterprise application.

#### Certificate for SharePoint

SharePoint Online (SPO) cannot be accessed using clientId/clientSecret (EnvironmentCredential). For local development certificate must be used.

1. Create self signed certificate. You may use [this tutorial](https://learn.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azuread).
1. Open automations-dev app registration in Entra, certificates & secrets, and upload your certificate (.cert file).
1. Install the certificate on your computer (.pfx file).
1. Find the certificate in the windows certificate manager.
1. Copy the it's thumbprint to `DEV_CERTIFICATE_THUMBPRINT`.

### Deploy

Application is automatically deployed on every push to main branch.

For production the [ManagedIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.managedidentitycredential) is used. Production application is assigned it's own service principal in the Azure Portal that is different to enterprise application service principal and gets injected via `MSI_SECRET` env variable.