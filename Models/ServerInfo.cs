using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Client.Models
{
    public class ServerInfo
    {
        public ServerInfo(string path)
        {
            this.Load(path);
        }

        public string Ip { get; set; }

        public string Port { get; set; }

        private void Load(string path)
        {
            // Modify
            this.Ip = "";
            this.Port = "";
        }

        public bool Save(string path)
        {
            return false;
        }
    }
}
