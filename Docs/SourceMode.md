# Source builds

Run `bash src/Kapusch.PostHogApisForiOSComponents/Native/iOS/build.sh`, then pack.
Inspect the dependency lock and packaged native resources. Source mode uses
`UseKapuschPostHogiOSInteropFromSource=true` and imports the matching
buildTransitive target explicitly when using a ProjectReference. All downloads
happen here, never in the consumer target.

Qualification toolchain: .NET SDK and workload set **10.0.203**. Install with
`dotnet workload install ios --version 10.0.203` rather than floating manifests.
Xcode 26.3 is selected in CI.
