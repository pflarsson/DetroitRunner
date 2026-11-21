# CLAUDE.md

This file gives Claude Code guidance for working in this repository.

## Project overview

DetroitRunner is a small .NET 10 solution (`DetroitRunner.sln`) with three projects:

- **DetroitRunner**: a worker-style console app that uses [Lamar](https://jasperfx.github.io/lamar/) for dependency injection to discover and run `IStartable` implementations.
- **DetroitRunner.Web**: a small ASP.NET Core Razor Pages app with one page that shows last month's paycheck, using sample data.
- **DetroitRunner.Tests**: an xUnit project of algorithm-practice exercises (TwoSum, StringReversal, MaximumSubarraySum) plus tests for the paycheck logic. It references `DetroitRunner.Web`, but not `DetroitRunner`.

## Structure

```
DetroitRunner.sln
Dockerfile                  # multi-stage build (dotnet/sdk:10.0 -> dotnet/runtime:10.0), runs as $APP_UID
compose.yaml                # single "detroitrunner" service (see Known issues)
.dockerignore
DetroitRunner/
  DetroitRunner.csproj      # Exe, net10.0, Lamar 16.0.0
  Program.cs                # composition root: builds the Lamar container, runs startables
  IStartable.cs             # the extension point: Task StartAsync()
  StartableOne.cs           # contains SimpleStartable (file/class name mismatch)
DetroitRunner.Web/
  DetroitRunner.Web.csproj  # Microsoft.NET.Sdk.Web, net10.0, no extra packages
  Program.cs                # registers Razor Pages + paycheck services (built-in DI, not Lamar)
  Pages/Shared/_Layout.cshtml  # page shell: left sidebar nav + main content; add new pages to the sidebar here
  Pages/Index.cshtml(.cs)   # the paycheck page (page-specific CSS in the "Styles" section)
  Paychecks/
    Paycheck.cs             # Paycheck + Deduction records; NetPay = GrossPay - deductions
    IPaycheckSource.cs      # data source seam: GetForMonth(year, month)
    SamplePaycheckSource.cs # demo data: $5,000/month, paid on the last day of the month
    PaycheckService.cs      # picks the previous calendar month using TimeProvider
DetroitRunner.Tests/
  DetroitRunner.Tests.csproj  # xUnit 2.9.2, global `using Xunit`, references DetroitRunner.Web
  PaycheckServiceTests.cs
  TwoSumTests.cs
  StringReversalTests.cs
  MaximumSubarraySumTests.cs
  mytestarea.cs             # scratch / in-progress work (class `testarea`)
Claude/                     # planning notes for Claude Code sessions
```

## Commands

Requires the **.NET 10 SDK**. All projects target `net10.0`; building with only an SDK 9.x fails with `NETSDK1045`. The repo has no `global.json`.

Run from the repo root:

```sh
dotnet build DetroitRunner.sln                              # build everything
dotnet test DetroitRunner.Tests/DetroitRunner.Tests.csproj  # run tests
dotnet test --filter "FullyQualifiedName~TwoSumTests"       # run a single test class
dotnet run --project DetroitRunner/DetroitRunner.csproj     # run the worker (never exits; Ctrl+C to stop)
dotnet run --project DetroitRunner.Web                      # run the paycheck site at http://localhost:5179
docker build -t detroitrunner .                             # build the container image
docker compose up --build                                   # see Known issues: Dockerfile path
```

## Architecture

- `Program.Main` builds a Lamar `Container` that scans the assembly containing `SimpleStartable` and registers every `IStartable` implementation (`scanner.AddAllTypesOf<IStartable>()`).
- To add a new background service, create a class implementing `IStartable` in the `DetroitRunner` assembly; the scan picks it up automatically.
- `Main` resolves all startables and **awaits each one sequentially**, then prints `Worker running...` and blocks forever on `Task.Delay(-1)`. A startable's `StartAsync` must therefore return promptly (start background work and return) or it blocks every startable after it. `SimpleStartable` shows the pattern: `StartAsync` fires off a private `async` loop and returns `Task.CompletedTask`.

### DetroitRunner.Web

- The page asks `PaycheckService` for last month's paycheck. The service takes today's date from the injected `TimeProvider`, goes back one calendar month, and calls `IPaycheckSource.GetForMonth`.
- `SamplePaycheckSource` is the only source for now. To use real payroll data, implement `IPaycheckSource` and change its registration in `DetroitRunner.Web/Program.cs`.
- In tests, get "today" from `TimeProvider`, never `DateTime.Now`. `PaycheckServiceTests` uses a small `FixedTimeProvider` to pin the date.
- The page has **no authentication**, even though paychecks are sensitive. It should sit behind the proposed OAuth login server (AUTH requirements in `Claude/requirements.md`) before it shows real data.

## Known issues

This section records findings from a code review so future sessions don't rediscover them. Update or remove entries as they are fixed.

1. **No cancellation or graceful shutdown.** Neither `IStartable` nor `Program` uses a `CancellationToken`, and nothing handles SIGTERM/Ctrl+C, even though the app is built to run in Docker. Consider moving to the Generic Host (`BackgroundService`) or passing a token through `StartAsync`.
2. **Dead commented-out code** in `Program.cs`: `// scanner.AssemblyContainingType<RabbitListener>();` refers to a type that doesn't exist in the project.
3. **File/type name mismatch**: `StartableOne.cs` contains `SimpleStartable`. `SimpleStartable` is also declared in the global namespace, not `DetroitRunner`.
4. **`compose.yaml` points to the wrong Dockerfile path**: it uses `dockerfile: DetroitRunner/Dockerfile`, but the Dockerfile is at the repo root, so `docker compose up --build` fails until one of them is moved or updated.
5. **`DetroitRunner.Tests/mytestarea.cs` is an incomplete duplicate of `TwoSumTests.cs`**: it copies the `TwoSum` method, but the `[Theory]` body is empty and has only one `[InlineData]` case, so it always passes without verifying anything. The class name `testarea` also breaks PascalCase convention.

## Conventions

- Both projects enable `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>`; use file-scoped namespaces (`namespace DetroitRunner.Tests;`).
- Tests use xUnit `[Theory]` + `[InlineData]` for cases and `[Fact]` for single scenarios such as exception paths.
- Each exercise test class contains its reference implementation as a `private static` method, with a short comment explaining the algorithm and its time/space complexity. Follow the pattern in `TwoSumTests.cs` and `MaximumSubarraySumTests.cs`.
- Test method names follow `Method_ExpectedBehavior` (for example, `TwoSum_ThrowsWhenNoPairSumsToTarget`).

## Azure
- [1] *  N/A(tenant level account)  cb2fb6b1-571c-4381-9ada-644edcb4916d  cb2fb6b1-571c-4381-9ada-644edcb4916d

- PS C:\dev\LamarRunners\DetroitRunner> az group create -l eastus -n protea
{
  "id": "/subscriptions/4cc9f1b9-4414-40e6-8e40-019640f50899/resourceGroups/protea",
  "location": "eastus",
  "managedBy": null,
  "name": "protea",
  "properties": {
    "provisioningState": "Succeeded"
  },
  "tags": null,
  "type": "Microsoft.Resources/resourceGroups"
}
