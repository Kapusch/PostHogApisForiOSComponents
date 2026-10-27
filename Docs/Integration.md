# Integration

Reference `Kapusch.PostHog.iOS` 1.0.0-rc.1 from a .NET 10 iOS application.
Initialize once, on the platform UI thread during startup, with a public project
key and host. Use separate test and production projects. No secret API keys in
apps.

Capture is enabled by default, as upstream. If the app needs consent first,
start opted out and opt in after consent; see [consent and opt-out](Consent.md)
for what the SDK stores and sends before and after, and for the `Reset()` caveats.

Use native anonymous identity until sign-in; identify with a stable account ID.
Call Reset on logout and before direct account A to B transitions. While
`IsOptOut` is true, call `Reset()` only when an identified user must be cleared
from disk. Every call sends a `/flags` request with the device ID (see
[consent](Consent.md#reset-sends-a-feature-flag-request-even-while-opted-out)).
Never identify one account as another. Persist business event IDs to deduplicate downstream.
Flush requests sending; it does not await or prove ingestion. Auto screen
capture and replay are disabled; lifecycle capture is configurable. SDK debug
logs may contain event properties; disable in production and redact before
sharing.

## API example

The calls below belong to different moments of the app, not to one sequence.

```csharp
using Kapusch.PostHog.iOS;

// Startup, once per process, before any other call:
NativePostHog.Configure("phc_example", "https://eu.i.posthog.com", new NativePostHogOptions
{
    OptOut = !consentStore.HasAnalyticsConsent, // omit to keep the upstream default: capture enabled
    PersistOptOut = false,                      // the app stores consent and passes it on every launch
});

// When the user grants consent (for example from a consent prompt callback):
consentStore.HasAnalyticsConsent = true;
NativePostHog.OptIn();

// When the user withdraws consent:
consentStore.HasAnalyticsConsent = false;
NativePostHog.OptOut();

// Anywhere after Configure; ignored while opted out:
NativePostHog.Capture("item_completed", "{\"item_id\":\"example\"}");

// On sign-in; ignored, and not stored, while opted out:
NativePostHog.Identify("account-example");

// On logout or before switching accounts (see the opted-out note above):
NativePostHog.Reset();
```

`OptIn()`, `OptOut()`, `Capture`, `Identify` and `Reset()` do nothing before
`Configure`: the upstream SDK returns early when it is not set up. A consent
decision taken before `Configure` must be passed as `OptOut` instead.

`Configure(key, host, debug, captureLifecycle)` remains available and uses the
default options. `NativePostHog.IsOptOut` reports the current state.

Properties are a JSON object. Omit missing properties rather than sending JSON
nulls. Since SDK 3.92.0, a property passed to `Capture` overrides a registered
super property or SDK context property of the same name (`$app_version`,
`$os_name`, `$lib`, ...), so do not pass `$`-prefixed SDK keys. Event capture
time is the SDK time; keep a business completion time in a separate UTC
property for events recorded later. SDK lifetime is process-wide: configure
once before using this singleton API.

## Upgrading from 0.1.0

The native entry point is now `kph_setup_v2`; managed and native code ship in
the same package, so no consumer change is needed. Existing calls keep their
behavior. `OptIn`, `OptOut` and `IsOptOut` defer to the upstream SDK.
