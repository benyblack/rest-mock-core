# Changelog

All notable changes to **rest-mock-core** are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and the project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- `CONTRIBUTING.md` — contribution guide (workflow, scope rules, coding style).
- `SECURITY.md` — supported versions and private disclosure process via GitHub Security Advisories.
- Issue templates (`.github/ISSUE_TEMPLATE/`): bug report, feature request, question, plus a config that points users to Discussions for open-ended conversation.
- `.github/CODEOWNERS` — auto-request review from the maintainer on all PRs.

## [0.9.1] - 2026-08-08

### Fixed
- `dotnet pack` was emitting a "License missing" warning on 0.9.0. Set
  `<PackageLicenseExpression>MIT</PackageLicenseExpression>` so the
  SPDX expression is published and the warning goes away.

## [0.9.0] - 2026-08-07

### Changed
- Multi-target `net8.0` and `net10.0` (was `net10.0` only).
- Enabled nullable reference types across the library; cleaned up null annotations.
- `RouteTableItem.Verify` failures now throw `VerificationException` (was `System.Exception`).

### Fixed
- Reverted a regression in `HttpServer.Run` that made `HandleRouteNotFound` an
  `async void` method (could swallow / crash on unobserved exceptions).
- Restored `Interlocked.Increment` for the call counter so concurrent requests
  no longer race on the read-modify-write.
- Tests that bind a real `HttpServer` now run inside an xUnit collection so
  the hardcoded port does not collide when tests run in parallel.

### Removed
- The legacy `src/RestMockCore/rest-mock-core.nuspec` is no longer used; the
  .csproj already supplies all NuGet metadata. File moved to `.obsolete/` for
  reference.

## [0.8.0] - 2025-11-14

### Added
- Upgrade to .NET 10.0.
- `VerifyAll` and individual `Verify(...)` overloads on `RouteTableItem`,
  including a `Func<int, bool>` overload that integrates with Moq's `Times`.
