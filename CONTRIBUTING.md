# Contributing to rest-mock-core

Thanks for taking an interest! 🎉

`rest-mock-core` is a small HTTP mock server for .NET test projects. We aim to keep it **small, reliable, and zero-dependency** (only `Microsoft.AspNetCore.App`).

## Ground rules

- **Keep the scope tight.** This library is intentionally minimal. If you want WireMock-level features, [WireMock.Net](https://github.com/WireMock-Net/WireMock.Net) exists for that. Open a discussion *before* opening a large feature PR so we can agree on the direction.
- **No new third-party dependencies** without prior discussion. Every dependency is a long-term maintenance cost for the community.
- **Bug fixes and hygiene PRs are always welcome.** Refactors, multi-target updates, nullable, packaging metadata — all good.
- **Tests are required** for any behavior change. xUnit is the framework; the existing `HttpServerCollection` is the place for tests that spin up a real server.

## Development setup

```bash
git clone https://github.com/benyblack/rest-mock-core.git
cd rest-mock-core
dotnet restore
dotnet build
dotnet test
```

The library targets `net8.0;net10.0`. You can build for just one TFM with `dotnet build -f net8.0` if you don't have the .NET 10 SDK locally.

## Workflow

1. **Fork** the repo and create a topic branch off `main`:
   - `fix/<short-description>` for bug fixes
   - `feat/<short-description>` for new features (after discussion)
   - `chore/<short-description>` for refactors, dependencies, packaging
2. Make your changes. Keep commits focused; small, reviewable commits are easier to merge.
3. **All tests must pass** locally: `dotnet test` should report 0 failures.
4. **Add a CHANGELOG.md entry** under the `[Unreleased]` section for any user-visible change.
5. Open a **pull request** against `main` with:
   - A clear title (Conventional Commits style is appreciated but not enforced)
   - A short description of the *why* (the *what* is the diff)
   - A note on how you tested it
6. Wait for review. The maintainer is one person with limited time — please be patient. If your PR is stuck after a couple of weeks, feel free to ping.

## Coding style

- C# 12+ features are fine. `<Nullable>enable</Nullable>` is on for the library.
- Match the existing style. The `.editorconfig` is the source of truth for formatting.
- Prefer explicit over clever. This library is read by people debugging their test fixtures at 2am.
- Keep public API surface minimal. Internal helpers are cheap; public types are forever.

## Reporting bugs

Use the **Bug report** issue template. Please include:
- `rest-mock-core` version
- Target framework (`net8.0` / `net10.0`)
- A minimal reproduction (a single `.cs` file is ideal)

## Suggesting features

Open a **Feature request** issue first. Maintainer will weigh in before any code is written.

## Security issues

**Do not open a public issue.** See [SECURITY.md](SECURITY.md) for the private disclosure process.

## Code of conduct

By participating, you agree to abide by the [Code of Conduct](CODE_OF_CONDUCT.md).
