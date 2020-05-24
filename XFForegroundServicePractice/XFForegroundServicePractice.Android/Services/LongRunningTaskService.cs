using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using XFForegroundServicePractice.Messages;
using XFForegroundServicePractice.Tasks;
using static Android.Bluetooth.BluetoothClass;

namespace XFForegroundServicePractice.Droid.Services
{
    //https://docs.microsoft.com/ja-jp/xamarin/android/app-fundamentals/services/creating-a-service/started-services
    [Service]
    public class LongRunningTaskService : Android.App.Service
    {
        CancellationTokenSource _cts;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    //INVOKE THE SHARED CODE
                    var counter = new TaskCounter();
                    counter.RunCounter(_cts.Token).Wait();
                }
                catch (System.OperationCanceledException)
                {
                }
                finally
                {
                    if (_cts.IsCancellationRequested)
                    {
                        var message = new CancelledMessage();
                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(
                            () => MessagingCenter.Send(message, "CancelledMessage")
                        );
                    }
                }

            }, _cts.Token);


            //Notificationの作成(Android 8.0以降では必須)
            var notification = CreateNotification();

            //Foreground Serviceとして実行
            //id:サービスを識別するためにアプリケーション内で一意である整数値。
            //notification:サービスが実行されている間、Android がステータスバーに表示する Notification オブジェクト。
            //Permissionがないと,ここで例外
            StartForeground(id: 1, notification: notification);


            //OnstartCommandメソッドの戻り値について
            //http://www.gigas-jp.com/appnews/archives/6228
            /*
  ●START_NOT_STICKY
　　Serviceがkillされた場合、Serviceは再起動しません。

　●START_STICKY
　　デフォルトではこの設定になります。
　　Serviceがkillされた場合、直ちにServiceの再起動を行います。
　　再起動時、前回起動時のIntentは再配信されず、
　　複数のServiceが起動していても再起動するServiceは1つです。

　●START_REDELIVER_INTENT
　　Serviceがkillされた場合、直ちにServiceの再起動を行います。
　　再起動時、最後に使用したIntentを使用します。
　　また、複数のServiceが起動している場合、すべて再起動します。

　●START_STICKY_COMPATIBLILITY
　　START_STICKYの互換バージョンです。Android2.0未満ではこれがデフォルト設定です。
             */
            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Token.ThrowIfCancellationRequested();

                _cts.Cancel();
            }
            base.OnDestroy();
        }

        //フォアグラウンドサービス用のNotification ChannelとNotificationを作成する
        private Notification CreateNotification()
        {
            #region Create Channel
            string channelId = "ForegroundPracticeChannel";
            string channelName = "ForegroundPracticeChannel";
            string channelDescription = "The foreground practice channel for notifications";
            int _pendingIntentId = 1;

            NotificationManager _notificationManager;
            _notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Android.App.Application.NotificationService);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Low)
                {
                    Description = channelDescription,
                };
                channel.EnableVibration(false);
                _notificationManager.CreateNotificationChannel(channel);
            }
            #endregion

            #region Create Notification
            Intent foregroundNotificationIntent = new Intent(Android.App.Application.Context, typeof(MainActivity));

            PendingIntent pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, _pendingIntentId, foregroundNotificationIntent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("Foreground Practice!")
                .SetContentText("Foreground service started")
                .SetOngoing(true)
                .SetColor(ActivityCompat.GetColor(Android.App.Application.Context, Resource.Color.colorAccent))
                .SetLargeIcon(BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.launcher_foreground))
                .SetSmallIcon(Resource.Drawable.launcher_foreground);

            var notification = builder.Build();
            #endregion

            return notification;
        }
    }
}