namespace teams_analog_server_app
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VideoServer server = new VideoServer("239.0.0.1", 11000); // Создание объекта сервера с указанным мультитрансляционным адресом и портом
            server.Start(); // Запуск сервера
        }
    }
}
