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
        NativePostHog.Configure("phc_sample", "https://127.0.0.1:1", debug: true);
        NativePostHog.Capture("native_smoke", "{\"source\":\"sample\"}");
        Console.WriteLine("POSTHOG_NATIVE_SMOKE_OK " + NativePostHog.DistinctId);
        NativePostHog.Flush();
        return true;
    }
}
