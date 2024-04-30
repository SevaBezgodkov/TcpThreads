using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var thread = new Thread(async () => await CreateClient());
            thread.Start();

            await CreateClient();
            thread.Join();
        }

        static async Task CreateClient()
        {
            var words = new string[] { "red", "yellow", "blue", "green" };

            using TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 8888);
            var stream = tcpClient.GetStream();

            var response = new List<byte>();
            int bytesRead = 10; 
            foreach (var word in words)
            {

                byte[] data = Encoding.UTF8.GetBytes(word + '\n');
                await stream.WriteAsync(data);

                while ((bytesRead = stream.ReadByte()) != '\n')
                {
                    response.Add((byte)bytesRead);
                }
                var translation = Encoding.UTF8.GetString(response.ToArray());
                Console.WriteLine($"Слово {word}: {translation}");
                response.Clear();

                await Task.Delay(2000);
            }

            await stream.WriteAsync(Encoding.UTF8.GetBytes("END\n"));
        }
    }
}
