# Packed-package smoke sample

Build the native wrapper and pack it into `artifacts/nuget` first, then build
`samples/Smoke/Smoke.csproj`. This sample uses **PackageReference**, not project
references. It starts opted out, captures an event that must be dropped, opts
in, then exercises capture/flush using a loopback endpoint; no real project key
is committed and no data is sent to PostHog Cloud.

The sample is a packaging and startup check. Phone identity, offline restart and
delivery tests remain separate qualification gates in `Docs/Qualification.md`.
