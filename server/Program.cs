using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using mylibrary;
namespace server
{
    internal class Program
    {
        static void Main(string[] args)
        {    
            List<User> users = new List<User>() {
                new User("hieu","hieule"),
                new User("minhcuong","minc")
            };
            var listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 1308);
            listener.Bind(ipEndPoint);
            listener.Listen(10);
            while (true)
            {
                var socket = listener.Accept();
                NetworkStream stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream);
                string request = reader.ReadLine();
                User user = JsonSerializer.Deserialize<User>(request);
                Console.WriteLine($"User (tk : {user.account}, mk : {user.password})");
                string response = "haha";
               foreach(var user_ in users)
                {
                    if (user_.account == user.account && user_.password == user.password) response = $"chao mung {user.account}";
                    else response = $"tk hoac mk ko dung";
                }
                writer.WriteLine(response);
                writer.Flush();
                socket.Close();
            }
        }
    }
}
