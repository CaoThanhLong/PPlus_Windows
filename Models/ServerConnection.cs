using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace PC_Client.Models
{
    public class ServerConnection
    {
        public WebSocket WebSocket;
        public string IP { get; set; }
        public string Port { get; set; }
        public ServerConnection(ServerInfo serverInfo) 
        {
            this.IP = serverInfo.Ip;
            this.Port = serverInfo.Port;
            this.CreateSocket();
            
        }
        public ServerConnection(string Ip, string Port)
        {    
            this.IP = Ip;
            this.Port = Port;
            this.CreateSocket();
        }

        private void CreateSocket()
        {
            string url = string.Format("ws://{0}:{1}", this.IP, this.Port);
            this.WebSocket = new WebSocket(url);
        }

        public void Start()
        {
            this.WebSocket.Connect();
        }

        public void Stop()
        {
            this.WebSocket.Close();
        }

        public void LoadInfo()
        {

        }

        public bool SendMessage(string message)
        {
            if (this.WebSocket.IsAlive)
            {
                this.WebSocket.Send(message);
                return true;
            }

            else return false;
        }
    }
}
