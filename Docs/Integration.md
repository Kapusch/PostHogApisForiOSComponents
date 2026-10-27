# Integration

Reference `Kapusch.PostHog.iOS` 1.0.0-rc.1 from a .NET 10 iOS application.
Initialize once, on the platform UI thread during startup, with a public project
key and host. Use separate test and production projects. No secret API keys in
apps.

Capture is enabled by default, as upstream. If the app needs consent first,
start opted out and opt in after consent; see [consent and opt-out](Consent.md)
for what the SDK stores and sends before and after, and for the `Reset()` caveat.

Use native anonymous identity until sign-in; identify with a stable account ID.
Call Reset on logout and before direct account A to B transitions. Never identify
one account as another. Persist business event IDs to deduplicate downstream.
Flush requests sending; it does not await or prove ingestion. Auto screen
capture and replay are disabled; lifecycle capture is configurable. SDK debug
logs may contain event properties; disable in production and redact before
sharing.

## API example

```csharp
using Kapusch.PostHog.iOS;

NativePostHog.Configure("phc_example", "https://eu.i.posthog.com", new NativePostHogOptions
{
    OptOut = !userHasConsented, // omit to keep the upstream default: capture enabled
    PersistOptOut = false,      // the app stores consent and passes it on every launch
});
NativePostHog.OptIn();          // after consent; NativePostHog.OptOut() withdraws it
NativePostHog.Capture("item_completed", "{\"item_id\":\"example\"}");
NativePostHog.Identify("account-example");
NativePostHog.Reset();
```

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
