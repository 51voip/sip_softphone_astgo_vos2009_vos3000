using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RegisterAndPay
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length >= 1)
            {
                string argsStr = args[0];
                if (argsStr.CompareTo("0") == 0)
                {
                    Application.Run(new FormRegister());
                }
                else if (argsStr.CompareTo("1") == 0)
                {
                    Application.Run(new FormPay(args[1]));
                }
                else if (argsStr.CompareTo("2") == 0)
                {
                    Application.Run(new FormModifyPassword());
                }
                else
                {
                    Application.Run(new FormRegister());
                }
            }
            else 
            {
                MessageBox.Show("参数传递错误");
                return;
            }
        }
    }
}
