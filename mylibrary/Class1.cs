using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mylibrary
{
    public class User
    {
/*        public bool active = false;*/
        public string account { set; get; }
        public string password { set; get; }
        [JsonConstructor]
        public User(string account, string password)
        {
            this.account = account;
            this.password = password;
        }
/*        public User(int id, string account, string password)
        {
            this.id = id;
            this.account = account;
            this.password = password;
        }*/
    }
    public class NetworkUntil
    {
        public static void Writer(NetworkStream stream,string messerge) {
            byte[] bytes = Encoding.UTF8.GetBytes(messerge);
            stream.Write(bytes, 0, bytes.Length);
            Console.WriteLine($"data sent: {messerge} ");
            stream.Flush();
        }
        public static string Render(NetworkStream stream)
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
    }

}
