using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using System.Runtime.Remoting.Messaging;

namespace mylibrary
{
    public class Unfriended
    {
        public string User_Name { get; set; }
        public string Receiver_name { get; set; }
        public string Status { get; set; }
        public Unfriended(string User_name, string Receiver_name,string Status)
        {
            this.User_Name = User_name;
            this.Receiver_name = Receiver_name;
            this.Status = Status;
        }
    }
    public class Friend_user_and_cm_id
    {
        public string User_name { get; set; }
        public string User_status { get; set; }
        public int Cm_id { get; set; }
        [JsonConstructor]
        public Friend_user_and_cm_id(string User_name,string User_status,int Cm_id)
        {
            this.User_name = User_name;
            this.User_status = User_status;
            this.Cm_id = Cm_id;
        }
    }
    public class User
    {
        public string User_comm { get; set; }
        public string User_name { set; get; }
        public string Password { set; get; }
        [JsonConstructor]
        public User(string User_comm,string User_name, string Password)
        {
            this.User_comm = User_comm;
            this.User_name = User_name;
            this.Password = Password;
        }
    }
    public class NetworkUntil
    {
        public static void Writer(NetworkStream stream,string context_type ,string message) {
            message = $"{context_type}\n{message}";
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
            Console.WriteLine($"data sent: {message} ");
            stream.Flush();
        }
        public static void Writer(NetworkStream stream,string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
            Console.WriteLine($"data sent: {message} ");
            stream.Flush();
        }
        public static string Reader(NetworkStream stream)
        {
            try
            {
                byte[] bytes = new byte[1024*1024];
                int bytesRead = stream.Read(bytes, 0, bytes.Length);
                string data = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                if (data != "")
                {
                    Console.WriteLine($"data received: {data} ");
                }
                return data;
            }
            catch
            {
                return null;
            }

        }
    }
    public class Database_manager
    {
        private string ConnString;
        private MySqlConnection Conn;
        public Database_manager(string server,string database,string user,string password)
        {
            ConnString = $"server={server};user={user};database={database};port=3306;password={password}";
            Conn = new MySqlConnection(ConnString);
        }
        public void OpeDb()
        {
            try
            {
                Conn.Open();
                Console.WriteLine("Ket noi den database thanh cong");
            }
            catch (Exception e)
            {
                Console.WriteLine("Loi ket noi" + e.ToString());
            }
        }
        public void Comm_Sql(string comm)
        {
            MySqlCommand command = new MySqlCommand(comm, Conn);
            command.ExecuteNonQuery();
        }
        public List<int> Comm_Cow_List(string select, string cow1)
        {
            List<int> lists = new List<int>();
            try
            {
                MySqlCommand cmd = new MySqlCommand(select, Conn);
                var comstr = cmd.ExecuteReader();
                if (comstr != null)
                {
                    while (comstr.Read())
                    {
                        int query = comstr.GetInt32(cow1);
                        lists.Add(query);
                    }
                    comstr.Close();
                    return lists;
                }
                else
                {
                    return null;
                }
            }
            catch (NullReferenceException)
            {
                return null;
            }

        }

        public List<Dictionary<string, string>> Comm_Dictionary_List(string select,string cow1,string cow2)
        {
            List<Dictionary<string, string>> lists = new List<Dictionary<string, string>>();
            try
            {
                MySqlCommand cmd = new MySqlCommand(select, Conn);
                var comstr = cmd.ExecuteReader();
                if (comstr != null)
                {
                    while (comstr.Read())
                    {
                        Dictionary<string, string> cows = new Dictionary<string, string>();
                        cows.Add(cow1, comstr.GetString(cow1));
                        cows.Add(cow2, comstr.GetString(cow2));
                        lists.Add(cows);
                    }
                    comstr.Close();
                    return lists;
                }
                else
                {
                    return null;
                }
            }
            catch (NullReferenceException)
            {
                return null;
            }

        }
        public List<Friend_user_and_cm_id> Comm_Class_List(string select)
        {
            List<Friend_user_and_cm_id> friend_User_And_Cm_Ids = new List<Friend_user_and_cm_id>();
            try
            {
                MySqlCommand cmd = new MySqlCommand(select, Conn);
                var comstr = cmd.ExecuteReader();
                if (comstr != null)
                {
                    while (comstr.Read())
                    {
                        Friend_user_and_cm_id friend_User_And_Cm_Id = new Friend_user_and_cm_id(
                            comstr.GetString("user_name"),
                            comstr.GetString("status"),
                            comstr.GetInt32("conversation_id")
                            );
                        friend_User_And_Cm_Ids.Add(friend_User_And_Cm_Id);
                    }
                    comstr.Close();
                    return friend_User_And_Cm_Ids;
                }
                else
                {
                    return null;
                }
            }
            catch (NullReferenceException)
            {
                return null;
            }

        }
        public List<Unfriended> Comm_Unfriended_List(string select)
        {
            List<Unfriended> unfriended_list = new List<Unfriended>();
            try
            {
                MySqlCommand cmd = new MySqlCommand(select, Conn);
                var comstr = cmd.ExecuteReader();
                if (comstr != null)
                {
                    while (comstr.Read())
                    {
                        string receiver_name;
                        if (comstr.IsDBNull(1)) receiver_name = null;
                        else receiver_name = comstr.GetString("receiver_name");
                        Unfriended unfriended = new Unfriended(
                            comstr.GetString("other_user"),
                            receiver_name,
                            comstr.GetString("status")
                            );
                        unfriended_list.Add(unfriended);
                    }
                    comstr.Close();
                    return unfriended_list;
                }
                else
                {
                    return null;
                }
            }
            catch (NullReferenceException)
            {
                return null;
            }

        }
        public string Comm_Str(string select)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(select,Conn);
                if (cmd == null)
                {
                    return null;
                }
                object comstr = cmd.ExecuteScalar();
                if (comstr != null)
                {
                    return comstr.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (NullReferenceException)
            {
                return null;
            }

        }
        public void CloseDb()
        {
            Conn.Close();
        }
    }
}
