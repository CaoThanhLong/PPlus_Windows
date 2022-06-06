using Microsoft.MixedReality.WebRTC;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PC_Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PC_Client.Views
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        static string ip;
        static string port = "8886";
        private Process process = new Process();
        private ServerConnection connection;

        private Dictionary<string, Phone> history = new Dictionary<string, Phone>();
        private Dictionary<string, Phone> activePhones = new Dictionary<string, Phone>();
        private string currentMac;

        private DeviceAudioTrackSource microphoneSource;
        private DeviceVideoTrackSource webcamSource;
        private LocalAudioTrack localAudioTrack;
        private LocalVideoTrack localVideoTrack;

        private RemoteVideoTrack remoteVideoTrack;
        private RemoteAudioTrack remoteAudioTrack;

        private Transceiver videoTransceiver;
        private Transceiver audioTransceiver;

        private PeerConnection peerConnection;

        public Welcome()
        {
            InitializeComponent();
            StartServer();
            SetupConnection();
            ConnectToServer();
            CaptureLocalMedia();
            SetUpPeerConnection();
        }

        private void StartServer()
        {
            process.StartInfo.FileName = "cmd.exe";
            string server_path = Path.Combine(Environment.CurrentDirectory, "signaling_server.js");
            process.StartInfo.Arguments = string.Format(@"/C node {0}", server_path);
            process.Start();
        }

        private void SetupConnection()
        {
            ip = GetLocalIpAddress();

            ip_label.Text = ip;
            port_label.Text = port.ToString();

            connection = new ServerConnection(ip, port.ToString());
            connection.WebSocket.Connect();
            connection.WebSocket.OnMessage += WebSocket_OnMessage;
        }

        private string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return null;
        }

        private void ConnectToServer()
        {
            var pcName = Environment.MachineName;
            var os = Environment.OSVersion.ToString();

            string message = PCMessage.ConnnectToServer(ip, pcName, os);

            bool result = connection.SendMessage(message);

            if (!result)
            {
                Console.WriteLine("Sever died");
            }
        }
        private async Task CaptureLocalMedia()
        {
            // Video
            webcamSource = await DeviceVideoTrackSource.CreateAsync();
            var videoTrackConfig = new LocalVideoTrackInitConfig
            {
                trackName = "webcam_track"
            };
            localVideoTrack = LocalVideoTrack.CreateFromSource(webcamSource, videoTrackConfig);

            // Audio
            microphoneSource = await DeviceAudioTrackSource.CreateAsync();
            var audioTrackConfig = new LocalAudioTrackInitConfig
            {
                trackName = "microphone_track",

            };
            localAudioTrack = LocalAudioTrack.CreateFromSource(microphoneSource, audioTrackConfig);
        }

        private async Task SetUpPeerConnection()
        {
            peerConnection = new PeerConnection();
            var config = new PeerConnectionConfiguration
            {
                IceServers = new List<IceServer> {
                new IceServer{ Urls = { "stun:stun.l.google.com:19302" } }
                }
            };

            await peerConnection.InitializeAsync(config);

            SetUpTransceiver();

            peerConnection.VideoTrackAdded += (RemoteVideoTrack track) =>
            {
                remoteVideoTrack = track;
                //remoteVideoTrack.Argb32VideoFrameReady += RemoteVideoTrack_Argb32VideoFrameReady;
            };

            peerConnection.AudioTrackAdded += (RemoteAudioTrack track) =>
            {
                remoteAudioTrack = track;
                //remoteAudioTrack.AudioFrameReady += RemoteAudioTrack_AudioFrameReady;
            };

            peerConnection.LocalSdpReadytoSend += PeerConnection_LocalSdpReadytoSend;
            peerConnection.IceCandidateReadytoSend += PeerConnection_IceCandidateReadytoSend;
        }

        private void SetUpTransceiver()
        {
            videoTransceiver = peerConnection.AddTransceiver(MediaKind.Video);
            audioTransceiver = peerConnection.AddTransceiver(MediaKind.Audio);

            videoTransceiver.DesiredDirection = Transceiver.Direction.SendReceive;
            audioTransceiver.DesiredDirection = Transceiver.Direction.SendReceive;

            videoTransceiver.LocalVideoTrack = localVideoTrack;
            audioTransceiver.LocalAudioTrack = localAudioTrack;
        }

        private async void WebSocket_OnMessage(object? sender, WebSocketSharp.MessageEventArgs e)
        {
            var converter = new ExpandoObjectConverter();
            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(e.Data, converter);

            switch (data.type)
            {
                case "server_android_connect":
                    Console.WriteLine("Android send connection request");
                    AcceptPhone(data.mac);
                    break;
                case "server_offer":

                    var sdpMessage = new SdpMessage()
                    {
                        Type = SdpMessageType.Offer,
                        Content = data.offer.sdp
                    };
                    await peerConnection.SetRemoteDescriptionAsync(sdpMessage);
                    peerConnection.CreateAnswer();

                    break;
                case "server_candidate":
                    var iceCandidate = new IceCandidate()
                    {
                        SdpMid = data.sdpMid,
                        SdpMlineIndex = (int)data.sdpMLineIndex,
                        Content = data.candidate
                    };

                    peerConnection.AddIceCandidate(iceCandidate);
                    Console.WriteLine("PC add ice candidate");

                    break;
                case "server_android_disconnect":
                    DeleteWebRTCConnection();
                    break;
                case "server_android_close":
                    DeleteWebRTCConnection();
                    break;
                default:
                    break;
            }
        }

        private void ConnectionConfirm(string mac, string name)
        {
            Alert alert = new Alert(mac, name);
            alert.ShowDialog();

            if (alert.Confirm)
            {
                AcceptPhone(mac);
                this.Visibility = Visibility.Hidden;
                Thread.Sleep(100);
                OpenConfig(name);

            }
            else
            {
                Reject(mac);
            }
        }

        private void AcceptPhone(string mac)
        {
            var pcName = Environment.MachineName;
            var os = Environment.OSVersion.ToString();
            string linkedDate = DateTime.Today.ToString();
            string message = PCMessage.Accept(mac, linkedDate);
            currentMac = mac;
            connection.SendMessage(message);
        }

        private void OpenConfig(string name)
        {
            Configuration config = new Configuration(name);
            config.ShowDialog();
            if (config.IsDisconnect)
            {
                this.Visibility = Visibility.Visible;
            }
            else
            {
                this.Close();
            }
        }

        private void Reject(string mac)
        {
            var pcName = Environment.MachineName;
            var os = Environment.OSVersion.ToString();
            string message = PCMessage.Reject(mac);
        }

        private void DeleteWebRTCConnection()
        {
            peerConnection.Close();
            peerConnection.Dispose();
            peerConnection = null;
        }

        private void PeerConnection_IceCandidateReadytoSend(IceCandidate candidate)
        {
            Console.WriteLine("PC send candidate");
            string iceCandidate = PCMessage.Candidate(currentMac, candidate);

            bool result = connection.SendMessage(iceCandidate);

            if (!result)
            {
                Console.WriteLine("Sever died");
            }
        }

        private void PeerConnection_LocalSdpReadytoSend(SdpMessage message)
        {
            Console.WriteLine("PC send answer");
            message.Type = SdpMessageType.Answer;

            dynamic sdpAnswer = new ExpandoObject();

            sdpAnswer.type = SdpMessageType.Answer.ToString().ToLower();
            sdpAnswer.sdp = message.Content;

            string answer = PCMessage.Answer(currentMac, sdpAnswer);
            bool result = connection.SendMessage(answer);

            if (!result)
            {
                Console.WriteLine("Sever died");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (peerConnection != null)
            {
                DeleteWebRTCConnection();
            }

            if (process != null)
            {
                process.Kill();

                foreach (var node in Process.GetProcessesByName("node"))
                {
                    node.Kill();
                }
            }

            if (localVideoTrack != null)
            {
                localVideoTrack.Dispose();
                webcamSource.Dispose();
            }

            if (localAudioTrack != null)
            {
                localAudioTrack.Dispose();
                microphoneSource.Dispose();
            }            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //string mac = "0C:00:91:88:C9:E3";
            //string name = "SAMSUNG 1";

            //Alert alert = new Alert(mac, name);
            //alert.ShowDialog();

            //if (alert.Confirm)
            //{
            //    AcceptPhone(mac);
            //    this.Hide();
            //    Thread.Sleep(100);

            //    OpenConfig(name);
                
            //}
            //else
            //{
            //    Reject(mac);
            //}
        }
    }
}
