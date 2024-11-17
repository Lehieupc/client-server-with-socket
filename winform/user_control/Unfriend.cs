using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winform.user_control
{
    public partial class Unfriend : UserControl
    {
        public Unfriend()
        {
            InitializeComponent();
        }
        public string LabelName
        {
            get { return guna2HtmlLabel1.Text; }
            set { guna2HtmlLabel1.Text = value; }
        }
        private void add_friend_Click(object sender, EventArgs e)
        {
            add_friend.Hide();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            add_friend.Show();
        }

        private void refuse_Click(object sender, EventArgs e)
        {
            add_friend.Show();
            cancel.Show();
        }
    }
}
