using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = "atrsoufl@gmail.com";
        private string _mailFrom = "atrsoufl@gmail.com";

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo} from CloudMailService");
            Debug.WriteLine($"Subject {subject}");
            Debug.WriteLine($"Message {message}");
        }
    }
}
