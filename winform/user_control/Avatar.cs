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
    public partial class Avatar : UserControl
    {
        public Avatar()
        {
            InitializeComponent();
        }
        public string LabelName {
            get { return guna2HtmlLabel1.Text; }
            set { guna2HtmlLabel1.Text = value; } }
        public string LabelStatus
        {
            get { return guna2HtmlLabel2.Text; }
            set {
                if (guna2HtmlLabel2.InvokeRequired)
                {
                    guna2HtmlLabel2.Invoke(new Action(() => { 
                        guna2HtmlLabel2.Text = value;
                        if (value == "online") guna2HtmlLabel2.ForeColor = Color.Green;
                        else guna2HtmlLabel2.ForeColor = Color.Red;
                    }));
                }
                else
                {
                    guna2HtmlLabel2.Text = value;
                    if (value == "online") guna2HtmlLabel2.ForeColor = Color.Green;
                    else guna2HtmlLabel2.ForeColor = Color.Red;
                }
            ; }
        }
    }
}
