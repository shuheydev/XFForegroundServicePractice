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
            await Task.Run(async () =>
            {
                for (long i = 0; i < long.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();

                    await Task.Delay(1000);
                    var message = new TickedMessage
                    {
                        Message = i.ToString()
                    };

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send<TickedMessage>(message, nameof(TickedMessage));
                    });
                }
            }, token);
        }
    }
}
