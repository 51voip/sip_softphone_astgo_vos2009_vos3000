using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class GM : Window
    {
        public GM()
        {
            InitializeComponent();
            r1.Text = Common.displayName;

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
                new M1("请重新登陆后再操作！").Show();
                return;
            }

            if (r3.Password .Length ==0||r3.Password !=r4.Password )
            {
                new M1("两次密码输入不一致！").Show();
                return;
            }
            string cookies = new THJL().GetThjl(r2.Password);
            if  (cookies.Length ==0)
            {
                new M1("原密码错误，请重新输入！").Show();
                return;
            }
            string htm = new HttpHelper().GetHtml(new HttpItem { URL = "http://115.239.227.121/chs/query-terminal-password.jsp", Method = "Post", Cookie = cookies, ContentType = "application/x-www-form-urlencoded",Postdata ="v1="+r2.Password +"&v2="+r3.Password  }).Html;
            if (htm.Split('|').GetValue(0).ToString() == "0")
            {
                new M1("用户密码修改成功！").Show();
                Common.Password = r3.Password;
            }
            else
            {
                new M1("用户密码修改失败！").Show();
            }


        }
    }
}
