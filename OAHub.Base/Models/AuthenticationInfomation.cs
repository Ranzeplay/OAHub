using System;
using System.Collections.Generic;
using System.Text;

namespace OAHub.Base.Models
{
    public class AuthenticationInfomation
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string PassportServerAddress { get; set; }

        public string PassportServerAuthorizePath { get; set; }

        public string PassportServerGetTokenPath { get; set; }

        public string PassportServerRequestProfilePath { get; set; }

        public string AppServerAddress { get; set; }

        public string AppServerCallbackPath { get; set; }
    }
}
