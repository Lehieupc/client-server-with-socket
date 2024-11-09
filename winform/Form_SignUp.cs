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
using System.Text.Json;

namespace winform
{
    public partial class Form_SignUp : Form
    {
        public Form_SignUp()
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
            string response = NetworkUntil.Reader(stream);
            socket.Close();
            return response;
        }

        private void Button_SignUp(object sender, EventArgs e)
        {
            if(password.Text == Re_password.Text)
            {
                User user = new User("SignUp", account.Text, password.Text);
                string data_user = JsonSerializer.Serialize(user);
                string response = ConnSv(data_user);
                MessageBox.Show(response, "Thông báo", MessageBoxButtons.OK);
                account.ResetText();
                password.ResetText();
                Re_password.ResetText();
                if (string.Compare(response, "Tên tài khoản đã bị đăng kí", true) != 0) this.Close();
            }
            else MessageBox.Show("Mật khẩu chưa trùng khớp","Thông báo",MessageBoxButtons.OK);
        }
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            password.UseSystemPasswordChar = !Checkbox.Checked;
        }

        private void Form_SignUp_Load(object sender, EventArgs e)
        {
            password.UseSystemPasswordChar = true;
            Re_password.UseSystemPasswordChar = true;
        }
    }
}
