/*
* Boghe IMS/RCS Client - Copyright (C) 2010 Mamadou Diop.
*
* Contact: Mamadou Diop <diopmamadou(at)doubango.org>
*	
* This file is part of Boghe Project (http://code.google.com/p/boghe)
*
* Boghe is free software: you can redistribute it and/or modify it under the terms of 
* the GNU General Public License as published by the Free Software Foundation, either version 3 
* of the License, or (at your option) any later version.
*	
* Boghe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
* without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
* See the GNU General Public License for more details.
*	
* You should have received a copy of the GNU General Public License along 
* with this program; if not, write to the Free Software Foundation, Inc., 
* 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*
*/
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
using BogheApp.Services.Impl;
using BogheApp.Screens;
using BogheControls;
using BogheApp.Services;
using BogheCore.Services;
using BogheControls.Utils;
using BogheCore.Model;
using BogheCore.Events;
using BogheCore.Xcap.Events;
using System.IO;
using log4net;
using BogheCore;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using System.Reflection;


/*
 * XAML Namespaces and Namespace Mapping for WPF XAML: http://msdn.microsoft.com/en-us/library/ms747086.aspx
 * Data Binding: http://msdn.microsoft.com/en-us/library/ms752347.aspx
 * Group Items: http://msdn.microsoft.com/en-us/library/ms754027.aspx
 * Validation: http://www.codeproject.com/KB/WPF/wpfvalidation.aspx
 * 
 * Serialization: http://msdn.microsoft.com/en-us/library/aa302290.aspx
 * 
 * The 96 DPI Solution: http://www.charlespetzold.com/blog/2005/11/250723.html
 * Play MFile: http://en.csharp-online.net/Graphics,_Multimedia,_and_Printing_Recipes%E2%80%94Recipe_8_12
 * */

namespace BogheApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MainWindow));

        private readonly IWin32ScreenService screenService;
        private readonly ISipService sipService;
        private readonly IConfigurationService configurationService;
        private readonly IContactService contactService;
        private readonly ISoundService soundService;
        private readonly IHistoryService historyService;
        private readonly IStateMonitorService stateMonitorService;
        private readonly IXcapService xcapService;
        public const String AVATAR_PATH = ".\\avatar.png";


        private readonly MyObservableCollection<RegistrationInfo> registrations;
        private readonly MyObservableCollection<WatcherInfo> watchers;
        public  System.Timers.Timer aTimer = new System.Timers.Timer();
        

        public MainWindow()
        {
            InitializeComponent();
            aTimer.Interval =10000;
            aTimer.Elapsed += aTimer_Elapsed;
            aTimer.Start();
             //Initialize Screen Service
            this.screenService = Win32ServiceManager.SharedManager.Win32ScreenService;
            this.screenService.SetTabControl(this.Tab1);
            //this.screenService.SetProgressLabel(this.labelProgressInfo);
            Common.MainW = this;
            // Initialize SIP Service
            this.sipService = Win32ServiceManager.SharedManager.SipService;
            this.sipService.onStackEvent += this.sipService_onStackEvent;
            this.sipService.onRegistrationEvent += this.sipService_onRegistrationEvent;
            this.sipService.onInviteEvent += this.sipService_onInviteEvent;
            this.sipService.onMessagingEvent += this.sipService_onMessagingEvent;
            this.sipService.onSubscriptionEvent += this.sipService_onSubscriptionEvent;
            this.sipService.onHyperAvailabilityTimedout += this.sipService_onHyperAvailabilityTimedout;
            
            // Initialize other Services
            this.configurationService = Win32ServiceManager.SharedManager.ConfigurationService;
            this.contactService = Win32ServiceManager.SharedManager.ContactService;
            this.soundService = Win32ServiceManager.SharedManager.SoundService;
            this.historyService = Win32ServiceManager.SharedManager.HistoryService;
            this.stateMonitorService = Win32ServiceManager.SharedManager.StateMonitorService;
            this.xcapService = Win32ServiceManager.SharedManager.XcapService;
            this.configurationService.onConfigurationEvent += this.configurationService_onConfigurationEvent;
            this.xcapService.onXcapEvent += this.xcapService_onXcapEvent;
            this.stateMonitorService.onStateChangedEvent += this.stateMonitorService_onStateChangedEvent;

            // Hook Closeable items
            this.AddHandler(CloseableTabItem.CloseTabEvent, new RoutedEventHandler(this.CloseTab));

            this.registrations = new MyObservableCollection<RegistrationInfo>();
            this.watchers = new MyObservableCollection<WatcherInfo>();


            this.screenService.Show(ScreenType.Authentication);
            this.Uname.Text  = Common.displayName;
            this.Ye.Text  = Common.Uye.ToString("0.00");
            
        }

        void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //获取用户信息

            try
            {
                if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new EventHandler<ElapsedEventArgs>(this.aTimer_Elapsed), sender, new object[] { e });
                    return;
                }

                string htm = new HttpHelper().GetHtml(new HttpItem { URL = "http://114.215.109.111:8500/phone/Users?Uid=" + Common.Uid}).Html;
                string ee = htm.Split('|').GetValue(0).ToString();

                if (ee == "0")
                {
                    string re = htm.Split('|').GetValue(1).ToString();
                    double.TryParse(re.Split(';').GetValue(2).ToString(), out Common.Uye);
                    Ye.Text = Common.Uye.ToString("0.00");
                }
            }
            catch (TargetInvocationException ex)
            {
                LOG.Error(ex);
            }






        }

        private void CloseTab(object source, RoutedEventArgs args)
        {
            TabItem tabItem = args.Source as TabItem;
            if (tabItem != null && tabItem.Content != null)
            {
                BaseScreen screen = tabItem.Content as BaseScreen;
                if (screen != null)
                {
                    this.screenService.Hide(screen);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;

            //this.imageAvatar.Source = MyImageConverter.FromBitmap(Properties.Resources.avatar_48);
            //this.labelFreeText.Content = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.FREE_TEXT, Configuration.DEFAULT_RCS_FREE_TEXT);
            string strName = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.DISPLAY_NAME, Configuration.DEFAULT_IDENTITY_DISPLAY_NAME);
            
            if (strName != "John Doe")
            {
                //this.labelDisplayName.Content = strName;
            }

            if (System.IO.File.Exists(MainWindow.AVATAR_PATH))
            {
                //this.imageAvatar.Source = new ImageSourceConverter().ConvertFromInvariantString(MainWindow.AVATAR_PATH) as ImageSource;
            }

            
            Status[] statues = new Status[]
            {
                new Status("Online", PresenceStatus.Online,"/BogheApp;component/embedded/16/user_16.png"),
                new Status("Busy", PresenceStatus.Busy,"/BogheApp;component/embedded/16/user_busy_16.png"),
                new Status("Be Right Back", PresenceStatus.BeRightBack,"/BogheApp;component/embedded/16/user_back16.png"),
                new Status("Away", PresenceStatus.Away,"/BogheApp;component/embedded/16/user_time_16.png"),
                new Status("On The Phone", PresenceStatus.OnThePhone,"/BogheApp;component/embedded/16/user_onthephone_16.png"),
                new Status("HyperAvailable", PresenceStatus.HyperAvailable,"/BogheApp;component/embedded/16/user_hyper_avail_16.png"),
                new Status("Offline", PresenceStatus.Offline,"/BogheApp;component/embedded/16/user_offline_16.png")
            };
            PresenceStatus status = (PresenceStatus)Enum.Parse(typeof(PresenceStatus),
                this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.STATUS, Configuration.DEFAULT_RCS_STATUS.ToString()));
            int statusIndex = statues.ToList().FindIndex(x => x.Value == status);

            //this.registrations.CollectionChanged += (_sender, _e) =>
            //{
            //    this.MenuItemFile_Registrations.Header = String.Format("Registrations ({0})", this.registrations.Count);
            //};
            this.watchers.CollectionChanged += (_sender, _e) =>
            {
                //this.MenuItemEAB_Authorizations.Header = String.Format("Authorizations ({0})", this.watchers.Count);
            };

            this.Cursor = System.Windows.Input.Cursors.Arrow;

            table1_MouseLeftButtonDown(null, null);
        }

        private void comboBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //PresenceStatus status = (this.comboBoxStatus.SelectedValue as Status).Value;
            //this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.STATUS, status.ToString());

            //if (this.sipService.IsPublicationEnabled)
            //{
            //    this.sipService.PresencePublish(status);
            //}
        }

        private void configurationService_onConfigurationEvent(object sender, ConfigurationEventArgs e)
        {
            if (e.Value == null)
            {
                return;
            }

            switch (e.Folder)
            {
                case Configuration.ConfFolder.IDENTITY:
                    {
                        switch (e.Entry)
                        {
                            case Configuration.ConfEntry.DISPLAY_NAME:
                                {
                                    //this.labelDisplayName.Content = e.Value;
                                    break;
                                }
                        }
                        break;
                    }

                case Configuration.ConfFolder.RCS:
                    {
                        switch (e.Entry)
                        {
                            case Configuration.ConfEntry.FREE_TEXT:
                                {
                                    //this.labelFreeText.Content = e.Value;
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        private void stateMonitorService_onStateChangedEvent(object sender, StateMonitorEventArgs e)
        {
            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<StateMonitorEventArgs>(this.stateMonitorService_onStateChangedEvent), sender, new object[] { e });
                return;
            }

            //String[] topState = this.stateMonitorService.TopState;
            //if (topState == null)
            //{
            //    this.imageIndicatorHourGlass.Visibility = Visibility.Hidden;
            //    this.labelProgressInfo.Content = String.Empty;
            //}
            //else
            //{
            //    this.imageIndicatorHourGlass.Visibility = Visibility.Visible;
            //    this.labelProgressInfo.Content = topState[1];
            //}
        }

        private void xcapService_onXcapEvent(object sender, XcapEventArgs e)
        {
            if (e.Type != XcapEventTypes.PRESCONTENT_DONE)
            {
                return;
            }

            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<XcapEventArgs>(this.xcapService_onXcapEvent), sender, new object[] { e });
                return;
            }

            switch (e.Type)
            {
                case XcapEventTypes.PRESCONTENT_DONE:
                    {
                        System.Drawing.Image avatar;
                        object content = e.GetExtra(XcapEventArgs.EXTRA_CONTENT);
                        if (content != null && content is String)
                        {
                            try
                            {
                                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(content as String)))
                                {
                                    avatar = System.Drawing.Bitmap.FromStream(stream);
                                    //this.imageAvatar.Source = MyImageConverter.FromBitmap(avatar as System.Drawing.Bitmap);
                                }
                            }
                            catch (Exception ex)
                            {
                                LOG.Error("Failed to get avatar", ex);
                            }
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (SessionWindow.Windows)
            {
                SessionWindow.Windows.ForEach(w => w.Close());
            }

            lock (MessagingWindow.Windows)
            {
                MessagingWindow.Windows.ForEach(w => w.Close());
            }
        }

        private void labelFreeText_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.screenService.ScreenOptions.SelectPresenceTab();
            this.screenService.Show(ScreenType.Options);
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.screenService.ScreenOptions.SelectPresenceTab();
            this.screenService.Show(ScreenType.Options);
        }

        private void labelPay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string parameter = "1";
            parameter += " ";
           // parameter += this.labelDisplayName.Content;

            // 启动
            Process.Start(@"RegisterAndPay.exe", parameter);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
           // this.tabControl.SelectedIndex = 2;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        
        private void table1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           // this.tabControl.SelectedIndex = 0;

            //string exeFileFullPath = System.Windows.Forms.Application.ExecutablePath;
            //exeFileFullPath = exeFileFullPath.Substring(0, exeFileFullPath.LastIndexOf("\\") + 1);
            //exeFileFullPath += "Images\\index.jpg";
            
            //ImageBrush imageBrush = new ImageBrush();
            //imageBrush.ImageSource = new BitmapImage(new Uri(exeFileFullPath, UriKind.Absolute));
            //imageBrush.Stretch = Stretch.Fill;//设置图像的显示格式
            //imageBrush.TileMode = TileMode.None;

            //this.window.Background = imageBrush;
        }

        private void table2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.tabControl.SelectedIndex = 1;

            //string exeFileFullPath = System.Windows.Forms.Application.ExecutablePath;
            //exeFileFullPath = exeFileFullPath.Substring(0, exeFileFullPath.LastIndexOf("\\") + 1);
            //exeFileFullPath += "Images\\2-02.jpg";

            //ImageBrush imageBrush = new ImageBrush();
            //imageBrush.ImageSource = new BitmapImage(new Uri(exeFileFullPath, UriKind.Absolute));
            //imageBrush.Stretch = Stretch.Fill;//设置图像的显示格式
            //imageBrush.TileMode = TileMode.None;

            //this.window.Background = imageBrush;
        }

        private void table3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.tabControl.SelectedIndex = 2;
            //string exeFileFullPath = System.Windows.Forms.Application.ExecutablePath;
            //exeFileFullPath = exeFileFullPath.Substring(0, exeFileFullPath.LastIndexOf("\\") + 1);
            //exeFileFullPath += "Images\\3-02.jpg";

            //ImageBrush imageBrush = new ImageBrush();
            //imageBrush.ImageSource = new BitmapImage(new Uri(exeFileFullPath, UriKind.Absolute));
            //imageBrush.Stretch = Stretch.Fill;//设置图像的显示格式
            //imageBrush.TileMode = TileMode.None;

            //this.window.Background = imageBrush;
        }

        private void x_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ___Click(object sender, RoutedEventArgs e)
        {
            //this.WindowState = WindowState.Minimized;

            QPan_MiniMizedToTuoPan();
            this.WindowState = WindowState.Minimized;
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
           
        }

        public void QPan_OpenFromTuoPan()
        {
            this.Visibility = Visibility.Visible;

            this.ShowInTaskbar = true;

            this.WindowState = WindowState.Normal;

          
        }

        public void IconShow()
        {

        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Grid)sender).Background = new SolidColorBrush { Color = Color.FromArgb(120, 255, 255, 255) };
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((Grid)sender).Background = new SolidColorBrush { Color = Color.FromArgb(0, 255, 255, 255) };
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            //nico.Visibility=Visibility.Collapsed;
            //退出程序
            System.Windows.Application.Current.Shutdown();
            
        }

        private void T0_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Grid img=((Grid)sender);
           
            TT.Source = new BitmapImage(new Uri("\\Images\\"+img.Tag.ToString ()+".jpg", UriKind.Relative));
            if (img.Tag.ToString ().Trim ()=="0")
            {
               this.screenService.Show(ScreenType.Call);
               

               return;
            }
            if (img.Tag.ToString().Trim() == "1")
            {
                this.screenService.Show(ScreenType.History);
                return;
            }
            if (img.Tag.ToString().Trim() == "2")
            {
                this.screenService.Show(ScreenType.PersonalCenter);
                return;
            }
           
        }

        private void TextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UserCz.Background =new SolidColorBrush { Color = Color.FromArgb(0, 255, 255, 255) };
        }

        private void TextBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UserCz.Background = new SolidColorBrush { Color = Color.FromArgb(0, 255, 255, 255) };
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            new CZ().Show();
        }

        private void window_StateChanged_1(object sender, EventArgs e)
        {
            //nico.Visibility = Visibility.Visible;
            if (this.WindowState==WindowState.Minimized)
            {                
                this.WindowState = WindowState.Normal;

                this.Visibility = Visibility.Hidden;

            }
        }

        private void nico_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            Show();

        }



        private void MenuItem_Click_1(object sender, EventArgs e)
        {
            Show();
        }



        private void OnMenuItemAboutClick(object sender, EventArgs e)
        {
            Common.IsLoginOut = true;
            new Login().Show();
            this.Close();
            
        }

        private void OnMenuItemExitClick(object sender, EventArgs e)
        {
          //nico.Visibility = Visibility.Collapsed;
          System.Windows.Application.Current.Shutdown();
        }

        private void OnMenuItemOpenClick(object sender, EventArgs e)
        {
            Show();
        }

        private void OnNotificationAreaIconDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Show();
        }
    }
}