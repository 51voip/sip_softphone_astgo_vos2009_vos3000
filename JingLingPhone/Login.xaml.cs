using BogheApp.Services.Impl;
using BogheCore.Model;
using BogheCore.Services;
using BogheCore.Sip.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        LocalUsers tt = null;
        private readonly ISipService sipService;
        public Login()
        {
            InitializeComponent();
            this.sipService = Win32ServiceManager.SharedManager.SipService;
            this.sipService.onRegistrationEvent += sipService_onRegistrationEvent;
         
        }

        void sipService_onRegistrationEvent(object sender, BogheCore.Sip.Events.RegistrationEventArgs e)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<RegistrationEventArgs>(this.sipService_onRegistrationEvent), sender, new object[] { e });
                return;
            }

            switch (e.Type)
            {
                case RegistrationEventTypes.REGISTRATION_INPROGRESS:
                case RegistrationEventTypes.UNREGISTRATION_INPROGRESS:
   
                    break;

                case RegistrationEventTypes.REGISTRATION_NOK:
                case RegistrationEventTypes.REGISTRATION_OK:
                case RegistrationEventTypes.UNREGISTRATION_NOK:
                case RegistrationEventTypes.UNREGISTRATION_OK:

                    break;
            }
        }

        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            this.DragMove();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Grid)sender).Background = new SolidColorBrush {Color=Color.FromArgb(120,255,255,255) };
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Grid)sender).Background = new SolidColorBrush { Color = Color.FromArgb(0, 255, 255, 255) };
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {






        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush { Color = Color.FromArgb(255, 255, 255, 255) };
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = new SolidColorBrush { Color = Color.FromArgb(50, 255, 255, 255) };
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //注册新用户           
            new Reg().Show ();








        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            info.Visibility = Visibility.Visible;
            int kkk = 0;
            int.TryParse(Uid.Text, out kkk);
            if (kkk==0)
            {
                new M1("用户号必须为数字!").Show(); info.Visibility = Visibility.Hidden; return;

            }
            if (Upass.Password.Length==0)
            {
                new M1("请输入用户密码!").Show(); info.Visibility = Visibility.Hidden; return;
            }
            HttpItem ks = new HttpItem { URL = "http://114.215.109.111:8500/phone/Login?Uid="+Uid.Text +"&password="+Upass.Password };
            string htm = new HttpHelper().GetHtml(ks).Html;
           
            //如果登陆成功则获取账户余额信息
            
            if (htm.Split ('|').GetValue (0).ToString() =="0")
            {
                
                HttpItem httpi = new HttpItem { URL = "http://114.215.109.111:8500/phone/Users?Uid=" + Uid.Text };
               
                htm = new HttpHelper().GetHtml(httpi ).Html;
                string ee = htm.Split('|').GetValue(0).ToString();

                if (ee=="0")
	             {
                    string re=htm.Split('|').GetValue(1).ToString();
		         Common.Uname = re.Split(';').GetValue(0).ToString();
                 double.TryParse ( re.Split(';').GetValue(2).ToString (),out Common.Uye);
                 Common.Uid = Uid.Text;
                 Common.displayName = Uid.Text;
                 Common.Password = Upass.Password;
                
                    //记住用户信息
                 if (C1.IsChecked ==true )
                 {
                     Common.WriteUsers(new LocalUsers { IsSaveUser = (bool)C1.IsChecked, UserName = Uid.Text  , UserPass =Upass.Password , IsAutoLogin =(bool) C2.IsChecked  }, "c:\\windows\\system32\\NetPhone.ken");
                 }
                MainWindow ttt = new MainWindow();

                ttt.Show();
                 this.Close();

	             }else
	            {
                    new M1("用户名错误!").Show(); info.Visibility = Visibility.Hidden; return;
	             }



            }
            else
            {
                MainWindow ttt = new MainWindow();
                ttt.Show();
                this.Close();

                new M1("用户名或者密码错误!").Show(); info.Visibility = Visibility.Hidden; return;
            }


 


        }

        void configurationService_onConfigurationEvent(object sender, BogheCore.Events.ConfigurationEventArgs e)
        {
            if (e.Value == null)
            {
                return;
            }

            switch (e.Folder)
            {
                case Configuration.ConfFolder.IDENTITY:
                    switch (e.Entry)
                    {
                        case Configuration.ConfEntry.DISPLAY_NAME:
                            //this.textBoxDisplayName.Text = e.Value.ToString();
                            break;
                        case Configuration.ConfEntry.IMPI:
                           // this.textBoxPrivateIdentity.Text = e.Value.ToString();
                            break;
                        case Configuration.ConfEntry.IMPU:
                            //this.textBoxPublicIdentity.Text = e.Value.ToString();
                            break;
                        case Configuration.ConfEntry.PASSWORD:
                           // this.passwordBox.Password = e.Value.ToString();
                            break;
                    }
                    break;

                case Configuration.ConfFolder.NETWORK:
                    switch (e.Entry)
                    {
                        case Configuration.ConfEntry.EARLY_IMS:
                            //this.checkBoxEarlyIMS.IsChecked = e.Value.Equals(Boolean.TrueString);
                            break;
                        case Configuration.ConfEntry.REALM:
                            //this.textBoxRealm.Text = e.Value.ToString();
                            break;
                    }
                    break;
            }
        }

        private void Upass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                Button_Click(null, null);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                tt = Common.ReadUsers("c:\\windows\\system32\\NetPhone.ken");

            if (tt.IsSaveUser==true )
            {            
                Uid.Items.Add(tt.UserName );
                Uid.Text = tt.UserName;
                Upass.Password = tt.UserPass;   
                C1.IsChecked = tt.IsSaveUser;
                C2.IsChecked = tt.IsAutoLogin;
            }

            if (C1.IsChecked ==true && C2.IsChecked ==true )
            {
                 Button_Click(null, null);
            }
            }
            catch (Exception)
            {
                
               
            }


        }

  








    }
}
