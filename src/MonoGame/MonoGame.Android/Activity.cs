using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using MonoGame.Shared;

namespace MonoGameAndroid
{
    [Activity(Label = "MonoGameAndroid"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.FullUser
        , ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout)]
    public class Activity : Microsoft.Xna.Framework.AndroidGameActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var g = new MainGame();
            g.Deactivated +=
                (sender, e) =>
                {
                    Process.KillProcess(Process.MyPid());
                }; 
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }
    }
}

