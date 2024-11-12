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
using winform.user_control;
using Guna.UI2.WinForms;
using static Guna.UI2.Native.WinApi;
using System.Text.Json.Serialization;
using System.Text.Json;

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
        private Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private static NetworkStream stream;
        private bool isRunning = true;
        List<UserControl> chatlist = new List<UserControl>();
        List<Avatar> avatars = new List<Avatar>();
        private void WaitMess()
        {
            while (true)
            {
                string message = NetworkUntil.Reader(stream);
                if (message == null) {
                    break;
                }
                string[] mess = message.Split('|');
                panel_chat.Invoke(new Action(() =>
                {
                    Chat_box_left chat_box_left = new Chat_box_left();
                    chat_box_left.LabelName = mess[0];
                    chat_box_left.LabelChat = mess[1];
                    chat_box_left.Location = new Point(0, chatlist.Count * chat_box_left.Height);
                    chatlist.Add(chat_box_left);
                    panel_chat.Controls.Add(chat_box_left);
                }));
            }
        }
        private void SendMess(string message)
        {
            NetworkUntil.Writer(stream, "string" ,message);
        }
        private void Render_avatar()
        {
            Avatar avatar = new Avatar();
            avatar.LabelName = "test";
            avatar.LabelStatus = "test";
            avatar.Location = new Point(0, avatars.Count*avatar.Height);
            friend_user.Controls.Add(avatar);
            avatar.MouseDown += (sender, e) =>
            {
                panel_chat.Controls.Clear();
                chatlist.Clear();
            };
            foreach (Control control in avatar.Controls)
            {
                control.MouseDown += (sender, e) =>
                {
                    panel_chat.Controls.Clear();
                    chatlist.Clear();
                };
            }
            avatars.Add(avatar);
        }
        private void Form_chat_Load(object sender, EventArgs e)
        {
            Avatar avatar = new Avatar();
            avatar.LabelName = user.User_name;
            avatar.LabelStatus = "online";
            avatar.BackColor = Color.FromArgb(36,38,42);
            avatar.BorderStyle = BorderStyle.None;
            avatar.Size = new Size(300, 70);
            Main_user.Controls.Add(avatar);
            for(int i = 0; i < 10; i++)
            {
                Render_avatar();
            }
            SendMess(user.User_name);
        }
        private void Button_seen_mess(object sender, EventArgs e)
        {
            SendMess($"{user.User_name}|{guna2TextBox1.Text}");
            Chat_box_right chat_box_right = new Chat_box_right();
            chat_box_right.LabelName = user.User_name;
            chat_box_right.LabelChat = guna2TextBox1.Text;
            chat_box_right.Location = new Point(panel_chat.Width- chat_box_right.Width, chatlist.Count * chat_box_right.Height);
            panel_chat.Controls.Add(chat_box_right);
            chatlist.Add(chat_box_right);
            guna2TextBox1.ResetText();
        }
        #region đoạn code để di chuyển form
        // Khai báo biến để lưu trữ trạng thái kéo
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        // Sự kiện MouseDown trên form để bắt đầu kéo
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        // Sự kiện MouseMove trên form để thay đổi vị trí khi đang kéo
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        // Sự kiện MouseUp trên form để dừng kéo
        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        #endregion

        private void Form_chat_FormClosed(object sender, FormClosedEventArgs e)
        {
            user.User_comm = "Logout";
            string user_close = JsonSerializer.Serialize(user);
            NetworkUntil.Writer(stream, "json",user_close);
        }
    }
}
