using System;
using SMSApp.IServices;
using Xamarin.Forms;

namespace SMSApp.Helper
{
    public class ToastMessageHelper
    {
        public static void ShortMessage(string message)
        {
            DependencyService.Get<IToastMessage>().ShortAlert(message);
        }

        public static void LongMessage(string message)
        {
            DependencyService.Get<IToastMessage>().LongAlert(message);
        }
    }
}
