# Source builds

Run `bash src/Kapusch.PostHogApisForiOSComponents/Native/iOS/build.sh`, then pack.
Inspect the dependency lock and packaged native resources. Source mode uses
`UseKapuschPostHogiOSInteropFromSource=true` and imports the matching
buildTransitive target explicitly when using a ProjectReference. All downloads
happen here, never in the consumer target.
