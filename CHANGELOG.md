# Changelog

## 1.0.0-rc.1 (unreleased)

- Bundled PostHog iOS SDK updated from 3.90.2 to 3.92.5. Since 3.92.0, a
  property passed to `Capture` overrides a super property or SDK context
  property of the same name (`$app_version`, `$lib`, ...).
- New `NativePostHogOptions` (`Debug`, `CaptureLifecycle`, `OptOut`,
  `PersistOptOut`), a `Configure(key, host, options)` overload and
  `NativePostHog.IsOptOut`. Defaults match upstream: capture stays enabled.
- Native ABI: `kph_setup` is replaced by `kph_setup_v2`, and `kph_is_opt_out`
  is added. Managed and native code ship together, so packaged consumers need
  no change; source-mode builds must rebuild the native slices.
- Documented `Reset()` behavior: it deletes the persisted opt-out record and,
  even while opted out, sends a `/flags` request with the device ID. See
  [consent](Docs/Consent.md) and the 0.1.0 migration steps in
  [integration](Docs/Integration.md#upgrading-from-010).

## 0.1.0

- Qualification build: native PostHog iOS 3.90.2 interop with setup, capture,
  identify, reset, flush, distinct ID and opt-in/out.
