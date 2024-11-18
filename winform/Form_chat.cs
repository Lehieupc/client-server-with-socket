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
using System.Web.UI.WebControls;

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
        private void Panel_chat_word_add(string[] mess)
        {
            Chat_box_left chat_box_left = new Chat_box_left();
            chat_box_left.LabelName = mess[1];
            chat_box_left.LabelChat = mess[2];
            int currentY = 0;
            foreach (Control control in panel_chat.Controls)
            {
                if (control != panel_chat_with_friend)
                {
                    currentY = Math.Max(currentY, control.Bottom); // Lấy điểm thấp nhất của các control trong panel
                }
            }
            chat_box_left.Location = new Point(0, currentY);
            chatlist_word.Add(chat_box_left);
            panel_chat.Controls.Add(chat_box_left);
            panel_chat.VerticalScroll.Value = panel_chat.VerticalScroll.Maximum;
            panel_chat.PerformLayout();
        }
        private void Panel_chat_with_friend_add(string[] mess)
        {
            Chat_box_left chat_box_left = new Chat_box_left();
            chat_box_left.LabelName = mess[1];
            chat_box_left.LabelChat = mess[2];
            int currentY = 0;
            foreach (Control control in panel_chat_with_friend.Controls)
            {
                currentY = Math.Max(currentY, control.Bottom); // Lấy điểm thấp nhất của các control trong panel
            }
            chat_box_left.Location = new Point(0, currentY);
            chatlist_friend.Add(chat_box_left);
            panel_chat_with_friend.Controls.Add(chat_box_left);
            panel_chat_with_friend.VerticalScroll.Value = panel_chat_with_friend.VerticalScroll.Maximum;
            panel_chat_with_friend.PerformLayout();
        }
        private void Render_avatar(Friend_user_and_cm_id friend_User_And_Cm_Id)
        {
            Avatar avatar = new Avatar();
            avatar.LabelName = friend_User_And_Cm_Id.User_name;
            avatar.LabelStatus = friend_User_And_Cm_Id.User_status;
            avatar.Location = new Point(0, (avatars.Count + 1) * avatar.Height);
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
        private void Render_unfriended(Unfriended unfriended)
        {
            Unfriend unfriend = new Unfriend();
            unfriend.LabelName = unfriended.User_Name;
            unfriend.Dock = DockStyle.Top;
            if(unfriended.Status == "pending" && unfriended.Receiver_name == unfriended.User_Name)
            {
                unfriend.add_friend.Hide();
                unfriend.cancel.Hide();
            }
            else if (unfriended.Status == "pending" && unfriended.Receiver_name != unfriended.User_Name)
            {
                unfriend.add_friend.Hide();
            }
            unfriend.add_friend.Click += (sender, e) =>
            {
                SendMess($"friendship|add_friend|{user.User_name}|{unfriend.LabelName}");
            };
            unfriend.cancel.Click += (sender, e) =>
            {
                SendMess($"friendship|cancel|{user.User_name}|{unfriend.LabelName}");
            };
            unfriend.refuse.Click += (sender, e) =>
            {
                SendMess($"friendship|cancel|{user.User_name}|{unfriend.LabelName}");
            };
            unfriend.accept.Click += (sender, e) =>
            {
                SendMess($"friendship|accept|{user.User_name}|{unfriend.LabelName}");
                UnfriendedList.Controls.Remove(unfriend);
                SendMess(user.User_name);
            };
            UnfriendedList.Controls.Add(unfriend);
        }
        private void WaitMess()
        {
            while (true)
            {
                string response = NetworkUntil.Reader(stream);
                if (response == null) break;
                string[] mess = response.Split('|');
                if (mess.Length >= 3)
                {
                    if (mess[0] == "word_chat")
                    {
                        panel_chat.Invoke(new Action(() =>
                        {
                            Panel_chat_word_add(mess);
                        }));
                    }
                    else
                    {
                        if (panel_chat_with_friend.InvokeRequired)
                        {
                            panel_chat_with_friend.BeginInvoke(new Action(() =>
                            {
                                Panel_chat_with_friend_add(mess);
                            }));
                        }
                        else
                        {
                            Panel_chat_with_friend_add(mess);
                        }
                    }
                }
                else if (mess.Length == 2)
                {
                    List<Friend_user_and_cm_id> friends = JsonSerializer.Deserialize<List<Friend_user_and_cm_id>>(mess[1]);
                    if (mess[0] == "list_friend")
                    {
                        avatars.Clear();
                        friend_user.Invoke(new Action(() =>
                        {
                            
                            foreach (Control control in friend_user.Controls)
                            {
                                if (control.Name == "ChatWord" || control == UnfriendedList)
                                {
                                    continue;
                                }
                                else
                                {
                                    friend_user.Controls.Remove(control);
                                }
                            }
                            foreach (var friend in friends)
                            {
                                Render_avatar(friend);
                            }
                        }));
                    }
                    else if (mess[0] == "list_unfriended")
                    {
                        List<Unfriended> unfriendeds = JsonSerializer.Deserialize<List<Unfriended>>(mess[1]);
                        UnfriendedList.Invoke(new Action(() =>
                        {
                            foreach (var unfriended in unfriendeds)
                            {
                                Render_unfriended(unfriended);
                            }
                        }));
                    }
                    else if (mess[0] == "list_message")
                    {
                        List<Dictionary<string, string>> list_message = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(mess[1]);
                        panel_chat_with_friend.Invoke(new Action(() =>
                        {
                            panel_chat_with_friend.Controls.Clear();
                            chatlist_friend.Clear();
                            foreach (var message in list_message) {
                                if (message["sender_name"] != user.User_name)
                                {
                                    Chat_box_left chat_box_left = new Chat_box_left();
                                    chat_box_left.LabelName = message["sender_name"];
                                    chat_box_left.LabelChat = message["message_content"];
                                    chat_box_left.Location = new Point(0, chatlist_friend.Count * chat_box_left.Height);
                                    chatlist_friend.Add(chat_box_left);
                                    panel_chat_with_friend.Controls.Add(chat_box_left);
                                }
                                else
                                {
                                    Chat_box_right chat_box_right = new Chat_box_right();
                                    chat_box_right.LabelName = message["sender_name"];
                                    chat_box_right.LabelChat = message["message_content"];
                                    chat_box_right.Location = new Point(panel_chat_with_friend.Width - chat_box_right.Width-20, chatlist_friend.Count * chat_box_right.Height);
                                    panel_chat_with_friend.Controls.Add(chat_box_right);
                                    chatlist_friend.Add(chat_box_right);
                                }
                            }
                            panel_chat_with_friend.VerticalScroll.Value = panel_chat_with_friend.VerticalScroll.Maximum;
                            panel_chat_with_friend.PerformLayout();
                        }));
                    }
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
            panel_chat.AutoScroll = false;
            panel_chat_with_friend.Show();
        }
        private void Form_chat_Load(object sender, EventArgs e)
        {
            Avatar avatar = new Avatar();
            avatar.LabelName = user.User_name;
            avatar.LabelStatus = "online";
            avatar.BackColor = Color.FromArgb(36,38,42);
            avatar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            avatar.Size = new Size(300, 70);
            Avatar[] avatars1 = new Avatar[10];
            Main_user.Controls.Add(avatar);
            SendMess(user.User_name);
        }
        private void Button_seen_mess(object sender, EventArgs e)
        {
            string test_text = guna2TextBox1.Text.Trim();
            if (test_text != "")
            {
                SendMess($"{user.User_name}|{guna2TextBox1.Text}|{mess_word.Name}|{mess_word.Text}");
                Chat_box_right chat_box_right = new Chat_box_right();
                chat_box_right.LabelName = user.User_name;
                chat_box_right.LabelChat = guna2TextBox1.Text;
                int currentY = 0;
                if (mess_word.Name == "mess_word")
                {
                    foreach (Control control in panel_chat.Controls)
                    {
                        if (control != panel_chat_with_friend)
                        {
                            currentY = Math.Max(currentY, control.Bottom); // Lấy điểm thấp nhất của các control trong panel
                        }
                    }

                    // Đặt vị trí Y tiếp theo
                    chat_box_right.Location = new Point(
                        panel_chat.Width - chat_box_right.Width - 20, // Canh lề phải
                        currentY
                    );
                    panel_chat.Controls.Add(chat_box_right);
                    panel_chat.VerticalScroll.Value = panel_chat.VerticalScroll.Maximum;
                    panel_chat.PerformLayout();
                    chatlist_word.Add(chat_box_right);
                }
                else
                {
                    foreach (Control control in panel_chat_with_friend.Controls)
                    {
                        currentY = Math.Max(currentY, control.Bottom); // Lấy điểm thấp nhất của các control trong panel
                    }

                    chat_box_right.Location = new Point(panel_chat_with_friend.Width - chat_box_right.Width - 20, currentY);

                    // Kiểm tra nếu đang ở UI thread, nếu không dùng Invoke
                    if (panel_chat_with_friend.InvokeRequired)
                    {
                        panel_chat_with_friend.Invoke(new Action(() =>
                        {
                            panel_chat_with_friend.Controls.Add(chat_box_right);
                            panel_chat_with_friend.VerticalScroll.Value = panel_chat_with_friend.VerticalScroll.Maximum;
                            panel_chat_with_friend.PerformLayout();
                        }));
                    }
                    else
                    {
                        // Nếu đang ở UI thread, trực tiếp thêm control
                        panel_chat_with_friend.Controls.Add(chat_box_right);
                        panel_chat_with_friend.VerticalScroll.Value = panel_chat_with_friend.VerticalScroll.Maximum;
                        panel_chat_with_friend.PerformLayout();
                    };
                    chatlist_friend.Add(chat_box_right);
                }
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
            socket.Close();
            socket = null;
        }
        private void ChatWord_Click(object sender, EventArgs e)
        {
            panel_chat_with_friend.Hide();
            panel_chat.AutoScroll = true;
            panel_chat.VerticalScroll.Value = panel_chat.VerticalScroll.Maximum;
            panel_chat.PerformLayout();
            mess_word.Name = "mess_word";
            mess_word.Text = ChatWord.LabelName;
        }

        private void ChatWord_Load(object sender, EventArgs e)
        {
            ChatWord.LabelName = "Nhóm chung";
            ChatWord.LabelStatus = "online";
        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            if (UnfriendedList.Visible == false)
            {
                label1.Text = "Lời mời và gợi ý bạn bè";
                friend_user.AutoScroll = false;
                UnfriendedList.Show();
            }
            else
            {
                label1.Text = "Đoạn chat";
                friend_user.AutoScroll = true;
                UnfriendedList.Hide();
            }
        }
    }
}
