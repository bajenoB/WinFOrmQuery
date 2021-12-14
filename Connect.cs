using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp23
{
    
        public class Connect
        {
            public string Server { get; set; }
            public string Database { get; set; }
            public string Password { get; set; }
            public string Username { get; set; }
            public Connect()
            {

            }
            public Connect(string server, string database, string password, string username)
            {
                Server = server;
                Database = database;
                Password = password;
                Username = username;
            }
        }
    
}
