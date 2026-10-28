namespace Kapusch.PostHog.iOS;

/// <summary>Setup options for <see cref="NativePostHog"/>. Defaults match the upstream SDK.</summary>
public sealed class NativePostHogOptions
{
    /// <summary>Verbose native SDK logs. They may contain event properties; keep off in production.</summary>
    public bool Debug { get; init; }

    /// <summary>Capture Application Installed/Updated/Opened/Backgrounded. Default <c>true</c>.</summary>
    public bool CaptureLifecycle { get; init; } = true;

    /// <summary>
    /// Start with capture disabled. Default <c>false</c> (upstream default: capture enabled).
    /// When <see cref="PersistOptOut"/> is <c>true</c>, a state stored by an earlier
    /// <see cref="NativePostHog.OptIn"/> or <see cref="NativePostHog.OptOut"/> wins over this value.
    /// </summary>
    public bool OptOut { get; init; }

    /// <summary>
    /// Let the native SDK store the opt-in/out state on disk and restore it at setup. Default <c>true</c>
    /// (upstream default). Set <c>false</c> when the app keeps its own consent record and passes it as
    /// <see cref="OptOut"/> on every launch; <see cref="NativePostHog.Reset"/> then cannot change it.
    /// </summary>
    public bool PersistOptOut { get; init; } = true;
}
