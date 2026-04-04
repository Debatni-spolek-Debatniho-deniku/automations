# Automations

This repository contains .NET applications to automate processes in DSDD.

[UML diagram](https://debatnispolek.sharepoint.com/:u:/r/Shared%20Documents/IT%20komise/Automatizace%20proces%C5%AF/Spr%C3%A1va%20plateb%20a%20reporty.vsdx?d=wf2d58d606e964fe0bf85d1f024a5466f&csf=1&web=1&e=gsjnz1)

## Deployment

All apps are automatically deployed on every push to `main` via a single GitHub Actions workflow (`.github/workflows/deploy.yml`). All apps deploy using **RBAC with GitHub federated identity (OIDC)** — GitHub exchanges its OIDC token directly with Entra ID, no stored secrets needed.

### Setup

An Entra app registration is configured with a federated identity credential:
- **Issuer:** `https://token.actions.githubusercontent.com`
- **Subject:** `repo:<org>/<repo>:ref:refs/heads/main`
- **Audience:** `api://AzureADTokenExchange`

The app registration must have the **Website Contributor** role assigned on each Function App.

Three GitHub secrets are needed:

| Secret | Description |
|--------|-------------|
| `AZURE_CLIENT_ID` | App registration client ID (from Entra > App registrations > Overview) |
| `AZURE_TENANT_ID` | Entra tenant ID (from the same Overview page) |
| `AZURE_SUBSCRIPTION_ID` | Azure subscription ID (from the Function App > Overview) |

### Adding a new Function App

1. Create the Function App in Azure
2. Assign **Website Contributor** role to the existing app registration on the new Function App
3. Add the app to the matrix in `.github/workflows/deploy.yml`

### Runtime

For production the [ManagedIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.managedidentitycredential) is used. Production applications are assigned their own service principal in the Azure Portal that is different to the enterprise application service principal and gets injected via `MSI_SECRET` env variable.

## DSDD.Automations.Payments

Application for fee payments administration. Runs on **Linux Consumption**.

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

    "RBCZ_IMPORT_TIMER_CRON": "0 0 1 * * *",

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

## DSDD.Automations.Members

Centralized member data service. Exposes member information from SharePoint Online via an HTTP API (`GET /api/members`). Other services consume member data through this service instead of accessing SharePoint directly. Runs on **Flex Consumption**.

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

    "SPO_SITE_URL": "https://debatnispolek.sharepoint.com/",
    "SPO_MEMBERS_DOCUMENT_ID": "1F2E4D5C-11FB-4B2C-AC80-65853C589D78",

    "SPO_MEMBERS_WORKSHEET": "Členové",
    "SPO_MEMBERS_HEADER_SIZE": 2,
    "SPO_MEMBERS_FIRST_NAME_COL": "A",
    "SPO_MEMBERS_LAST_NAME_COL": "B",
    "SPO_MEMBERS_VARIABLE_SYMBOL_COL": "C",
    "SPO_MEMBERS_ENLISTED_COL": "D",
    "SPO_MEMBERS_FEE_PAYMENT_PERIOD_COL": "J",
    "SPO_MEMBERS_FEE_PAYMENT_PERIOD_ANNUALY_VALUE": "roční",
    "SPO_MEMBERS_FEE_PAYMENT_PERIOD_MONTHLY_VALUE": "měsíční",

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

## DSDD.Automations.Reports

Application for generating user reports. Runs on **Linux Consumption**.

> **Note:** Reports currently still accesses SharePoint Online directly for member data and requires the same SPO and certificate configuration as the Members service. Once the Members service is validated in production, Reports will be switched to consume member data via HTTP from the Members service instead.

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

    "SPO_MEMBERS_WORKSHEET": "Členové",
    "SPO_MEMBERS_HEADER_SIZE": 2,
    "SPO_MEMBERS_FIRST_NAME_COL": "A",
    "SPO_MEMBERS_LAST_NAME_COL": "B",
    "SPO_MEMBERS_VARIABLE_SYMBOL_COL": "C",
    "SPO_MEMBERS_ENLISTED_COL": "D",
    "SPO_MEMBERS_FEE_PAYMENT_PERIOD_COL": "J",
    "SPO_MEMBERS_FEE_PAYMENT_PERIOD_ANNUALY_VALUE": "roční",
    "SPO_MEMBERS_FEE_PAYMENT_PERIOD_MONTHLY_VALUE": "měsíční",

    "MEMBER_FEES_FROM_MONTH": 7,
    "MEMBER_FEES_FROM_YEAR": 2024,
    "MEMBER_FEE_ANNUALY_CZK": 1000,
    "MEMBER_FEES_ANNUALY_CONSTANT_SYMBOL": 1000,
    "MEMBER_FEE_MONTHLY_CZK": 1000,
    "MEMBER_FEES_MONTHLY_CONSTANT_SYMBOL": 1000,

    "MAILING_SENDER_EMAIL": "noreply@debatnispolek.debatnidenik.cz",
    "MAILING_SENDER_NAME": "No Reply",
    "MAILING_SMTP_ENDPOINT": "smtp.office365.com",
    "MAILING_SMTP_PORT": 587,
    
    "NOTIFY_TIMER_CRON": "0 0 4 1 * *",
    "NOTIFY_MEMBER_FEES_REPORT_RECIPIENT_EMAIL": "vybor@debatnispolek.debatnidenik.cz",
    "NOTIFY_MEMBER_FEES_REPORT_RECIPIENT_NAME": "Výbor",

    "DEV_CERTIFICATE_THUMBPRINT": "see below"
  }
}
```

For local development the [EnvironmentCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.environmentcredential) is used. In order to access Azure resource settings must contain `AZURE_TENANT_ID`, `AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET`. This service principal represents enterprise application.

#### Certificate for SharePoint

See the certificate setup instructions in the [Members](#dsddautomationsmembers) section above.

#### Email

Email address used as sender must be accessible by the principal that this application runs as.

## DSDD.Automations.DiscordMessages

Periodically posts to a disocrd channel via webhook. Runs on **Windows Consumption**.

### Run locally

Create `local.settings.json`.
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",


    "PRAHA_TIMER_CRON": "0 0 12 * * 5",
    "PRAHA_MESSAGE": "Message for Prague",
    "PRAHA_WEBHOOK_URL": "Prague channel webhook URL",

    "PLZEN_TIMER_CRON": "0 0 12 * * 6",
    "PLZEN_MESSAGE": "Message for Pilsen",
    "PLZEN_WEBHOOK_URL": "Pilsen channel webhook URL"
  }
}
```
