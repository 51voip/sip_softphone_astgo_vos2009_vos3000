using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RegisterAndPay
{
    public partial class FormPay : Form
    {
        private string m_telNum = "";
        public FormPay(string telNum)
        {
            InitializeComponent();
            m_telNum = telNum;
        }

        private void panelPay_Click(object sender, EventArgs e)
        {
            string amount = "";
            string payStr = "https://shenghuo.alipay.com/send/payment/fill.htm?optEmail=fefads@126.com&title=";
            payStr += m_telNum;
            payStr += "&memo=&payAmount=";

            if (this.radioButton1.Checked)
            {
                amount = "500";
            }
            else if (this.radioButton2.Checked)
            {
                amount = "300";
            }
            else if (this.radioButton3.Checked)
            {
                amount = "200";
            }
            else if (this.radioButton4.Checked)
            {
                amount = "100";
            }
            else if (this.radioButton4.Checked)
            {
                amount = "50";
            }
            else
            {
                amount = "500";
            }

            payStr += amount;

            Process.Start(payStr);

            this.Close();
        }

        private void panelPay_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
