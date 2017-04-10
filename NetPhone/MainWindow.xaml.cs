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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.Win32;
using System.IO;

namespace NetPhone
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string CONFIG_NAME = "User.xml";
        private string m_XMLFileFullPath = "";
        string m_dispName = "", m_passwordBox = "", m_isAuto = "0", m_isAutoRun = "0";
        System.Windows.Forms.NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            IconShow();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void x_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ___Click(object sender, RoutedEventArgs e)
        {
            QPan_MiniMizedToTuoPan();
            this.WindowState = WindowState.Minimized;
        }

        private void labelReg_MouseEnter(object sender, MouseEventArgs e)
        {
            this.labelReg.Foreground = Brushes.Blue;
        }

        private void labelReg_MouseLeave(object sender, MouseEventArgs e)
        {
            this.labelReg.Foreground = Brushes.White;
        }

        private void labelReg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string parameter = "0";

            // 启动
            Process.Start(@"RegisterAndPay.exe", parameter);
        }

        private void label1_MouseEnter(object sender, MouseEventArgs e)
        {
            this.label1.Foreground = Brushes.Blue;
        }

        private void label1_MouseLeave(object sender, MouseEventArgs e)
        {
            this.label1.Foreground = Brushes.White;
        }

        private void label1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string strQQ = "http://www.qqsxy.com/";
            Process.Start(strQQ);
        }

        private void buttonLogIn_Click(object sender, RoutedEventArgs e)
        {
            string dispName = this.textBoxDisplayName.Text;
            string passwordBox = this.passwordBox.Password.ToString();

            if(dispName.Length <= 0)
            {
                MessageBox.Show("用户名不能为空!");
                return;
            }

            if (passwordBox.Length <= 0)
            {
                MessageBox.Show("密码不能为空!");
                return;
            }

            if (!File.Exists(m_XMLFileFullPath))
            {
                MessageBox.Show("配置文件被损坏，程序无法启动!");
                return;
            }

            try
            {
                XmlDocument XMLDocFriend = new XmlDocument();
                XMLDocFriend.Load(m_XMLFileFullPath);

                XmlNode root = XMLDocFriend.SelectSingleNode("root");
                XmlElement chiedNode = root.SelectSingleNode("user") as XmlElement;
                chiedNode.SetAttribute("name", dispName);
                chiedNode.SetAttribute("password", passwordBox);

                // 是否记住密码
                if ((bool)this.checkBoxPassword.IsChecked)
                {
                    chiedNode.SetAttribute("isAuto", "1");
                }
                else
                {
                    chiedNode.SetAttribute("isAuto", "0");
                }

                // 自启动设置
                if ((bool)this.checkBoxAutoRun.IsChecked)
                {
                    chiedNode.SetAttribute("isAutoRun", "1");
                }
                else
                {
                    chiedNode.SetAttribute("isAutoRun", "0");
                }

                XMLDocFriend.Save(CONFIG_NAME);

                // 个别机器还不行，暂时注掉
                //if ((bool)this.checkBoxAutoRun.IsChecked)
                //{
                //    RunWhenStart(true, "NetPhone", System.Windows.Forms.Application.ExecutablePath);
                //}
                //else
                //{
                //    if (m_isAutoRun == "1")
                //    {
                //        RunWhenStart(false, "NetPhone", System.Windows.Forms.Application.ExecutablePath);
                //    }
                //}

                // 启动主程序
                string exeFileFullPath = System.Windows.Forms.Application.ExecutablePath;
                exeFileFullPath = exeFileFullPath.Substring(0, exeFileFullPath.LastIndexOf("\\") + 1);
                Process.Start(exeFileFullPath + "BogheApp.exe");

                this.Close();
            }
            catch (Exception)
            {
                // .. error log
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            XmlDocument XMLDocFriend = new XmlDocument();
            string exeFileFullPath = System.Windows.Forms.Application.ExecutablePath;
            exeFileFullPath = exeFileFullPath.Substring(0, exeFileFullPath.LastIndexOf("\\") + 1);

            // 读取User.xml文件
            m_XMLFileFullPath = exeFileFullPath + CONFIG_NAME;

            try
            {
                XMLDocFriend.Load(m_XMLFileFullPath);
                XmlNodeList notes = XMLDocFriend.DocumentElement.ChildNodes;
                XmlElement user = (XmlElement)notes[0];

                m_dispName = user.GetAttribute("name");
                m_passwordBox = user.GetAttribute("password");
                m_isAuto = user.GetAttribute("isAuto");
                m_isAutoRun = user.GetAttribute("isAutoRun");

                this.textBoxDisplayName.Text = m_dispName;

                if (m_isAuto == "1")
                {
                    this.passwordBox.Password = m_passwordBox;
                    this.checkBoxPassword.IsChecked = true;
                }

                if (m_isAutoRun == "1")
                {
                    this.checkBoxPassword.IsChecked = true;
                }
            }
            catch (Exception)
            {
                // .. error log
            }
        }

        public static void RunWhenStart(bool Started, string name, string path) 
        {
            RegistryKey HKLM = Registry.LocalMachine; 
            RegistryKey Run = HKLM.CreateSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\");

            if (Started == true) 
            {
                Run.SetValue(name, path);
                HKLM.Close();
            }
            else 
            {
                Run.DeleteValue(name);
                HKLM.Close(); 
            }
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            QPan_OpenFromTuoPan();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            QPan_MiniMizedToTuoPan();
        }

        public void QPan_MiniMizedToTuoPan()
        {
            this.ShowInTaskbar = false;
            this.notifyIcon.Visible = true;
        }

        public void QPan_OpenFromTuoPan()
        {
            this.Visibility = Visibility.Visible;

            this.ShowInTaskbar = true;

            this.WindowState = WindowState.Normal;

            this.notifyIcon.Visible = false;
        }

        public void IconShow()
        {
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.BalloonTipText = "小精灵网络电话";
            this.notifyIcon.Text = "小精灵网络电话";
            this.notifyIcon.Icon = new System.Drawing.Icon("icon.ico");
            this.notifyIcon.Visible = false;
            notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
            this.notifyIcon.ShowBalloonTip(500);
        }
    }
}