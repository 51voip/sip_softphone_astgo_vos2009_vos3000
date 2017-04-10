using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RegisterAndPay
{
    public partial class FormModifyPassword : Form
    {
        public FormModifyPassword()
        {
            InitializeComponent();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (textBoxNewPassword1.Text.Length == 0 || textBoxNewPassword2.Text.Length == 0)
            {
                MessageBox.Show("新密码不能为空");
                return;
            }

            if (textBoxNewPassword1.Text != textBoxNewPassword2.Text)
            {
                MessageBox.Show("两次输入的新密码不相同，请重新输入");
                textBoxNewPassword1.Text = "";
                textBoxNewPassword2.Text = "";
                return;
            }

            string newPassword1 = textBoxNewPassword1.Text;
            string newPassword2 = textBoxNewPassword2.Text;

            if (MessageBox.Show("确认要修改密码吗?", " ", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2).ToString() == "Yes")
            {
                //
            }

            this.Close();
        }

        private void textBoxTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                e.Handled = false;
            }
        }
    }
}
