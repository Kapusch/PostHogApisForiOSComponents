# Kapusch.PostHog.iOS

Thin interop for PostHog iOS 3.92.5. Upstream manages persistence and retries.
Initialize once on the main thread; reset between accounts. Flush is not a receipt.
Capture is enabled by default; set `NativePostHogOptions.OptOut` to start disabled
and call `OptIn()` after consent. Replay is disabled and not qualified.

`Reset()` deletes the SDK's persisted opt-out and, even while opted out, sends a
feature flag request with the device ID; consent-gated apps should keep consent
in their own store (`PersistOptOut = false`) and call `Reset()` while opted out
only to clear an identified user.

See the [integration guide](https://github.com/Kapusch/PostHogApisForiOSComponents/blob/main/Docs/Integration.md),
the [consent guide](https://github.com/Kapusch/PostHogApisForiOSComponents/blob/main/Docs/Consent.md)
and the [changelog](https://github.com/Kapusch/PostHogApisForiOSComponents/blob/main/CHANGELOG.md).
