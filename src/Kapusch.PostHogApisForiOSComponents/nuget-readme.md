# Kapusch.PostHog.iOS

Thin interop for PostHog iOS 3.92.5. Upstream manages persistence and retries.
Initialize once on the main thread; reset between accounts. Flush is not a receipt.
Capture is enabled by default; set `NativePostHogOptions.OptOut` to start disabled
and call `OptIn()` after consent. Replay is disabled and not qualified. See the
repository integration and consent guides.
