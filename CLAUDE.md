# DSDD Automations

Automation suite for DSDD (Czech debate organization). Four .NET 8 Azure Functions apps handling payment administration, report generation, member data access, and Discord notifications.

## Build & Test

```bash
# Build entire solution
dotnet build DSDD.Automations.sln

# Build single app
dotnet build DSDD.Automations.Payments/DSDD.Automations.Payments.csproj

# Run all tests
dotnet test DSDD.Automations.sln

# Run specific test project
dotnet test DSDD.Automations.Payments.Tests/DSDD.Automations.Payments.Tests.csproj
```

## Architecture

### Function Apps (entry points)

- **DSDD.Automations.Payments** — Bank payment import (RBCZ timer-triggered durable orchestration) + Razor web UI for payer/payment management. Deploys to `dsdd-payments-functions`.
- **DSDD.Automations.Members** — Centralized member data HTTP API backed by SharePoint Online. Isolates the complex SPO/IAM setup so other services consume members via HTTP. Deploys to `dsdd-members-functions`.
- **DSDD.Automations.Reports** — Generates Excel reports (ClosedXML), emails them via Office 365 SMTP. Timer-triggered durable orchestration. Currently still accesses SPO directly for member data (will switch to Members service via HTTP once validated). Deploys to `dsdd-reports-functions`.
- **DSDD.Automations.DiscordMessages** — Posts scheduled messages to Discord via webhooks. Timer-triggered. Deploys to `dsdd-discordmessages-functions`.

### Shared Libraries

| Library | Purpose |
|---------|---------|
| `Hosting` | Azure Functions host config, Razor rendering, middleware (auth, errors), App Insights, sister apps |
| `Members.Abstranctions` | Member data interfaces (note: typo in name is intentional — do not rename) |
| `Members.SharePoint` | SharePoint Online member list access via PnP Core (used by Members app) |
| `Members.Http` | HTTP client for the Members service (used by consumers like Reports) |
| `Mailing.Abstractions` | Email sending interfaces |
| `Mailing.MailKit` | SMTP implementation (Office 365) |
| `Payments.Helpers` | Numeric symbol parsing |
| `Payments.Banking.Abstractions` | Bank importer interface |
| `Payments.Banking.RBCZ` | Raiffeisenbank CZ Premium API client (certificate auth) |
| `Payments.Persistence.Abstractions` | Data access interfaces (payers, unpaired payments) |
| `Payments.Persistence.Cosmos` | Cosmos DB implementation |

### Test Projects

NUnit + Moq. Test project detection is automatic via `Directory.Build.props` — any project ending in `.Tests` gets test dependencies injected.

- `DSDD.Automations.Payments.Tests`
- `DSDD.Automations.Payments.Helpers.Tests`
- `DSDD.Automations.Payments.Banking.RBCZ.Tests` (csproj named `DSDD.Automations.Payments.RBCZ.Tests.csproj`)
- `DSDD.Automations.Reports.Tests`
- `DSDD.Automations.Hosting.Tests`

## Key Conventions

- **.NET 8**, nullable reference types enabled, implicit usings enabled, code analysis enforced (`Directory.Build.props`)
- **Central package management** via `Directory.Packages.props` — add package versions there, not in individual csproj files
- **Azure Functions v4** isolated worker model (`dotnet-isolated`)
- **Durable Tasks** for multi-step orchestrations (payment import, report generation)
- **Razor views** rendered server-side for web UI (Payments app)
- **Czech locale** (`cs-CZ`) used throughout for date/number formatting
- **Auth**: `EnvironmentCredential` locally, `ManagedIdentityCredential` in production
- Config via `local.settings.json` (git-ignored) for local dev

## Deployment

GitHub Actions auto-deploys each app on push to `main`:
- `.github/workflows/payments-deploy.yml`
- `.github/workflows/reports-deploy.yml`
- `.github/workflows/members-deploy.yml`
- `.github/workflows/discordmessages-deploy.yml`

Each workflow builds the specific project and deploys via Azure Functions publish profile stored in GitHub secrets.
