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
using Guna.UI2.Material.Animation;

namespace winform
{
    public partial class Form_login : Form
    {

        public Form_login()
        {
            InitializeComponent();
        }
        private static readonly IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1308);
        private string ConnSv(string request)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPoint);
            NetworkStream stream = new NetworkStream(socket);
            NetworkUntil.Writer(stream, "json" ,request);
            string response = NetworkUntil.Reader(stream);
            socket.Close();
            return response;
        }
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            password.UseSystemPasswordChar= !Checkbox.Checked;
        }

        private void Sign_in(object sender, EventArgs e)
        {
            User user = new User("Login",account.Text, password.Text);
            string data_user = JsonSerializer.Serialize(user);
            string response = ConnSv(data_user);
            MessageBox.Show(response, "Thông báo", MessageBoxButtons.OK);
            if (string.Compare(response, "Tài khoản hoặc mật khâu không đúng", true) != 0){
                this.Hide();
                Form_chat form_chat = new Form_chat(user);
                form_chat.FormClosed += (s, args) =>
                {
                    // Hiển thị lại Form_login khi Form_chat đóng
                    this.Show();
                };
                form_chat.ShowDialog();
            }
            account.ResetText();
            password.ResetText();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            password.UseSystemPasswordChar = true;
        }

        private void Label_SignUp_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form_SignUp form_signup = new Form_SignUp();
            form_signup.FormClosed += (s, args) =>
            {
                // Hiển thị lại Form_login khi Form_chat đóng
                this.Show();
            };
            form_signup.ShowDialog();
        }
    }
}
