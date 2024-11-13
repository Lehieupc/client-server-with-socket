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
            string select = $"select password_hash from users_account where user_name = '{user.User_name}'";
            string update_onl = $"update users_account set status = 'online' where user_name = '{user.User_name}'";
            string update_off = $"update users_account set status = 'offline' where user_name = '{user.User_name}'";
            string insert = $"insert into users_account value(null,'{user.User_name}','{user.Password}','offline')";
            switch (user.User_comm)
            {
                case "Login":
                    if (database.Comm_Str(select) == user.Password)
                    {
                        database.Comm_Sql(update_onl, select);
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
                case "Logout":
                    database.Comm_Sql(update_off, select);
                    break;
                default:
                    break;
            }
            return response;
        }
        #endregion
        #region Code để lấy đoạn chat
        private static List<Friend_user_and_cm_id> Friend_chat(string User_name,Database_manager database)
        {
            string select_user_id = $"select user_id from users_account where user_name = '{User_name}'";
            int user_id = Convert.ToInt32(database.Comm_Str(select_user_id));
            string select_user_names_and_status_in_conversation = $@"
                SELECT ua.user_name, ua.status, conversation_id
                FROM conversation_members cm
                JOIN users_account ua ON cm.user_id = ua.user_id
                WHERE cm.conversation_id IN (
                    SELECT conversation_id 
                    FROM conversation_members 
                    WHERE user_id = {user_id}
                ) AND cm.user_id != {user_id}";
            return database.Comm_Class_List(select_user_names_and_status_in_conversation);
        }
        #endregion

        #region code để lấy tin nhắn
        private static List<Dictionary<string, string>> Friend_mess(string conversation_id, Database_manager database)
        {
            string query = $@"
                SELECT 
                    u.user_name AS sender_name,
                    m.content AS message_content
                FROM 
                    messages m
                JOIN 
                    users_account u ON m.sender_id = u.user_id
                WHERE 
                    m.conversation_id = {conversation_id}
                ORDER BY 
                    m.message_id ASC;";

            return database.Comm_Cows_List(query, "sender_name", "message_content");
        }
        #endregion
        #region Code để lắng nghe request của client 
        private static void Listen_request(Socket socket,NetworkStream stream,Database_manager database)
        {
            while (true)
            {
                string request = NetworkUntil.Reader(stream);
                string[] socket_type = request.Split('\n');
                if (socket_type.Length >= 2)
                {
                    string context_type = socket_type[0];
                    string body = socket_type[1];
                    if (context_type == "json")
                    {
                        User user = JsonSerializer.Deserialize<User>(body);
                        Console.WriteLine($"User (tk : {user.User_name}, mk : {user.Password})");
                        string response = User_Conn(user, database);
                        if (user.User_comm == "Logout") break;
                        NetworkUntil.Writer(stream, response);
                    }
                    else if(context_type == "string")
                    {
                        string[] mess = body.Split('|');
                        if (mess.Length == 2)
                        {
                            Friend_mess(mess[1], database);
                        }
                        else if (mess.Length >= 3)
                        {
                            if (mess[2].ToString() == "mess_word")
                            {
                                foreach (var connectedclient in ConnectedClients)
                                {
                                    if (connectedclient.Value != socket)
                                    {
                                        NetworkStream client_steam = new NetworkStream(connectedclient.Value);
                                        NetworkUntil.Writer(client_steam, $"word_chat|{mess[0]}|{mess[1]}");
                                    }
                                }
                            }
                            else
                            {
                                foreach (var connectedclient in ConnectedClients)
                                {
                                    if (connectedclient.Value != socket && connectedclient.Key == mess[3])
                                    {
                                        NetworkStream client_steam = new NetworkStream(connectedclient.Value);
                                        NetworkUntil.Writer(client_steam, $"friend_chat|{mess[0]}|{mess[1]}");
                                    }
                                }
                            }
                        }
                        else 
                        {
                            ConnectedClients[body] = socket;
                            foreach (var connectedclient in ConnectedClients)
                            {
                                if (connectedclient.Value == socket)
                                {
                                    string friend_chat = JsonSerializer.Serialize(Friend_chat(body, database));
                                    NetworkUntil.Writer(stream, friend_chat);
                                }
                            }
                        };
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
