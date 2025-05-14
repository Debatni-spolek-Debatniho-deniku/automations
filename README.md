# Automations

This repository contains .NET applications to automate processes in DSDD.

[UML diagram](https://debatnispolek.sharepoint.com/:u:/r/Shared%20Documents/IT%20komise/Automatizace%20proces%C5%AF/Spr%C3%A1va%20plateb%20a%20reporty.vsdx?d=wf2d58d606e964fe0bf85d1f024a5466f&csf=1&web=1&e=gsjnz1)

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

    "SPO_MEMBERS_WORKSHEET": "Členové",
    "SPO_MEMBERS_HEADER_SIZE": 2,
    "SPO_MEMBERS_FIRST_NAME_COL": "A",
    "SPO_MEMBERS_LAST_NAME_COL": "B",
    "SPO_MEMBERS_VARIABLE_SYMBOL_COL": "C",
    "SPO_MEMBERS_ENLISTED_COL": "D",

    "MEMBER_FEES_FROM_MONTH": 7,
    "MEMBER_FEES_FROM_YEAR": 2024,
    "MEMBER_FEE_CZK": 100,
    "MEMBER_FEES_CONSTANT_SYMBOL": 100,

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

SharePoint Online (SPO) cannot be accessed using clientId/clientSecret (EnvironmentCredential). For local development certificate must be used.

1. Create self signed certificate. You may use [this tutorial](https://learn.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azuread).
1. Open automations-dev app registration in Entra, certificates & secrets, and upload your certificate (.cert file).
1. Install the certificate on your computer (.pfx file).
1. Find the certificate in the windows certificate manager.
1. Copy the it's thumbprint to `DEV_CERTIFICATE_THUMBPRINT`.

#### Email

Email address used as sender must be accessible by the principal that this application runs as.

### Deploy

Application is automatically deployed on every push to main branch.

For production the [ManagedIdentityCredential](https://learn.microsoft.com/en-us/dotnet/api/azure.identity.managedidentitycredential) is used. Production application is assigned it's own service principal in the Azure Portal that is different to enterprise application service principal and gets injected via `MSI_SECRET` env variable.

## DSDD.Automations.DiscordMessages

Periodically posts to a disocrd channel via webhook.

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

### Deploy

Application is automatically deployed on every push to main branch.
