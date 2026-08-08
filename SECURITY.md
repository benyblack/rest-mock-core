# Security Policy

## Supported versions

| Version | Supported           |
|---------|---------------------|
| 0.9.x   | ✅ Active           |
| 0.8.x   | ⚠️ Best-effort only |
| < 0.8   | ❌ End of life      |

This library is small and low-surface; most releases do not require urgent security patches. That said, please report anything you find.

## Reporting a vulnerability

**Please do not open a public GitHub issue for security problems.** Public issues are visible to everyone, including attackers.

Use **GitHub Security Advisories** to report privately:

👉 **https://github.com/benyblack/rest-mock-core/security/advisories/new**

This creates a private thread that only the maintainer can see. You can discuss the issue, attach code, and coordinate a fix and disclosure timeline without exposing the vulnerability to the public.

If you cannot use GitHub Advisories, you can also email the maintainer — the contact address is the one on the maintainer's GitHub profile (https://github.com/benyblack).

## What to expect

- **Acknowledgement** within a few days.
- An assessment of impact and a fix plan.
- A coordinated disclosure: we agree on a date, you get credit in the advisory (unless you prefer anonymity), and we ship a patched release.
- If the issue is not actually a security problem (e.g. a behavior question), we will redirect you to a regular issue.

## Scope

`rest-mock-core` runs in the same process as your test code. The realistic security surface is:

- **Server binding:** the `HttpServer` constructor accepts a `hostname` and `port`. Binding to a non-loopback address exposes the mock to the network; the library will not warn about this. Be careful with shared CI runners.
- **Header / body matching:** all matching is exact string compare; no regex or eval surface, so no ReDoS or injection risk.
- **Dependencies:** only `Microsoft.AspNetCore.App` (framework reference) and the test-only `xunit`, `Moq`, `AutoFixture`. None of these run in the consumer's process at test time beyond what the test framework itself loads.
