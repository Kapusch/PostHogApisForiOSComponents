using Foundation;
using Kapusch.PostHog.iOS;
using UIKit;

UIApplication.Main(args, null, typeof(AppDelegate));

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    public override UIWindow? Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary options)
    {
        Window = new UIWindow(UIScreen.MainScreen.Bounds)
        {
            RootViewController = new UIViewController(),
        };
        Window.MakeKeyAndVisible();
        // Loopback is deliberate: this sample tests native startup and the disk queue without sending data.
        // Start opted out, as a consent-gated app would. PersistOptOut = false keeps every launch
        // deterministic: the sample's own state, not a value stored by a previous run, decides.
        NativePostHog.Configure(
            "phc_sample",
            "https://127.0.0.1:1",
            new NativePostHogOptions { Debug = true, OptOut = true, PersistOptOut = false }
        );
        var startedOptedOut = NativePostHog.IsOptOut;
        NativePostHog.Capture("native_smoke_before_consent", "{\"source\":\"sample\"}"); // dropped
        NativePostHog.OptIn();
        NativePostHog.Capture("native_smoke", "{\"source\":\"sample\"}");
        Console.WriteLine(
            $"POSTHOG_NATIVE_SMOKE_OK {NativePostHog.DistinctId} started_opted_out={startedOptedOut} opted_out={NativePostHog.IsOptOut}"
        );
        NativePostHog.Flush();
        return true;
    }
}
