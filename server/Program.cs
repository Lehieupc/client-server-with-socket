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
        private static readonly Dictionary<string, Socket> ConnectedClients = new Dictionary<string, Socket>();
        private static readonly Socket listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private static readonly IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1308);
        #region Code để check user
        private static string User_Conn(User user,Database_manager database)
        {
            string response = "";
            string select = $"select password from user_account where user_name = '{user.User_name}'";
            string insert = $"insert into user_account value(null,'{user.User_name}','{user.Password}')";
            switch (user.User_comm)
            {
                case "Login":
                    if (database.Comm_Str(select) == user.Password)
                    {
                        response = $"Chào mừng {user.User_name}";
                    }
                    else response = "Tài khoản hoặc mật khâu không đúng";
                    break;
                case "SignUp":
                    if (database.Comm_Str(select) == null)
                    {
                        database.Comm_Sql(insert, select);
                        response = $"Đăng kí thành công";
                    }
                    else response = "Tên tài khoản đã bị đăng kí";
                    break;
                default:
                    break;
            }
            return response;
        }
        #endregion

        #region Code để lắng nghe request của client 
        private static void Listen_request(Socket socket,NetworkStream stream,Database_manager database)
        {
            while (true)
            {
                string request = NetworkUntil.Reader(stream);
                try
                {
                    User user = JsonSerializer.Deserialize<User>(request);
                    Console.WriteLine($"User (tk : {user.User_name}, mk : {user.Password})");
                    string response = User_Conn(user,database);
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
            Console.OutputEncoding = Encoding.UTF8;
            Database_manager database = new Database_manager("127.0.0.1", "project_mess", "root", "");
            database.OpeDb();
            listener.Bind(ipEndPoint);
            listener.Listen(10);
            Console.WriteLine("Đang lắng nghe client...");
            while (true)
            {
                #region Code để lắng nghe các client kết nối
                Socket socket = listener.Accept();
                NetworkStream stream = new NetworkStream(socket);
                #endregion

                #region Tạo ra luồng riêng để lắng nghe request của các client
                Thread thread = new Thread(() => Listen_request(socket,stream,database));
                thread.Start();
                #endregion
            }
        }
    }
}
