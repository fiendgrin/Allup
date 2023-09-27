﻿using System.ComponentModel.DataAnnotations;

namespace Allup.ViewModels
{
    public class SmtpSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
