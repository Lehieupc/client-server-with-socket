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
    public partial class Chat_box_right : UserControl
    {
        public Chat_box_right()
        {
            InitializeComponent();
        }
        public string LabelName
        {
            get { return guna2HtmlLabel1.Text; }
            set { 
                guna2HtmlLabel1.Text = value;
                guna2HtmlLabel1.Location = new Point(guna2HtmlLabel1.Location.X - guna2HtmlLabel1.Width+48, 3);
            }
        }
        public string LabelChat
        {
            get { return guna2HtmlLabel2.Text; }
            set
            {
                guna2HtmlLabel2.Text = value;
                guna2HtmlLabel2.Location = new Point(guna2HtmlLabel2.Location.X-guna2HtmlLabel2.Width+32,33);
            }
        }
    }
}
