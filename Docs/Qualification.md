# Qualification gates

Build device and simulator slices. Pack, restore into a clean consumer,
and link an optimized release. Inspect native symbols, bundled licenses and
privacy resources, and scan the public Git tree for private identifiers.

On iOS devices and simulators, test anonymous-to-account, logout and A-to-B switches, offline
capture, kill/restart/reconnect, bounds/overflow, opt-out and duplicate business
IDs. Reconcile received events and timestamps on the server. Native disk queues
reduce loss but are not unlimited storage or exactly-once delivery. Cache
purging/uninstall and queue bounds may discard data. No replay claim until tested.

## Local evidence — 2026-10-08

Native compilation and packed NuGet Release sample build passed locally on
.NET 10.0.203. iOS native slices: device arm64 and simulator arm64/x64.
(The original entry also listed an Android build; this repository ships only
the iOS package, so that line is historical and not part of its evidence.)
This proves package linking only; phone ingestion/restart/identity tests remain
pending. No NuGet.org publication or production readiness is claimed.

Runtime sample passed on iOS 18.3 x64 simulator. Native setup/capture/flush ABI
resolved, the anonymous ID survived process termination/relaunch, and all three
original queued records remained on disk afterward (five total after relaunch).
Loopback endpoint intentionally refused connections, so this proves local retry
and persistence only, not ingestion or return-to-network delivery. The two
upstream privacy manifests are present in the packed consumer app bundle.

The packed sample also linked for unsigned iOS ARM64 device. GitHub Actions now
passes native build, package layout and optimized simulator consumer with the
pinned 10.0.203 workload set and Xcode 26.3. The first floating-workload run
failed on the Xcode version requirement; the explicit pin resolves that mismatch.

## Local evidence — 2026-10-27 (SDK 3.92.5, 1.0.0-rc.1)

Native slices built without warnings on Xcode 26.3 (Swift 6.2.4); the packed
package passed `verify-package.py`, whose native symbol check found all ten
managed entry points in both slices, and the packed sample built for the x64
simulator on .NET 10.0.203.

On an iOS 26.3 simulator, the sample started opted out (`IsOptOut` true) with
`PersistOptOut = false`, then opted in. The disk queue held Application
Installed, Application Opened and the post-consent event, all tagged
`$lib_version` 3.92.5. The event captured before consent was absent, and no
`posthog.optOut` record was written.

A temporary probe, not committed, used the default options: it called
`OptOut()`, `Reset()`, then `OptOut()` again. `IsOptOut` stayed true in that
process, but no `posthog.optOut` record remained. The next launch started opted
in and queued Application Opened under the new anonymous ID. This confirms the
`Reset()` caveat in [consent](Consent.md). Loopback refused all connections, so
nothing here shows ingestion.

## Manual publication

Releases are manual: there is no tag trigger. The `publish.yml` workflow runs
only through workflow_dispatch on `main` and publishes the version declared in
the project (`VersionPrefix` plus `VersionSuffix`, currently `1.0.0-rc.1`). It
creates no Git tag or GitHub release; tag the published commit by hand if needed.

After device qualification, configure a NuGet.org Trusted Publishing policy
for this repository and workflow `publish.yml`, using the NuGet profile `Kapusch`.
The job uses GitHub OIDC through `NuGet/login@v1`; no permanent API key is required
scoped to this package on NuGet.org. Run Publish verified NuGet package on main.
The workflow rebuilds, scans, checks the dependency lock, packs, checks native
entry points and links a consumer before publishing. Never overwrite a published
version: for a later candidate bump `VersionSuffix` (`rc.2`); for the final
release remove `VersionSuffix`. Update the sample's PackageReference to match;
`verify-package.py` fails otherwise. No publication runs automatically on push.
Device qualification remains a human-reviewed prerequisite.
