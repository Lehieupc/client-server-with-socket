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
using System.Threading;

namespace winform
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        private static readonly IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1308);
        private string ConnSv(string request)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPoint);
            NetworkStream stream = new NetworkStream(socket);
            NetworkUntil.Writer(stream, request);
            string response = NetworkUntil.Render(stream);
            socket.Close();
            return response;
        }
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            password.UseSystemPasswordChar = !checkBox.Checked;
        }

        private void Sign_in(object sender, EventArgs e)
        {
                User user = new User(account.Text, password.Text);
                string data_user = JsonSerializer.Serialize(user);
                string response = ConnSv(data_user);
/*                MessageBox.Show(response, "Thông báo", MessageBoxButtons.OK);*/
                if (string.Compare(response, "tk hoac mk ko dung", true) != 0)
                {
                    Form_chat form_chat = new Form_chat(user);
                    form_chat.ShowDialog();
                }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
    }
}
