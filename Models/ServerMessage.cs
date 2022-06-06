using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Client.Models
{
    public class ServerMessage
    {
        public enum Type
        {
            server_connect,
            server_android_connect,
            server_offer,
            server_candidate,
            server_android_disconnect,
            server_android_close,

        }
    }
}
