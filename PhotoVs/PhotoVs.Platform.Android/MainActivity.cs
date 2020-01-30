using Android.App;
using Android.Content.PM;
using Android.Content.Res;
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
            debugAsset(Assets, "");

            base.OnCreate(bundle);
            var g = new MainGame(new AndroidPlatform(Assets));
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }

        private void debugAsset(AssetManager asset, string folder)
        {
            var a = asset.List(folder);

            foreach (var s in a)
            {
                System.Diagnostics.Debug.Print("ASSET IS HERE: " + s);
                debugAsset(asset, s);
            }
        }
    }
}

