# Consent and opt-out

This page describes what the bundled PostHog iOS SDK (version in
`DependencyLocks/posthog-ios.json`, currently 3.92.5) does before and after
consent, as configured by this wrapper. It was checked against the upstream
source of that version; re-check it on every SDK bump.

## Options and defaults

| Option | Default | Effect |
| --- | --- | --- |
| `NativePostHogOptions.OptOut` | `false` | Start with capture disabled. |
| `NativePostHogOptions.PersistOptOut` | `true` | The SDK stores the state set by `OptIn()`/`OptOut()` and restores it at setup, where it wins over `OptOut`. |
| `NativePostHogOptions.CaptureLifecycle` | `true` | Application Installed/Updated/Opened/Backgrounded events. |

Runtime calls: `NativePostHog.OptIn()`, `NativePostHog.OptOut()` and
`NativePostHog.IsOptOut` (also `true` before `Configure`).

Both defaults are the upstream defaults, so `Configure(key, host)` behaves as
in 0.1.0: capture is enabled. Whether capture needs prior consent depends on the
app, its users and its legal basis; the wrapper does not decide it. Apps that
need consent first must ask for it explicitly with `OptOut = true`.

`PersistOptOut` maps to `PostHogConfig.persistOptOut`, which posthog-ios
exposes only as `@_spi(PostHogInternal)` (no upstream stability promise). It
is unchanged from 3.76.0 through 3.92.5, and only the Swift facade uses it.

## First launch, before any call except Configure

Common to both states:

- An anonymous ID (UUID v7) and a device ID seeded from it are created and
  stored under `Library/Application Support/<bundle id>`. They survive
  restarts. `Reset()` replaces the anonymous ID; the device ID is kept.
- The SDK requests the project's remote configuration
  (`GET <host>/array/<project key>/config`; PostHog Cloud uses its assets
  host). The request carries no user ID, but the server sees the device's IP
  address. Feature flags are not preloaded by this wrapper, so no `/flags`
  request is made at setup.
- Session replay, screen views, surveys, push and exception autocapture are
  turned off by the wrapper whatever the consent state.

With `OptOut = false` (default), the lifecycle integration starts: it records
the app version and build in `UserDefaults` (`PHGVersionKey`,
`PHGBuildKeyV2`), queues **Application Installed** (first launch) or
**Application Updated** (new build), then **Application Opened** on activation
and **Application Backgrounded**. Events go to a disk queue and are sent every
5 seconds, at 20 events, on `Flush()` and when the app enters the background.

With `OptOut = true`, no integration is installed: no lifecycle event is
queued and the app version is not recorded. `Capture` and `Identify` are
ignored; nothing is queued and the identity is not stored. Setup reads the
persisted state but does not write one.

## Start disabled, enable after consent

Keep the decision in the app's own consent store and pass it on every launch:

```csharp
NativePostHog.Configure("phc_example", "https://eu.i.posthog.com", new NativePostHogOptions
{
    OptOut = !consentStore.HasAnalyticsConsent,
    PersistOptOut = false, // the app's store is the only source of truth
});

// The user grants consent later:
consentStore.HasAnalyticsConsent = true;
NativePostHog.OptIn();
NativePostHog.Identify("account-example"); // Identify while opted out was dropped
```

`OptIn()` installs the integrations at that moment. Application Installed
(or Updated) is captured immediately, since the version check was skipped
while opted out; Application Opened follows at the next activation, not for
the activation that already happened.

With the default `PersistOptOut = true`, `OptIn()`/`OptOut()` also write
`posthog.optOut` in the SDK's storage, and that record wins over `OptOut` at
the next setup.

## Withdrawal

`OptOut()` disables capture and uninstalls the integrations. Later `Capture`
and `Identify` calls are ignored. Two limits remain upstream behavior:

- Events queued before `OptOut()` stay on disk and are still sent by the
  queue timer, on `Flush()` or at the next launch. There is no purge API.
- The anonymous ID and device ID stay on disk. Deleting data already received
  is done in the PostHog project, not in the SDK.

## Reset() and the persisted opt-out

Upstream `reset()` (3.92.5, `PostHogStorage.reset`) deletes the
`posthog.optOut` record along with identity and super properties. It does not
change the in-memory state. With `PersistOptOut = true`:

1. The user opts out, then logs out and the app calls `Reset()`.
2. Capture stays off for the rest of this process.
3. At the next launch no record exists, so `OptOut` from the options applies.
   With the default `false`, capture resumes without the user opting in.

Calling `OptOut()` again after `Reset()` does not restore the record: the SDK
is still opted out in memory and returns early without writing. Also,
`Reset()` requests feature flags for the new anonymous ID even while opted
out.

To avoid the issue, set `PersistOptOut = false` and pass the app's stored
decision as `OptOut` on every launch, as in the example above. The SDK then
never reads its own record, so `Reset()` cannot change the outcome.
