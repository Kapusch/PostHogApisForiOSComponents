# Qualification gates

Build device and simulator/emulator slices. Pack, restore into a clean consumer,
and link an optimized release. Inspect native symbols, bundled licenses and
privacy resources, and scan the public Git tree for private identifiers.

On each platform test anonymous-to-account, logout and A-to-B switches, offline
capture, kill/restart/reconnect, bounds/overflow, opt-out and duplicate business
IDs. Reconcile received events and timestamps on the server. Native disk queues
reduce loss but are not unlimited storage or exactly-once delivery. Cache
purging/uninstall and queue bounds may discard data. No replay claim until tested.

## Local evidence — 2026-10-08

Native compilation and packed NuGet Release sample build passed locally on
.NET 10.0.203. iOS native slices: device arm64 and simulator arm64/x64. Android:
SDK 35, Java 17-compatible bytecode, optimized .NET Android consumer.
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

## Manual publication

After device qualification, configure a NuGet.org Trusted Publishing policy
for this repository and workflow `publish.yml`, using the NuGet profile `Kapusch`.
The job uses GitHub OIDC through `NuGet/login@v1`; no permanent API key is required
scoped to this package on NuGet.org. Run Publish verified NuGet package on main.
The workflow rebuilds, scans, packs and links a consumer before publishing the
version declared in the project. Never overwrite a published version; increment
the project and sample versions for subsequent releases. No publication runs
automatically on push. Device qualification remains a human-reviewed prerequisite.
