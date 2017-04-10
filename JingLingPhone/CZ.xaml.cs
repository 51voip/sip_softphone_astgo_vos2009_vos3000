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
    public partial class CZ: Window
    {
        public CZ()
        {
            InitializeComponent();
            r1.Text = Common.displayName;
            r2.Text = Common.displayName;
            r3.Text = Common.Uye.ToString("0.00");

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
                new M1("请重新登陆后再充值！").Show();
                return;
            }
            ComboBoxItem tt=(ComboBoxItem)r4.SelectedItem;


            Process.Start("http://115.28.150.197:8500/PayIndex.asp?PayJe="+tt.Tag +"&PayMore="+Common.displayName );

            this.Close();
    



        }
    }
}
