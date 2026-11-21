# Create CLAUDE.md for DetroitRunner (with review findings as a worked example)

## Context

The user asked to "review solution." Exploration showed `DetroitRunner.sln` is a small .NET 10 worker-service solution: a console app (`DetroitRunner`) that uses Lamar for DI to discover and run `IStartable` implementations, plus an xUnit test project (`DetroitRunner.Tests`) containing algorithm-practice exercises (TwoSum, StringReversal, MaximumSubarraySum) and an in-progress duplicate (`mytestarea.cs`).

After scoping questions, the user redirected the goal: rather than a plain findings report or an immediate code fix, they want a **CLAUDE.md** created for this repo — both as real project documentation for future Claude Code sessions, and as a learning example, with the DetroitRunner review findings folded in to illustrate how a "known issues" section works in practice. No source code will be changed in this task.

There is no existing CLAUDE.md and no git repository, so this is a fresh file creation at the repo root.

## What goes in CLAUDE.md

Write `C:\Dev\LamarRunners\DetroitRunner\CLAUDE.md` with these sections:

1. **Project overview** — a small .NET 10 solution: `DetroitRunner` (worker console app using Lamar DI to run `IStartable` services) and `DetroitRunner.Tests` (xUnit algorithm-practice exercises).

2. **Structure**
   - `DetroitRunner/` — the app: `Program.cs` (composition root, Lamar container, scans for `IStartable`), `IStartable.cs` (the extension point), `StartableOne.cs` (contains `SimpleStartable`, note the file/class name mismatch).
   - `DetroitRunner.Tests/` — xUnit tests: `TwoSumTests.cs`, `StringReversalTests.cs`, `MaximumSubarraySumTests.cs`, and `mytestarea.cs` (scratch/in-progress work, currently a duplicate of `TwoSumTests.cs` with an empty test body).
   - `compose.yaml` / `DetroitRunner/Dockerfile` — multi-stage Docker build (`dotnet/sdk:10.0` → `dotnet/runtime:10.0`), runs as `$APP_UID`.

3. **Build / test / run commands** (derived from the `.csproj`/`Dockerfile`/`compose.yaml` contents read during exploration):
   - Build: `dotnet build DetroitRunner.sln`
   - Test: `dotnet test DetroitRunner.Tests/DetroitRunner.Tests.csproj`
   - Run: `dotnet run --project DetroitRunner/DetroitRunner.csproj`
   - Docker: `docker compose up --build`

4. **Architecture notes** — Lamar container in `Program.cs` scans the assembly containing `SimpleStartable` and registers all `IStartable` implementations; `Program.Main` awaits each startable sequentially, then blocks forever on `Task.Delay(-1)`.

5. **Known issues (worked example section)** — present as an example of documenting findings for future sessions, drawn from this review of `DetroitRunner/StartableOne.cs` and `Program.cs`:
   - `SimpleStartable.StartAsync()` runs a synchronous `while (true)` loop with a blocking `Task.Delay(...).Wait()` — it never returns, so in `Program.cs`'s sequential `foreach (var startable in startables) await startable.StartAsync();`, any startable registered after this one would never start.
   - No `CancellationToken` / graceful shutdown support anywhere, despite the app being built for Docker (no handling of container stop signals).
   - `Program.cs` has a dead commented-out line (`// scanner.AssemblyContainingType<RabbitListener>();`) referencing a type that doesn't exist in the project.
   - File name (`StartableOne.cs`) doesn't match the type it contains (`SimpleStartable`) — inconsistent with normal C# convention.
   - `DetroitRunner.Tests/mytestarea.cs` is a partial duplicate of `TwoSumTests.cs`: the `TwoSum` method is copied in but the `[Theory]` test body is empty and only has one `[InlineData]` case, so it doesn't actually verify anything yet.

6. **Conventions** — nullable + implicit usings enabled on both projects; tests use xUnit `[Theory]`/`[InlineData]` with a static reference implementation embedded in the test class itself (see `TwoSumTests.cs`, `StringReversalTests.cs`, `MaximumSubarraySumTests.cs` for the established pattern).

## Verification

- Confirm `CLAUDE.md` renders as valid Markdown and file paths/commands referenced in it are accurate against the current repo layout.
- No build/test run is needed since no source files change; optionally run `dotnet build DetroitRunner.sln` to double check the documented build command works.
