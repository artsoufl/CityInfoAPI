using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Services
{
    public class LocalMailService : IMailService
    {
        private readonly IConfiguration _config;

        public LocalMailService(IConfiguration config)
        {
            _config = config;
        }

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail from {_config["mailSettings:mailFromAddress"]} to {_config["mailSettings:mailToAddress"]} from LocalMailService");
            Debug.WriteLine($"Subject {subject}");
            Debug.WriteLine($"Message {message}");
        }
    }
}
