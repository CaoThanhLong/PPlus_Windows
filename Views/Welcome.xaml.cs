using Microsoft.MixedReality.WebRTC;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PC_Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Windows;

namespace PC_Client.Views
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        Process process = new Process();
        private ServerConnection connection;

        private Dictionary<string, Phone> history = new Dictionary<string, Phone>();
        private Dictionary<string, Phone> activePhones = new Dictionary<string, Phone>();
        private string currentMac;

        RemoteVideoTrack remoteVideoTrack;
        RemoteAudioTrack remoteAudioTrack;
        private List<PeerConnection> peerConnections = new List<PeerConnection>();

        public Welcome()
        {
            InitializeComponent();
            //StartSignalingServer();
            //SetupServerConnection("localhost", "8886"); // temp
            //ConnectToServer();
            //LoadHistory();
        }

        private void StartSignalingServer()
        {
            process.StartInfo.FileName = "cmd.exe";
            string server_path = Path.Combine(Environment.CurrentDirectory, "signaling_server.js");
            process.StartInfo.Arguments = string.Format(@"/C node {0}", server_path);
            process.Start();
        }

        private void SetupServerConnection(string ip, string port)
        {
            connection = new ServerConnection(ip, port);
            connection.WebSocket.Connect();
            connection.WebSocket.OnOpen += WebSocket_OnOpen;
            connection.WebSocket.OnMessage += WebSocket_OnMessage;
            connection.WebSocket.OnError += WebSocket_OnError;
            connection.WebSocket.OnClose += WebSocket_OnClose;
        }

        private void ConnectToServer()
        {

            var pcName = Environment.MachineName;
            var os = Environment.OSVersion.ToString();

            string message = PCMessage.ConnnectToServer("ip1", pcName, os);
            bool result = connection.SendMessage(message);

            if (!result)
            {
                MessageBox.Show("Server died");
            }
        }

        private void SaveHistory()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "history.json");

            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, history);
            };
        }

        private void LoadHistory()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "history.json");

            if (File.Exists(path))
            {
                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    history = (Dictionary<string, Phone>)serializer.Deserialize(file, typeof(Dictionary<string, Phone>));
                }
            }
        }


        private void WebSocket_OnOpen(object? sender, EventArgs e) { }

        private void WebSocket_OnMessage(object? sender, WebSocketSharp.MessageEventArgs e)
        {
            //var converter = new ExpandoObjectConverter();
            //dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(e.Data, converter);
            //bool isAccept = true;

            //switch (data.type)
            //{
            //    case "server_connect":
            //        break;
            //    case "server_android_connect":
            //        ConfirmConnection(data, isAccept);
            //        break;
            //    case "server_offer":
            //        peerConnection.SetRemoteDescriptionAsync(data.offer);
            //        peerConnection.CreateAnswer();
            //        break;
            //    case "server_candidate":
            //        peerConnection.AddIceCandidate(data.candidate);
            //        break;
            //    case "server_android_disconnect":
            //        RemoveActivePhone(data);
            //        break;
            //    case "server_android_close":
            //        RemoveActivePhone(data);
            //        break;
            //    default:
            //        break;
            //}
        }

        private void ConfirmConnection(dynamic data, bool accept)
        {
            if (history.ContainsKey(data.mac))
            {
                MessageBox.Show("Ok");
                UpdateActivePhones(data);
                AcceptPhone(false);
            }
            else
            {
                if (accept)
                {
                    MessageBox.Show("Accept");
                    UpdateActivePhones(data);
                    UpdateHistory(data);
                    AcceptPhone(true);
                }
                else
                {
                    MessageBox.Show("Reject");
                }
            }
        }

        private void RemoveActivePhone(dynamic data)
        {
            if (!activePhones.ContainsKey(data.mac))
            {
                activePhones.Remove(data.mac);
            }
        }

        private void UpdateActivePhones(dynamic data)
        {
           
        }

        private void UpdateHistory(dynamic data)
        {
           
        }

        private void AcceptPhone(bool islinkedDate)
        {
            var pcName = Environment.MachineName;
            var os = Environment.OSVersion.ToString();

            string linkedDate = "";

            if (islinkedDate)
            {
                linkedDate = DateTime.Today.ToString();
            }

            string message = PCMessage.Accept(currentMac, linkedDate);
            bool result = connection.SendMessage(message);

            if (!result)
            {
                MessageBox.Show("Server died");
            }
        }

        private void DeleteWebRTCConnection()
        {
            
        }

        private void WebSocket_OnError(object? sender, WebSocketSharp.ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void WebSocket_OnClose(object? sender, WebSocketSharp.CloseEventArgs e)
        {
            MessageBox.Show("Server die");
        }
    }
}
