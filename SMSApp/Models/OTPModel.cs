using System;
namespace SMSApp.Models
{
    public class OTPModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Phone { get; set; }
        public string OTPCode { get; set; }
        public bool IsSend { get; set; }
        public bool IsLimitTime { get; set; }
        public bool IsConfirm { get; set; }
        public bool IsCanceled { get; set; }
        public DateTime Date { get; set; }
    }
}
