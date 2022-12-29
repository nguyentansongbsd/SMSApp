using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Plugin.Messaging;
using SMSApp.Helper;
using SMSApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SMSApp.Views
{
    public partial class DashboardPage : ContentPage
    {
        public ObservableCollection<OTPModel> Data { get; set; } = new ObservableCollection<OTPModel>();
        FirebaseClient firebaseClient = new FirebaseClient("https://smsappcrm-default-rtdb.asia-southeast1.firebasedatabase.app/",
            new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult("kLHIPuBhEIrL6s3J6NuHpQI13H7M0kHjBRLmGEPF") });
        public DashboardPage()
        {
            InitializeComponent();
            On<iOS>().SetUseSafeArea(true);
            this.BindingContext = this;
            Init();
        }

        public async void Init()
        {
            try
            {
                var status = await CheckPermission();
                if (status != PermissionStatus.Granted) return;

                LoadingHelper.Show();
                LoadDataOpt();
                //string temp = string.Empty;
                //var collection = firebaseClient
                //    .Child("SmsDb")
                //    .AsObservable<SMSModel>()
                //    .Subscribe(async (dbevent) =>
                //    {
                //        if (dbevent.EventType != Firebase.Database.Streaming.FirebaseEventType.Delete && dbevent.Object != null && temp != dbevent.Key)
                //        {
                //            temp = dbevent.Key;
                //            Data.Insert(0, dbevent.Object);
                //            if (dbevent.Object.isSend == false)
                //            {
                //                var smsMessenger = CrossMessaging.Current.SmsMessenger;
                //                if (smsMessenger.CanSendSmsInBackground)
                //                {
                //                    string numphone = string.Empty;
                //                    numphone = dbevent.Object.PhoneNumber.Contains("-") ? dbevent.Object.PhoneNumber.Trim().Split('-')[1] : dbevent.Object.PhoneNumber;

                //                    CrossMessaging.Current.SmsMessenger.SendSmsInBackground(numphone, dbevent.Object.Content);
                //                }
                //                await UpdateStatus(dbevent.Key, dbevent.Object);
                //            }
                //        }
                //        else if (dbevent.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                //        {
                //            var item = Data.SingleOrDefault(x => x == dbevent.Object);
                //            Data.Remove(item);
                //        }
                //    });

                var OTP = firebaseClient
                    .Child("PhuLongOTPDb")
                    .AsObservable<OTPModel>()
                    .Subscribe(async (dbevent) =>
                    {
                        if (dbevent.EventType != Firebase.Database.Streaming.FirebaseEventType.Delete && dbevent.Object != null)
                        {
                            if (dbevent.Object.IsSend == false && dbevent.Object.IsCanceled == false && dbevent.Object.IsConfirm == false && dbevent.Object.IsLimitTime == false)
                            {
                                this.Data.Insert(0, dbevent.Object);
                                var smsMessenger = CrossMessaging.Current.SmsMessenger;
                                if (smsMessenger.CanSendSmsInBackground)
                                {
                                    string numphone = string.Empty;
                                    numphone = dbevent.Object.Phone.Contains("-") ? dbevent.Object.Phone.Trim().Split('-')[1] : dbevent.Object.Phone;

                                    CrossMessaging.Current.SmsMessenger.SendSmsInBackground(numphone, dbevent.Object.Content);
                                }
                                await UpdateOTPDb(dbevent.Key, dbevent.Object);
                            }
                        }
                    });
                LoadingHelper.Hide();
            }
            catch(Exception ex)
            {

            }
            
        }

        private async void LoadDataOpt()
        {
            var data = (await firebaseClient
             .Child("PhuLongOTPDb")
             .OnceAsync<OTPModel>()).Select(item => new OTPModel
             {
                 Id = item.Object.Id,
                 Content = item.Object.Content,
                 Phone = item.Object.Phone,
                 OTPCode = item.Object.OTPCode,
                 IsSend = item.Object.IsSend,
                 IsLimitTime = item.Object.IsLimitTime,
                 IsConfirm = item.Object.IsConfirm,
                 IsCanceled = item.Object.IsCanceled,
                 Date = item.Object.Date,
             }).ToList();
            foreach (var item in data)
            {
                this.Data.Add(item);
            }
        }

        private async Task UpdateStatus(string key,SMSModel data)
        {
            await firebaseClient.Child("SmsDb").Child(key).PutAsync(new SMSModel() {Content = data.Content,date = data.date,CustomerName = data.CustomerName,PhoneNumber=data.PhoneNumber, isSend=true,type = data.type});
        }

        private async Task UpdateOTPDb(string key, OTPModel data)
        {
            await firebaseClient.Child("PhuLongOTPDb").Child(key).PutAsync(new OTPModel() { Content = data.Content, Id = data.Id, Phone = data.Phone, IsSend = true,IsLimitTime = data.IsLimitTime,IsConfirm = data.IsConfirm, IsCanceled = data.IsCanceled, OTPCode = data.OTPCode,Date=data.Date });
        }

        private async Task<PermissionStatus> CheckPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Sms>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Sms>();
            }
            return status;
        }
    }
}
