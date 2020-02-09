using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace DBD_Swim_Tracker_0._2.Controllers
{
    internal class IndexViewModels
    {
        public bool HasPassword { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public bool BrowserRemembered { get; set; }
    }
}