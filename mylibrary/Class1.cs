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

namespace mylibrary
{
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
        public static void Writer(NetworkStream stream,string message) {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
            Console.WriteLine($"data sent: {message} ");
            stream.Flush();
        }
        public static string Reader(NetworkStream stream)
        {
            try
            {
                byte[] bytes = new byte[1024];
                int bytesRead = stream.Read(bytes, 0, bytes.Length);
                string data = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                if (data != "")
                {
                    Console.WriteLine($"data received: {data} ");
                }
                return data;
            }
            catch (Exception)
            {
                return "";
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
        public void Comm_Sql(string comm,string select)
        {
            MySqlCommand command = new MySqlCommand(comm, Conn);
            command.ExecuteNonQuery();
        }
        public string Comm_Str(string select)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(select,Conn);
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
