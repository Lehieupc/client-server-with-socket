using mylibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace winform
{
    public partial class Form_chat : Form
    {
        private User user;
        public Form_chat(User user)
        {
            InitializeComponent();
            socket.Connect(ipEndPoint);
            stream = new NetworkStream(socket);
            Thread Wait_Mess = new Thread(WaitMess);
            Wait_Mess.Start();
            this.user = user;
        }
        private static readonly IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1308);
        private static readonly Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private static NetworkStream stream;
        private void WaitMess()
        {
            while (true)
            {
                string message = NetworkUntil.Render(stream);
                word_chat.Invoke(new Action(() => word_chat.Items.Add(message)));
            }
        }
        private void SendMess(string message)
        {
            NetworkUntil.Writer(stream, message);
        }
        private void Form_chat_Load(object sender, EventArgs e)
        {
            SendMess(user.account);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SendMess($"{user.account}:{textBox1.Text}");
            word_chat.Items.Add($"Bạn : {textBox1.Text}");
        }

        private void Form_chat_FormClosed(object sender, FormClosedEventArgs e)
        {
            socket.Close();
        }
    }
}
