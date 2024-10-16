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
using System.Threading;
using System.Runtime.InteropServices.ComTypes;

namespace server
{
    internal class Program
    {
        // Các 
        private static readonly Dictionary<string, Socket> ConnectedClients = new Dictionary<string, Socket>();
        private static readonly Socket listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private static readonly IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1308);
        #region Code để check user
        private static string User_Conn(User user,List<User> users)
        {
            string response = "";
            foreach (var user_ in users)
            {
                if (user_.account == user.account && user_.password == user.password)
                {
                    response = $"chao mung {user.account}";
                    break;
                }
                else response = $"tk hoac mk ko dung";
            }
            return response;
        }
        #endregion

        #region Code để lắng nghe request của client 
        private static void Listen_request(Socket socket,NetworkStream stream,List<User> users)
        {
            while (true)
            {
                string request = NetworkUntil.Render(stream);
                try
                {
                    User user = JsonSerializer.Deserialize<User>(request);
                    Console.WriteLine($"User (tk : {user.account}, mk : {user.password})");
                    string response = User_Conn(user, users);
                    NetworkUntil.Writer(stream, response);
                }
                catch (JsonException)
                {
                    if(request != "")
                    {
                        string[] mess = request.Split(':');
                        if (mess.Length >= 2)
                        {
                            foreach (var connectedclient in ConnectedClients)
                            {
                                if (connectedclient.Value != socket)
                                {
                                    NetworkStream client_steam = new NetworkStream(connectedclient.Value);
                                    NetworkUntil.Writer(client_steam, $"{mess[0]} : {mess[1]}");
                                }
                            }
                        }
                        else ConnectedClients[request] = socket;
                    }
                }
            }
        }
        #endregion
        static void Main(string[] args)
        {
            List<User> users = new List<User>() {
                new User("hieu","hieule"),
                new User("minhcuong","minc")
            };
            listener.Bind(ipEndPoint);
            listener.Listen(10);
            Console.WriteLine("Dang lang nghe client...");
            while (true)
            {
                #region Code để lắng nghe các client kết nối
                Socket socket = listener.Accept();
                NetworkStream stream = new NetworkStream(socket);
                #endregion

                #region Tạo ra luồng riêng để lắng nghe request của các client
                Thread thread = new Thread(() => Listen_request(socket,stream,users));
                thread.Start();
                #endregion
            }
        }
    }
}
