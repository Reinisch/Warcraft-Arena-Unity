using System;
using System.Collections.Generic;
using System.Text;
using ElleFramework.Database.MVC;

namespace ElleRealTime.Shared.DBEntities.Accounts
{
    public class Account : View
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
