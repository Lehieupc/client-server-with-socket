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
using System.Messaging;

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
        List<UserControl> chatlist_word = new List<UserControl>();
        List<UserControl> chatlist_friend = new List<UserControl>();
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
                if (mess.Length >= 3)
                {
                    if (mess[0] == "word_chat")
                    {
                        panel_chat.Invoke(new Action(() =>
                        {
                            Chat_box_left chat_box_left = new Chat_box_left();
                            chat_box_left.LabelName = mess[1];
                            chat_box_left.LabelChat = mess[2];
                            chat_box_left.Location = new Point(0, chatlist_word.Count * chat_box_left.Height);
                            chatlist_word.Add(chat_box_left);
                            panel_chat.Controls.Add(chat_box_left);
                        }));
                    }
                    else
                    {
                        panel_chat_with_friend.Invoke(new Action(() =>
                        {
                            Chat_box_left chat_box_left = new Chat_box_left();
                            chat_box_left.LabelName = mess[1];
                            chat_box_left.LabelChat = mess[2];
                            chat_box_left.Location = new Point(0, chatlist_friend.Count * chat_box_left.Height);
                            chatlist_friend.Add(chat_box_left);
                            panel_chat_with_friend.Controls.Add(chat_box_left);
                        }));
                    }
                }
                else
                {
                    friend_user.Invoke(new Action(() =>
                    {
                        avatars.Clear();
                        foreach(Control control in friend_user.Controls)
                        {
                            if(control.Name != "ChatWord")
                            {
                                friend_user.Controls.Remove(control);
                            }
                        }
                        List<Friend_user_and_cm_id> friends = JsonSerializer.Deserialize<List<Friend_user_and_cm_id>>(message);
                        foreach(var friend in friends)
                        {
                            Render_avatar(friend);
                        }
                    }));
                }
            }
        }
        private void SendMess(string message)
        {
           NetworkUntil.Writer(stream, "string", message);
        }
        private void avatar_and_control_click(Friend_user_and_cm_id friend_User_And_Cm_Id)
        {
            mess_word.Name = friend_User_And_Cm_Id.Cm_id.ToString();
            mess_word.Text = friend_User_And_Cm_Id.User_name;
            SendMess($"chatwith|{friend_User_And_Cm_Id.Cm_id}");
            panel_chat_with_friend.Show();
        }
        private void Render_avatar(Friend_user_and_cm_id friend_User_And_Cm_Id)
        {
            Avatar avatar = new Avatar();
            avatar.LabelName = friend_User_And_Cm_Id.User_name;
            avatar.LabelStatus = friend_User_And_Cm_Id.User_status;
            avatar.Location = new Point(0, (avatars.Count+1)*avatar.Height);
            friend_user.Controls.Add(avatar);
            avatar.MouseDown += (sender, e) =>
            {
                avatar_and_control_click(friend_User_And_Cm_Id);
            };
            foreach (Control control in avatar.Controls)
            {
                control.MouseDown += (sender, e) =>
                {
                    avatar_and_control_click(friend_User_And_Cm_Id);
                };
            }
            avatars.Add(avatar);
        }
        private void Form_chat_Load(object sender, EventArgs e)
        {
            Avatar avatar = new Avatar();
            avatar.Name = "1";
            avatar.LabelName = user.User_name;
            avatar.LabelStatus = "online";
            avatar.BackColor = Color.FromArgb(36,38,42);
            avatar.BorderStyle = BorderStyle.None;
            avatar.Size = new Size(300, 70);
            Main_user.Controls.Add(avatar);
            Thread thread = new Thread(() => {
                while (true)
                {
                    NetworkUntil.Writer(stream, "string", user.User_name);
                    Thread.Sleep(30000);
                }
            });
            thread.Start();
        }
        private void Button_seen_mess(object sender, EventArgs e)
        {
            SendMess($"{user.User_name}|{guna2TextBox1.Text}|{mess_word.Name}|{mess_word.Text}");
            Chat_box_right chat_box_right = new Chat_box_right();
            chat_box_right.LabelName = user.User_name;
            chat_box_right.LabelChat = guna2TextBox1.Text;
            if (mess_word.Name == "mess_word")
            {
                chat_box_right.Location = new Point(panel_chat.Width - chat_box_right.Width, chatlist_word.Count * chat_box_right.Height);
                panel_chat.Controls.Add(chat_box_right);
                chatlist_word.Add(chat_box_right);
            }
            else
            {
                chat_box_right.Location = new Point(panel_chat.Width - chat_box_right.Width, chatlist_friend.Count * chat_box_right.Height);
                panel_chat_with_friend.Controls.Add(chat_box_right);
                chatlist_friend.Add(chat_box_right);
            }
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
            dragCursorPoint = MousePosition; // Lấy vị trí con trỏ chuột
            dragFormPoint = this.Location; // Lưu vị trí hiện tại của form
        }

        // Sự kiện MouseMove trên form để thay đổi vị trí khi đang kéo
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                // Tính toán sự chênh lệch giữa vị trí chuột mới và cũ
                Point diff = Point.Subtract(MousePosition, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff)); // Cập nhật vị trí form
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
        private void ChatWord_Click(object sender, EventArgs e)
        {
            panel_chat_with_friend.Hide();
            mess_word.Name = "mess_word";
            mess_word.Text = ChatWord.LabelName;
        }

        private void ChatWord_Load(object sender, EventArgs e)
        {
            ChatWord.LabelName = "Nhóm chung";
            ChatWord.LabelStatus = "online";
        }
    }
}
