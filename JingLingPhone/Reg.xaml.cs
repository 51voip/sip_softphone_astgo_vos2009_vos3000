using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BogheApp
{
    /// <summary>
    /// Mess.xaml 的交互逻辑
    /// </summary>
    public partial class Reg : Window
    {
        public Reg()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush { Color = Color.FromArgb(120, 255, 255, 255) };
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush { Color = Color.FromArgb(10, 255, 255, 255) };
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (r1.Text.Length ==0 )
            {
                new M1("请输入用户名").Show();
                return;
            }
            if (r2.Text.Length == 0)
            {
                new M1("请输入用户号").Show(); return;
            }
            if (r3.Text.Length == 0)
            {
                new M1("请输入手机号码").Show(); return;
            }
            if (r4.Password.Length == 0)
            {
                new M1("请输入用户密码").Show(); return;
            }
            HttpHelper hh = new HttpHelper();

            string htm = hh.GetHtml(new HttpItem { URL = "http://115.239.227.121/chs/reg1.jsp?caller=" + r2.Text + "&password=" + r4.Password }).Html;
            if (htm.Split ('|').GetValue (0).ToString() !="0" )
            {
                  new M1(htm.Split ('|').GetValue (1).ToString().Replace("\r\n","")).Show(); return;
            }
            new M1("恭喜您，您已成功注册，请返回登陆！").Show();





            this.Close();


        }
    }
}
