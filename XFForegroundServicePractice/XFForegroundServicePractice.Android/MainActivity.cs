using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using XFForegroundServicePractice.Messages;
using Android.Content;
using XFForegroundServicePractice.Droid.Services;

namespace XFForegroundServicePractice.Droid
{
    [Activity(Label = "XFForegroundServicePractice", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            //Forms側からのバックグラウンドタスク開始,停止のメッセージ購読
            MessagingCenter.Subscribe<StartLongRunningTaskMessage>(this, nameof(StartLongRunningTaskMessage),_=> {
                var intent = new Intent(this, typeof(LongRunningTaskService));
                StartService(intent);
            });
            MessagingCenter.Subscribe<StopLongRunningTaskMessage>(this, nameof(StopLongRunningTaskMessage), _ => {
                var intent = new Intent(this, typeof(LongRunningTaskService));
                StopService(intent);
            });
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}