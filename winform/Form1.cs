using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using mylibrary;
namespace winform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string Connsv(string data)
        {
            var ip = "127.0.0.1";
            var ipAddress = IPAddress.Parse(ip);
            int port = 1308;
            var ipEndPoint = new IPEndPoint(ipAddress, port);
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPoint);
            NetworkStream stream = new NetworkStream(socket);
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            string request = data;
            writer.WriteLine(request);
            writer.Flush();
            string response = reader.ReadLine();
            socket.Close();
            return response;
        }
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            password.UseSystemPasswordChar = !checkBox.Checked;
        }

        private void Sign_in(object sender, EventArgs e)
        {
            User user = new User(account.Text,password.Text);
            string data_user = JsonSerializer.Serialize(user);
            MessageBox.Show(Connsv(data_user),"thong bao",MessageBoxButtons.OK);
        }
    }
}
