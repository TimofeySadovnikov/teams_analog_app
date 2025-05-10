using System.Net;
using System.Net.Sockets;
using Emgu.CV;
using Emgu.CV.Structure;

namespace teams_analog_server_app
{
    class VideoServer
    {
        private UdpClient _udpClient;
        private IPEndPoint _endPoint;
        private VideoCapture _capture;
        private int _port;
        private const int MaxPacketSize = 65000;

        public VideoServer(string multicastAddress, int port)
        {
            _udpClient = new UdpClient();
            _endPoint = new IPEndPoint(IPAddress.Parse(multicastAddress), port);
            _capture = new VideoCapture();
            _port = port;
        }

        public void Start()
        {
            Console.WriteLine("Video server started...");

            while (true)
            {
                using (Mat frame = _capture.QueryFrame())
                {
                    if (frame != null)
                    {
                        byte[] buffer = frame.ToImage<Bgr, Byte>().ToJpegData();
                        int totalPackets = (buffer.Length + MaxPacketSize - 1) / MaxPacketSize;

                        for (int i = 0; i < totalPackets; i++)
                        {
                            int offset = i * MaxPacketSize;
                            int size = Math.Min(MaxPacketSize, buffer.Length - offset);
                            byte[] packet = new byte[size + 4];
                            Buffer.BlockCopy(BitConverter.GetBytes(i), 0, packet, 0, 4);
                            Buffer.BlockCopy(buffer, offset, packet, 4, size);
                            _udpClient.Send(packet, packet.Length, _endPoint);
                        }
                    }
                }
                Thread.Sleep(30);
            }
        }
    }
}
