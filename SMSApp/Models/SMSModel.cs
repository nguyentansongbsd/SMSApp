using System;
using Newtonsoft.Json;

namespace SMSApp.Models
{
    public class SMSModel
    {
        [JsonIgnore]
        public string Avatar { get=> $"https://ui-avatars.com/api/?background=2196F3&rounded=false&bold=true&color=ffffff&uppercase=true&size=100&length=2&name={CustomerName}";  }

        public DateTime date { get; set; }
        public string PhoneNumber { get; set; }
        public string Content { get; set; }
        public bool isSend { get; set; }
        public bool isSendSuccess { get; set; }
        public string CustomerName { get; set; }
        public string type { get; set; }
    }
}
