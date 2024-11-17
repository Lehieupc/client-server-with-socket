namespace winform.user_control
{
    partial class Unfriend
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.accept = new Guna.UI2.WinForms.Guna2Button();
            this.refuse = new Guna.UI2.WinForms.Guna2Button();
            this.cancel = new Guna.UI2.WinForms.Guna2Button();
            this.add_friend = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guna2HtmlLabel1.ForeColor = System.Drawing.Color.White;
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(82, 3);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(75, 27);
            this.guna2HtmlLabel1.TabIndex = 4;
            this.guna2HtmlLabel1.Text = "Name";
            // 
            // guna2CirclePictureBox1
            // 
            this.guna2CirclePictureBox1.Image = global::winform.Properties.Resources.cat_;
            this.guna2CirclePictureBox1.ImageRotate = 0F;
            this.guna2CirclePictureBox1.Location = new System.Drawing.Point(12, 3);
            this.guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            this.guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox1.Size = new System.Drawing.Size(64, 64);
            this.guna2CirclePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2CirclePictureBox1.TabIndex = 3;
            this.guna2CirclePictureBox1.TabStop = false;
            // 
            // accept
            // 
            this.accept.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.accept.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.accept.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.accept.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.accept.Font = new System.Drawing.Font("Segoe UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accept.ForeColor = System.Drawing.Color.White;
            this.accept.Location = new System.Drawing.Point(169, 36);
            this.accept.Name = "accept";
            this.accept.Size = new System.Drawing.Size(101, 31);
            this.accept.TabIndex = 7;
            this.accept.Text = "chấp nhận";
            // 
            // refuse
            // 
            this.refuse.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.refuse.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.refuse.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.refuse.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.refuse.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(49)))), ((int)(((byte)(53)))));
            this.refuse.Font = new System.Drawing.Font("Segoe UI", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.refuse.ForeColor = System.Drawing.Color.White;
            this.refuse.Location = new System.Drawing.Point(82, 36);
            this.refuse.Name = "refuse";
            this.refuse.Size = new System.Drawing.Size(81, 31);
            this.refuse.TabIndex = 8;
            this.refuse.Text = "từ chối";
            this.refuse.Click += new System.EventHandler(this.refuse_Click);
            // 
            // cancel
            // 
            this.cancel.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.cancel.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.cancel.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.cancel.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.cancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(49)))), ((int)(((byte)(53)))));
            this.cancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cancel.ForeColor = System.Drawing.Color.White;
            this.cancel.Location = new System.Drawing.Point(82, 36);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(188, 31);
            this.cancel.TabIndex = 10;
            this.cancel.Text = "Hủy";
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // add_friend
            // 
            this.add_friend.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.add_friend.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.add_friend.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.add_friend.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.add_friend.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.add_friend.ForeColor = System.Drawing.Color.White;
            this.add_friend.Location = new System.Drawing.Point(82, 36);
            this.add_friend.Name = "add_friend";
            this.add_friend.Size = new System.Drawing.Size(188, 31);
            this.add_friend.TabIndex = 11;
            this.add_friend.Text = "Kết bạn";
            this.add_friend.Click += new System.EventHandler(this.add_friend_Click);
            // 
            // Unfriend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(57)))), ((int)(((byte)(63)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.add_friend);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.refuse);
            this.Controls.Add(this.accept);
            this.Controls.Add(this.guna2HtmlLabel1);
            this.Controls.Add(this.guna2CirclePictureBox1);
            this.Name = "Unfriend";
            this.Size = new System.Drawing.Size(278, 78);
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        public Guna.UI2.WinForms.Guna2Button accept;
        public Guna.UI2.WinForms.Guna2Button refuse;
        public Guna.UI2.WinForms.Guna2Button cancel;
        public Guna.UI2.WinForms.Guna2Button add_friend;
    }
}
