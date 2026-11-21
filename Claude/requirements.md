# DetroitRunner Requirements

What the DetroitRunner solution must do, based on the current code (see `../CLAUDE.md`). Each requirement has a status: **Met**, **Partial**, or **Not met**. Most "Not met" items in sections 1–4 and 7 map to the Known issues in `CLAUDE.md`. Section 5 is a proposed feature, and section 6 is the paycheck web app.

## 1. Environment

| ID | Requirement | Status |
|----|-------------|--------|
| ENV-1 | Build with the .NET 10 SDK; both projects target `net10.0`. | Met (SDK 10.0.401 installed) |
| ENV-2 | Use Lamar 16.0.0 for dependency injection in the app. | Met |
| ENV-3 | Use xUnit 2.9.x with `Microsoft.NET.Test.Sdk` in the test project. | Met |
| ENV-4 | Enable nullable reference types and implicit usings in both projects. | Met |
| ENV-5 | Docker (optional) for building and running the container image. | Met |

## 2. Worker app (`DetroitRunner`)

| ID | Requirement | Status |
|----|-------------|--------|
| APP-1 | Provide an `IStartable` interface with `Task StartAsync()` as the extension point for background services. | Met |
| APP-2 | Find every `IStartable` in the `DetroitRunner` assembly automatically with a Lamar assembly scan, so no manual registration is needed. | Met |
| APP-3 | Start every discovered startable when the app launches. | Met |
| APP-4 | `StartAsync` must return promptly and run long work in the background, so one startable never blocks the ones after it. | Met (fixed in `SimpleStartable`) |
| APP-5 | After all startables have started, print `Worker running...` and keep the process alive until it is stopped. | Met |
| APP-6 | `SimpleStartable` writes `SimpleStartable has started. {count}` to the console once a minute, and the count goes up each time. | Met |
| APP-7 | Shut down cleanly on SIGTERM or Ctrl+C by passing cancellation to startables and letting them stop. | Not met (Known issue 1) |
| APP-8 | Report unhandled exceptions from background startable work instead of silently dropping them. | Not met |

## 3. Container

| ID | Requirement | Status |
|----|-------------|--------|
| CTR-1 | Build with a multi-stage Dockerfile: `dotnet/sdk:10.0` to build and publish, `dotnet/runtime:10.0` to run. | Met |
| CTR-2 | Run the container as the non-root `$APP_UID` user. | Met |
| CTR-3 | `docker compose up --build` builds and starts the `detroitrunner` service. | Not met: `compose.yaml` points to the wrong Dockerfile path (Known issue 4) |

## 4. Tests (`DetroitRunner.Tests`)

| ID | Requirement | Status |
|----|-------------|--------|
| TST-1 | Each algorithm exercise is its own test class with a `private static` reference implementation and a comment on its time and space complexity. | Met |
| TST-2 | Use `[Theory]` + `[InlineData]` for input/output cases and `[Fact]` for single scenarios such as exception paths. | Met |
| TST-3 | Cover TwoSum, string reversal, and maximum subarray sum (Kadane's algorithm), including edge cases (empty input, single element, all negatives, no solution). | Met |
| TST-4 | Every test asserts something; no empty test bodies. | Not met: `mytestarea.cs` has an empty `[Theory]` (Known issue 5) |
| TST-5 | `dotnet test` passes with no failures. | Met (19/19) |

## 5. OAuth login server (proposed)

A new capability: an OAuth 2.0 / OpenID Connect login server that issues tokens to clients.

| ID | Requirement | Status |
|----|-------------|--------|
| AUTH-1 | Add an OAuth 2.0 login server that lets users sign in and issues access tokens to registered clients. | Not met (new) |
| AUTH-2 | Support the Authorization Code flow with PKCE for interactive user login. | Not met (new) |
| AUTH-3 | Support the Client Credentials flow for service-to-service access. | Not met (new) |
| AUTH-4 | Publish OpenID Connect discovery (`/.well-known/openid-configuration`) and a JWKS endpoint so clients can validate tokens. | Not met (new) |
| AUTH-5 | Issue signed JWT access tokens with a configurable lifetime, and support refresh tokens. | Not met (new) |
| AUTH-6 | Keep secrets (signing keys, client secrets) out of source control and load them from configuration or environment variables. | Not met (new) |
| AUTH-7 | Serve only over HTTPS outside local development. | Not met (new) |
| AUTH-8 | Run in Docker alongside `detroitrunner` as its own `compose.yaml` service. | Not met (new) |
| AUTH-9 | Cover the login and token flows with automated tests. | Not met (new) |

**Open questions**
- **Build or buy:** host the server in this solution (for example, ASP.NET Core with OpenIddict or Duende IdentityServer), or use an external provider (Entra ID, Auth0, Keycloak) and only integrate with it?
- **Where users come from:** a local user store (ASP.NET Core Identity) or an external identity provider (Google, Microsoft)?
- **What it protects:** `DetroitRunner` is a console worker with no HTTP endpoints today, so which clients or APIs will use these tokens?

## 6. Paycheck web app (`DetroitRunner.Web`)

| ID    | Requirement                                                                                                                                  | Status |
|-------|----------------------------------------------------------------------------------------------------------------------------------------------|--------|
| PAY-1 | Provide a small ASP.NET Core web app that shows the user's paycheck for the previous calendar month.                                         | Met |
| PAY-2 | Show the pay date, pay period, gross pay, each deduction, total deductions, and net pay, formatted as US dollars.                            | Met |
| PAY-3 | Work out "last month" correctly across year boundaries and shorter months (for example, January goes back to December of the previous year). | Met (tested) |
| PAY-4 | Keep the paycheck data source behind an interface (`IPaycheckSource`) so sample data can be replaced with a real payroll integration.        | Met |
| PAY-5 | Show a clear message when no paycheck exists for last month.                                                                                 | Met |
| PAY-6 | Load paycheck data from a real payroll source instead of `SamplePaycheckSource`.                                                             | Not met |
| PAY-7 | Require login (see AUTH-1) and show only the signed-in user's own paycheck.                                                                  | Not met: the page is currently public |
| PAY-8 | Run in Docker as its own `compose.yaml` service.                                                                                             | Not met |
| PAY-9 | Add a left hand side bar                                                                                                                     | Met (shared `_Layout.cshtml`; collapses to a top bar under 640px) |


## 7. Code quality

| ID | Requirement | Status |
|----|-------------|--------|
| QLT-1 | File names match the type they contain, and types live in the project namespace. | Not met: `StartableOne.cs` contains `SimpleStartable` in the global namespace (Known issue 3) |
| QLT-2 | No dead or commented-out code that refers to types that don't exist. | Not met: `RabbitListener` line in `Program.cs` (Known issue 2) |
| QLT-3 | The solution builds with no errors. | Met (4 warnings) |
