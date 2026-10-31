# PostHogApisForiOSComponents

Public .NET 10 iOS interop for the official PostHog mobile SDK.
NuGet ID: **Kapusch.PostHog.iOS**. The next release is 1.0.0-rc.1 (0.1.0 was a qualification build).

The thin wrapper exposes setup, identify/reset, capture, flush, distinct ID and
opt-in/out, including starting opted out until consent. Persistence, batching,
retry and lifecycle belong to PostHog, with its bounded queue and platform
storage limits. A returned call is not proof of
network delivery. Replay, surveys and feature flags are not exposed or qualified.

See [integration](Docs/Integration.md), [consent](Docs/Consent.md),
[source builds](Docs/SourceMode.md), [qualification](Docs/Qualification.md), [release workflow](Docs/Release.md), [samples](samples/README.md), [changelog](CHANGELOG.md) and
[third-party licenses](THIRD_PARTY_NOTICES.md).

Build native artifacts with `bash src/Kapusch.PostHogApisForiOSComponents/Native/iOS/build.sh`, then:

```sh
dotnet pack src/Kapusch.PostHogApisForiOSComponents/Kapusch.PostHogApisForiOSComponents.csproj
-c Release -o artifacts/nuget
```

Consumer builds do not download native SDKs.

## CI

- PR CI (`build.yml`) scans the public tree, builds the native wrapper, packs, verifies the package and builds the sample consumer.
- Publishing is handled by `.github/workflows/publish.yml` with channel routing:
  - tag `vX.Y.Z` on `main` -> NuGet.org (stable)
  - tag `vX.Y.Z-rc.N` on `main` -> NuGet.org (pre-release)
  - non-tag runs (`workflow_dispatch`) -> GitHub Packages (`X.Y.Z-preview.<run>.<sha>`)
  - `workflow_dispatch` on `main` with `manual_version` -> NuGet.org (forced version)
- NuGet.org publishing uses NuGet Trusted Publishing (OIDC via `NuGet/login@v1`), no long-lived NuGet API key.

### Required GitHub secret

- `NUGET_USER`: your nuget.org profile username (not email), used by `NuGet/login@v1`.

See [Docs/Release.md](Docs/Release.md) for the release sequence. Historical tags remain immutable even when obsolete branches are deleted.
