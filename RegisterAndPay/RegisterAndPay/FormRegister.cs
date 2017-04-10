using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace RegisterAndPay
{
    public partial class FormRegister : Form
    {
        private string SERVER_ADDRESS = "115.239.227.121";

        public FormRegister()
        {
            InitializeComponent();
        }

        #region 控件keyPress消息
        private void textBoxUserNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                e.Handled = false;
            }

            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
        }

        private void textBoxUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                e.Handled = false;
            }

            if (e.KeyChar == '_')
            {
                e.Handled = false;
            }

            if (e.KeyChar >= 'a' && e.KeyChar <= 'z')
            {
                e.Handled = false;
            }

            if (e.KeyChar >= 'A' && e.KeyChar <= 'Z')
            {
                e.Handled = false;
            }

            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
        }

        private void textBoxUserTel_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                e.Handled = false;
            }

            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
        }

        private void textBoxUserPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;

            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                e.Handled = false;
            }

            if (e.KeyChar == '\b')
            {
                e.Handled = false;
            }
        }
        
        #endregion
        private void panelSubmit_Click(object sender, EventArgs e)
        {
            //第一步：http://115.239.227.121/chs/setcustomer.jsp?account=8426516&name=kiven2014&loginName=admin&loginPassword=admin123&type=0
            //第二步：http://115.239.227.121/chs/setphone.jsp?e164=10001&password=123456&loginName=admin&loginPassword=admin123&type=0&account=8426516

            if (!checkActiveIsEmpty())
            {
                if (submitFirstStep())
                {
                    string secondStep = "http://" + SERVER_ADDRESS + "/chs/setphone.jsp?";
                    secondStep += "e164=" + this.textBoxUserTel.Text;
                    secondStep += "&password=" + this.textBoxUserPassword.Text;
                    secondStep += "&loginName=admin&loginPassword=admin123&type=0";
                    secondStep += "&account=" + this.textBoxUserNum.Text;

                    WebRequest wrt = null;
                    WebResponse wrp = null;

                    wrt = WebRequest.Create(secondStep);
                    wrp = wrt.GetResponse();

                    string html = string.Empty;
                    using (Stream stream = wrp.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                        {
                            html = sr.ReadToEnd();
                        }
                    }

                    string[] tempArray = html.Split(';');
                    for (int i = 0; i < tempArray.Length; i++)
                    {
                        tempArray[i] = tempArray[i].Replace("\r\n", "");
                    }

                    if (tempArray.Length > 0)
                    {
                        string[] temp = tempArray[0].Split('|');

                        if (temp.Length > 0)
                        {
                            if (temp[0] == "0")
                            {
                                MessageBox.Show("恭喜，注册成功");
                                this.Close();
                            }
                            else 
                            {
                                MessageBox.Show(html);
                                return;
                            }
                        }
                    }
                }

            }
        }

        bool submitFirstStep()
        {
            bool isRet = false;

            string firstStep = "http://" + SERVER_ADDRESS + "/chs/setcustomer.jsp?";
            firstStep += "account=" + this.textBoxUserNum.Text;
            firstStep += "&name=" + this.textBoxUserName.Text;
            firstStep += "&loginName=admin&loginPassword=admin123&type=0";

            WebRequest wrt = null;
            WebResponse wrp = null;

            wrt = WebRequest.Create(firstStep);
            wrp = wrt.GetResponse();

            string html = string.Empty;
            using (Stream stream = wrp.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                {
                    html = sr.ReadToEnd();
                }
            }

            string[] tempArray = html.Split(';');
            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = tempArray[i].Replace("\r\n", "");
            }

            if (tempArray.Length > 0)
            {
                string[] temp = tempArray[0].Split('|');

                if (temp.Length > 0)
                {
                    if (temp[0] == "0")
                    {
                        isRet = true;
                    }
                }
            }

            if (!isRet)
            {
                MessageBox.Show(html);
            }

            return isRet;
        }

        bool checkActiveIsEmpty()
        {
            bool isRet = false;
            if (this.textBoxUserNum.Text.Length <= 0)
            {
                MessageBox.Show("用户号不能为空");
                return true;
            }

            if (this.textBoxUserName.Text.Length <= 0)
            {
                MessageBox.Show("用户名不能为空");
                return true;
            }

            if (this.textBoxUserTel.Text.Length <= 0)
            {
                MessageBox.Show("电话号码不能为空");
                return true;
            }

            if (this.textBoxUserPassword.Text.Length <= 0)
            {
                MessageBox.Show("密码不能为空");
                return true;
            }

            return isRet;
        }

        private void panelSubmit_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
