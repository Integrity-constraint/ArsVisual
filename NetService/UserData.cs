using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVisual.NetService
{
    public class UserData
    {
        private static UserData _instance;
        private static readonly object _lock = new object();

        public string Password { get; private set; }
        public string Email { get; private set; }
        public bool IsInitialized => !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password);

        private UserData(string pass, string mail)
        {
            Password = pass;
            Email = mail;
        }

        public static UserData GetInstance(string pass = null, string mail = null)
        {
            lock (_lock)
            {
                if (pass != null && mail != null)
                {
                   
                    _instance = new UserData(pass, mail);
                }
                return _instance;
            }
        }

        public static void Reset()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }

    }
}
