namespace RegisterAndPay
{
    partial class FormRegister
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRegister));
            this.textBoxUserNum = new System.Windows.Forms.TextBox();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.textBoxUserTel = new System.Windows.Forms.TextBox();
            this.textBoxUserPassword = new System.Windows.Forms.TextBox();
            this.panelSubmit = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // textBoxUserNum
            // 
            this.textBoxUserNum.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxUserNum.Location = new System.Drawing.Point(206, 155);
            this.textBoxUserNum.Multiline = true;
            this.textBoxUserNum.Name = "textBoxUserNum";
            this.textBoxUserNum.Size = new System.Drawing.Size(270, 40);
            this.textBoxUserNum.TabIndex = 0;
            this.textBoxUserNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUserNum_KeyPress);
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Font = new System.Drawing.Font("宋体", 24F);
            this.textBoxUserName.Location = new System.Drawing.Point(206, 201);
            this.textBoxUserName.Multiline = true;
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(270, 40);
            this.textBoxUserName.TabIndex = 1;
            this.textBoxUserName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUserName_KeyPress);
            // 
            // textBoxUserTel
            // 
            this.textBoxUserTel.Font = new System.Drawing.Font("宋体", 24F);
            this.textBoxUserTel.Location = new System.Drawing.Point(206, 247);
            this.textBoxUserTel.Multiline = true;
            this.textBoxUserTel.Name = "textBoxUserTel";
            this.textBoxUserTel.Size = new System.Drawing.Size(270, 40);
            this.textBoxUserTel.TabIndex = 2;
            this.textBoxUserTel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUserTel_KeyPress);
            // 
            // textBoxUserPassword
            // 
            this.textBoxUserPassword.Font = new System.Drawing.Font("宋体", 24F);
            this.textBoxUserPassword.Location = new System.Drawing.Point(206, 293);
            this.textBoxUserPassword.Multiline = true;
            this.textBoxUserPassword.Name = "textBoxUserPassword";
            this.textBoxUserPassword.PasswordChar = '*';
            this.textBoxUserPassword.Size = new System.Drawing.Size(270, 40);
            this.textBoxUserPassword.TabIndex = 3;
            this.textBoxUserPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxUserPassword_KeyPress);
            // 
            // panelSubmit
            // 
            this.panelSubmit.BackColor = System.Drawing.Color.Transparent;
            this.panelSubmit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelSubmit.Location = new System.Drawing.Point(208, 347);
            this.panelSubmit.Name = "panelSubmit";
            this.panelSubmit.Size = new System.Drawing.Size(222, 53);
            this.panelSubmit.TabIndex = 5;
            this.panelSubmit.Click += new System.EventHandler(this.panelSubmit_Click);
            this.panelSubmit.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSubmit_Paint);
            // 
            // FormRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(612, 469);
            this.Controls.Add(this.panelSubmit);
            this.Controls.Add(this.textBoxUserPassword);
            this.Controls.Add(this.textBoxUserTel);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.textBoxUserNum);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRegister";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "用户注册";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxUserNum;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.TextBox textBoxUserTel;
        private System.Windows.Forms.TextBox textBoxUserPassword;
        private System.Windows.Forms.Panel panelSubmit;
    }
}

