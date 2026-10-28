using System.Runtime.InteropServices;

namespace Kapusch.PostHog.iOS;

/// <summary>Thin calls into the official mobile SDK. Capture/flush do not prove server delivery.</summary>
public static class NativePostHog
{
    /// <summary>Sets up the SDK with capture enabled, unless an earlier launch persisted an opt-out.</summary>
    public static void Configure(
        string projectKey,
        string host,
        bool debug = false,
        bool captureLifecycle = true
    ) =>
        Configure(
            projectKey,
            host,
            new NativePostHogOptions { Debug = debug, CaptureLifecycle = captureLifecycle }
        );

    /// <summary>Sets up the SDK once per process. Later calls are ignored by the native SDK.</summary>
    public static void Configure(string projectKey, string host, NativePostHogOptions options)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        ArgumentNullException.ThrowIfNull(options);
        Check(
            Setup(
                projectKey,
                host,
                options.Debug ? 1 : 0,
                options.CaptureLifecycle ? 1 : 0,
                options.OptOut ? 1 : 0,
                options.PersistOptOut ? 1 : 0
            )
        );
    }

    /// <summary>
    /// Whether capture is currently disabled. Also <c>true</c> before <see cref="Configure(string, string, NativePostHogOptions)"/>.
    /// </summary>
    public static bool IsOptOut => IsOptOutNative() != 0;

    /// <summary>Queues an event on disk. Ignored while opted out. <paramref name="propertiesJson"/> is a JSON object.</summary>
    public static void Capture(string name, string propertiesJson = "{}")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Check(CaptureNative(name, propertiesJson));
    }

    /// <summary>Links the anonymous user to an account ID. Ignored, and not stored, while opted out.</summary>
    public static void Identify(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Check(IdentifyNative(id));
    }

    public static string DistinctId
    {
        get
        {
            var p = GetId();
            try
            {
                return Marshal.PtrToStringUTF8(p) ?? string.Empty;
            }
            finally
            {
                Free(p);
            }
        }
    }

    private static void Check(int status)
    {
        if (status != 0)
            throw new ArgumentException("Native PostHog rejected invalid input.");
    }

    [DllImport("__Internal", EntryPoint = "kph_setup_v2")]
    private static extern int Setup(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string key,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string host,
        int debug,
        int lifecycle,
        int optOut,
        int persistOptOut
    );

    [DllImport("__Internal", EntryPoint = "kph_is_opt_out")]
    private static extern int IsOptOutNative();

    [DllImport("__Internal", EntryPoint = "kph_capture")]
    private static extern int CaptureNative(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string name,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string json
    );

    [DllImport("__Internal", EntryPoint = "kph_identify")]
    private static extern int IdentifyNative([MarshalAs(UnmanagedType.LPUTF8Str)] string id);

    [DllImport("__Internal", EntryPoint = "kph_distinct_id")]
    private static extern IntPtr GetId();

    [DllImport("__Internal", EntryPoint = "kph_free")]
    private static extern void Free(IntPtr p);

    /// <summary>
    /// Clears identity and super properties and starts a new anonymous ID. The native SDK also deletes
    /// its persisted opt-out record: the running process stays opted out, but the next launch starts
    /// from <see cref="NativePostHogOptions.OptOut"/> again.
    /// While <see cref="IsOptOut"/> is true, call <c>Reset()</c> only when an identified user must be cleared
    /// from disk. Every call sends a <c>/flags</c> request with the device ID, the new anonymous ID, the time
    /// zone and default device properties, whatever <see cref="NativePostHogOptions.PersistOptOut"/> says.
    /// </summary>
    [DllImport("__Internal", EntryPoint = "kph_reset")]
    public static extern void Reset();

    [DllImport("__Internal", EntryPoint = "kph_flush")]
    public static extern void Flush();

    /// <summary>Enables capture and, when <see cref="NativePostHogOptions.PersistOptOut"/> is set, stores that choice.</summary>
    [DllImport("__Internal", EntryPoint = "kph_opt_in")]
    public static extern void OptIn();

    /// <summary>
    /// Disables capture and, when <see cref="NativePostHogOptions.PersistOptOut"/> is set, stores that choice.
    /// Events queued before this call stay on disk and are still sent.
    /// </summary>
    [DllImport("__Internal", EntryPoint = "kph_opt_out")]
    public static extern void OptOut();
}
