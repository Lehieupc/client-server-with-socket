using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mylibrary
{
    public class User
    {
        public string account { set; get; }
        public string password { set; get; }
        public User(string account, string password)
        {
            this.account = account;
            this.password = password;
        }
    }
}
