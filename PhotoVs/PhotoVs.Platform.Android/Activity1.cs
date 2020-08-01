using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;
using PhotoVs.Logic;
using Debug = System.Diagnostics.Debug;

namespace PhotoVs.Platform.Android
{
    [Activity(Label = "PhotoVs.Platform.Android"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen"
        , AlwaysRetainTaskState = true
        , LaunchMode = LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.Landscape
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout)]
    public class Activity1 : AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            debugAsset(Assets, "");

            base.OnCreate(bundle);
            var g = new MainGame(new AndroidPlatform(Assets));
            SetContentView((View) g.Services.GetService(typeof(View)));
            g.Run();
        }

        private void debugAsset(AssetManager asset, string folder)
        {
            var a = asset.List(folder);

            foreach (var s in a)
            {
                Debug.Print("ASSET IS HERE: " + s);
                debugAsset(asset, s);
            }
        }
    }
}