using System.Runtime.InteropServices;

namespace Kapusch.PostHog.iOS;

/// <summary>Thin calls into the official mobile SDK. Capture/flush do not prove server delivery.</summary>
public static class NativePostHog
{
    public static void Configure(
        string projectKey,
        string host,
        bool debug = false,
        bool captureLifecycle = true
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        Check(Setup(projectKey, host, debug ? 1 : 0, captureLifecycle ? 1 : 0));
    }

    public static void Capture(string name, string propertiesJson = "{}")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Check(CaptureNative(name, propertiesJson));
    }

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

    [DllImport("__Internal", EntryPoint = "kph_setup")]
    private static extern int Setup(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string key,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string host,
        int debug,
        int lifecycle
    );

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

    [DllImport("__Internal", EntryPoint = "kph_reset")]
    public static extern void Reset();

    [DllImport("__Internal", EntryPoint = "kph_flush")]
    public static extern void Flush();

    [DllImport("__Internal", EntryPoint = "kph_opt_in")]
    public static extern void OptIn();

    [DllImport("__Internal", EntryPoint = "kph_opt_out")]
    public static extern void OptOut();
}
