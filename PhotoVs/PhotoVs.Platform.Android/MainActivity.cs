using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using PhotoVs.Logic;

namespace PhotoVs.Platform.Android
{
    [Activity(Label = "PhotoVs.Platform.Android"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.FullUser
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout)]
    public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            foreach (var s in Assets.List("shaders"))
            {
                System.Diagnostics.Debug.Print(s);
            }

            base.OnCreate(bundle);
            var g = new MainGame(new AndroidPlatform(Assets));
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }
    }
}

