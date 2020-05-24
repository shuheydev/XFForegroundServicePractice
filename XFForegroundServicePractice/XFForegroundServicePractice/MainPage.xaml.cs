using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XFForegroundServicePractice.Messages;

namespace XFForegroundServicePractice
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();

            this.BindingContext = this;

            MessagingCenter.Subscribe<TickedMessage>(this, nameof(TickedMessage), message =>
            {
                Message = message.Message;
            });
        }

        private void Button_LongRunningTaskStart_Clicked(object sender, EventArgs e)
        {
            var message = new StartLongRunningTaskMessage();
            MessagingCenter.Send(message, nameof(StartLongRunningTaskMessage));
        }

        private void Button_LongRunningTaskStop_Clicked(object sender, EventArgs e)
        {
            var message = new StopLongRunningTaskMessage();
            MessagingCenter.Send(message, nameof(StopLongRunningTaskMessage));
        }
    }
}
