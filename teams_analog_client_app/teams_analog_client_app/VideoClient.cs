using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace teams_analog_client_app
{
    public class VideoClient
    {
        private PictureBox _playerPictureBox;
        private UdpClient _udpClient;
        private const int PORT = 11000;
        private const string MULTICAST_GROUP = "239.0.0.1";
        private const int MAX_PACKET_SIZE = 65000;
        private Thread _receiveThread;
        private List<byte> _imageBuffer = new List<byte>();

        public VideoClient(PictureBox playerPictureBox)
        {
            _playerPictureBox = playerPictureBox;

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(new IPEndPoint(IPAddress.Any, PORT));

            _udpClient = new UdpClient();
            _udpClient.Client = socket;
            _udpClient.JoinMulticastGroup(IPAddress.Parse(MULTICAST_GROUP));

            _receiveThread = new Thread(ReceiveVideo);
            _receiveThread.IsBackground = true;
            _receiveThread.Start();
        }

        private void ReceiveVideo(object? obj)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, PORT);
            while (true)
            {
                try
                {
                    byte[] packet = _udpClient.Receive(ref iPEndPoint);
                    if (packet.Length < 2) continue;
                    int packetIndex = packet[0];
                    byte[] data = new byte[packet.Length - 4];
                    Buffer.BlockCopy(packet, 4, data, 0, data.Length);
                    if (packetIndex == 0)
                    {
                        _imageBuffer.Clear();
                    }
                    _imageBuffer.AddRange(data);

                    if (data.Length < MAX_PACKET_SIZE - 1)
                    {
                        ShowImage(_imageBuffer.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ShowImage(byte[] jpegData)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(jpegData))
                {
                    Image image = Image.FromStream(ms);
                    if (_playerPictureBox.InvokeRequired)
                    {
                        _playerPictureBox.Invoke(new Action(() =>
                            _playerPictureBox.Image = image));
                    }
                    else
                    {
                        _playerPictureBox.Image = image;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
