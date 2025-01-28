using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class LoginCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public LoginCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public string toString()
        {
            return Username + " " + Password;
        }
    }
}
