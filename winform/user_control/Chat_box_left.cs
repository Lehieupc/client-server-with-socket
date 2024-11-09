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
    public partial class Chat_box_left : UserControl
    {
        public Chat_box_left()
        {
            InitializeComponent();
        }
        public string LabelName
        {
            get { return guna2HtmlLabel1.Text; }
            set { guna2HtmlLabel1.Text = value; }
        }
        public string LabelChat
        {
            get { return guna2HtmlLabel2.Text; }
            set
            {
                    guna2HtmlLabel2.Text = value;
            }
        }
    }
}
