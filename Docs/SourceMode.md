# Source builds

Run `bash src/Kapusch.PostHogApisForiOSComponents/Native/iOS/build.sh`, then pack.
Inspect the dependency lock and packaged native resources. Source mode uses
`UseKapuschPostHogiOSInteropFromSource=true` and imports the matching
buildTransitive target explicitly when using a ProjectReference. All downloads
happen here, never in the consumer target.

Qualification toolchain: .NET SDK and workload set **10.0.203**. Install with
`dotnet workload install ios --version 10.0.203` rather than floating manifests.
Xcode 26.3 is selected in CI.

## Updating the PostHog iOS SDK

Pick the newest stable posthog-ios release whose `Package.swift` and sources
build with the pinned Xcode. Then update the `exact:` pin in `Package.swift`,
run `swift package resolve` in the package directory, copy the version and
revision into `DependencyLocks/posthog-ios.json`, and update the version in
`THIRD_PARTY_NOTICES.md` and `nuget-readme.md`. `scripts/verify-dependency-lock.py`
checks that they agree. Read the upstream CHANGELOG, re-check that the
`persistOptOut` SPI still exists, and re-verify [consent](Consent.md) against
the new source.
