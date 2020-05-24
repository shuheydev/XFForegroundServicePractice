using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XFForegroundServicePractice.Messages;

namespace XFForegroundServicePractice.Tasks
{
    public class TaskCounter
    {
        public async Task RunCounter(CancellationToken token)
        {
            //GPSの精度をHighに
            var request = new GeolocationRequest(GeolocationAccuracy.High);

            await Task.Run(async () =>
            {
                for (long i = 0; i < long.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();

                    await Task.Delay(1000);

                    //ここから
                    var location = await Geolocation.GetLocationAsync(request);

                    var message = new TickedMessage
                    {
                        Message = $"Count : {i.ToString()}, Lat = {location.Latitude}, Lon = {location.Longitude}"
                    };
                    //ここまで

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send<TickedMessage>(message, nameof(TickedMessage));
                    });
                }
            }, token);
        }
    }
}
