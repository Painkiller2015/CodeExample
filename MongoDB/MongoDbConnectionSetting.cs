using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB_Service.MongoDB
{
    internal class MongoDbConnectionSetting
    {
        public string Username { get; init; }
        public string Password { get; init; }
        public required string Hostname { get; init; }
        public required int Port { get; init; }
        public string Database { get; init; }
        public string AuthenticationSource { get; init; }
    }
}
