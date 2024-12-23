﻿using System;
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
            string select_user_name = $"select password_hash from users_account where user_name = '{user.User_name}'";
            string update_onl = $"update users_account set status = 'online' where user_name = '{user.User_name}'";
            string update_off = $"update users_account set status = 'offline' where user_name = '{user.User_name}'";
            string insert_user_account = $"insert into users_account value(null,'{user.User_name}','{user.Password}','offline')";
            string select_list_user_id = $"select user_id from users_account";
            switch (user.User_comm)
            {
                case "Login":
                    if (database.Comm_Str(select_user_name) == user.Password)
                    {
                        database.Comm_Sql(update_onl);
                        response = $"Chào mừng {user.User_name}";
                    }
                    else response = "Tài khoản hoặc mật khâu không đúng";
                    break;
                case "SignUp":
                    if (database.Comm_Str(select_user_name) == null)
                    {
                        List<int> list_user_id = database.Comm_Cow_List(select_list_user_id, "user_id");
                        database.Comm_Sql(insert_user_account);
                        foreach(var user_id in list_user_id)
                        {
                            string insert_friend_blocked = $@"insert into friendships value(
                                {user_id},
                                (select user_id from users_account where user_name = '{user.User_name}'),
                                null,
                                'blocked'
                            )";
                            database.Comm_Sql(insert_friend_blocked);
                        }
                        response = $"Đăng kí thành công";
                    }
                    else response = "Tên tài khoản đã bị đăng kí";
                    break;
                case "Logout":
                    database.Comm_Sql(update_off);
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

            return database.Comm_Dictionary_List(query, "sender_name", "message_content");
        }
        #endregion

        #region code để lưu tin nhắn vào bảng messages
        private static void Insert_messages(string Use_name,string content,string conversation_id,Database_manager database)
        {
            string query_get_user_id = $"SELECT user_id FROM users_account WHERE user_name = '{Use_name}'";
            string use_id = database.Comm_Str(query_get_user_id);
            string query_insert_message = $"INSERT INTO messages (conversation_id, sender_id, content) VALUES ({conversation_id}, {use_id}, '{content}')";
            database.Comm_Sql(query_insert_message);
        }
        #endregion

        #region code để cập nhập bạn bè trong bảng friendship
        private static void Update_friendships(string user_name,string friend_use_name,string e,Database_manager database)
        {
            string query_update_friendships = "";
            switch (e)
            {
                case "add_friend":
                    query_update_friendships = $@"update friendships set
                    receiver_id = (select user_id from users_account where user_name = '{user_name}'),
                    status = 'pending'
                    where
                    user_id = LEAST(
                        (SELECT user_id FROM users_account WHERE user_name = '{user_name}'),
                        (SELECT user_id FROM users_account WHERE user_name =  '{friend_use_name}')
                    ) and
                    friend_id = GREATEST(
                        (SELECT user_id FROM users_account WHERE user_name = '{user_name}'),
                        (SELECT user_id FROM users_account WHERE user_name = '{friend_use_name}')
                    )
                ";
                    database.Comm_Sql(query_update_friendships);
                    break;
                case "cancel":
                    query_update_friendships = $@"update friendships set
                    receiver_id = null,
                    status = 'blocked'
                    where
                    user_id = LEAST(
                        (SELECT user_id FROM users_account WHERE user_name = '{user_name}'),
                        (SELECT user_id FROM users_account WHERE user_name =  '{friend_use_name}')
                    ) and
                    friend_id = GREATEST(
                        (SELECT user_id FROM users_account WHERE user_name = '{user_name}'),
                        (SELECT user_id FROM users_account WHERE user_name = '{friend_use_name}')
                    )
                ";
                    database.Comm_Sql(query_update_friendships);
                    break;
                case "accept":
                    query_update_friendships = $@"update friendships set
                    status = 'accepted'
                    where
                    user_id = LEAST(
                        (SELECT user_id FROM users_account WHERE user_name = '{user_name}'),
                        (SELECT user_id FROM users_account WHERE user_name =  '{friend_use_name}')
                    ) and
                    friend_id = GREATEST(
                        (SELECT user_id FROM users_account WHERE user_name = '{user_name}'),
                        (SELECT user_id FROM users_account WHERE user_name = '{friend_use_name}')
                    )
                    ";
                    string insert_conversations = $@"insert into conversations value(null,'one_to_one');";
                    database.Comm_Sql(insert_conversations);
                    string conversation_id_last = database.Comm_Str($@"select LAST_INSERT_ID() from conversations");
                    string insert_conversation_members = $@"insert into conversation_members value
                        ({conversation_id_last},
                        (SELECT user_id FROM users_account WHERE user_name = '{user_name}')),
                        ({conversation_id_last},
                        (SELECT user_id FROM users_account WHERE user_name = '{friend_use_name}'))
                    ";
                    database.Comm_Sql(insert_conversation_members);
                    database.Comm_Sql(query_update_friendships);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region code để lấy ra danh sách kết bạn
        private static List<Unfriended> Select_friendships(string user_name,Database_manager database)
        {
            string select_friendships = $@"
                SELECT 
                    CASE 
                        WHEN f.user_id = (SELECT user_id FROM users_account WHERE user_name = '{user_name}') THEN u2.user_name
                        ELSE u1.user_name
                    END AS other_user,
                    (select user_name from users_account where user_id = receiver_id) as receiver_name,
                    f.status
                FROM friendships AS f
                JOIN users_account AS u1 ON f.user_id = u1.user_id
                JOIN users_account AS u2 ON f.friend_id = u2.user_id
                WHERE (f.user_id = (SELECT user_id FROM users_account WHERE user_name = '{user_name}') 
                   OR f.friend_id = (SELECT user_id FROM users_account WHERE user_name = '{user_name}')) and f.status != 'accepted';
            ";
            return database.Comm_Unfriended_List(select_friendships);
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
                        if (user.User_comm == "Logout")
                        {
                            socket.Close();
                            socket = null;
                            ConnectedClients.Remove(user.User_name);
                            break;
                        }
                        NetworkUntil.Writer(stream, response);
                    }
                    else if(context_type == "string")
                    {
                        string[] mess = body.Split('|');
                        if (mess.Length == 2)
                        {
                            string list_message = JsonSerializer.Serialize(Friend_mess(mess[1], database));
                            NetworkUntil.Writer(stream,$"list_message|{list_message}");
                        }
                        else if (mess.Length >= 3)
                        {
                            if (mess[0] == "friendship")
                            {
                                Update_friendships(mess[2], mess[3], mess[1],database);
                            }
                            else if (mess[2] == "mess_word")
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
                                Insert_messages(mess[0], mess[1], mess[2], database);
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
                                NetworkStream client_stream = new NetworkStream(connectedclient.Value);
                                string friend_chat = JsonSerializer.Serialize(Friend_chat(connectedclient.Key, database));
                                NetworkUntil.Writer(client_stream, $"list_friend|{friend_chat}");
                                if(connectedclient.Value == socket)
                                {
                                    string list_unfriended = JsonSerializer.Serialize(Select_friendships(body,database));
                                    NetworkUntil.Writer(stream, $"list_unfriended|{list_unfriended}");
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
