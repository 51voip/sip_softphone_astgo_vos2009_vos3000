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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BogheControls;
using BogheCore.Services;
using BogheApp.Services.Impl;
using BogheCore.Model;
using BogheCore.Events;
using BogheCore.Sip.Events;
using System.Threading;
using System.Xml;
using System.IO;
using System.Windows.Threading;

namespace BogheApp.Screens
{
    /// <summary>
    /// Interaction logic for ScreenAuthentication.xaml
    /// </summary>
    public partial class ScreenAuthentication : BaseScreen
    {
        static public string SIP_SERVER_ADDRESS = "115.239.227.121";
        private string CONFIG_NAME = "User.xml";
        private readonly IConfigurationService configurationService;
        private readonly ISipService sipService;
        private DispatcherTimer timer;

        public ScreenAuthentication()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer1_Tick;
            timer.Start();

            this.configurationService = Win32ServiceManager.SharedManager.ConfigurationService;
            this.sipService = Win32ServiceManager.SharedManager.SipService;

            string strName = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.DISPLAY_NAME, Configuration.DEFAULT_IDENTITY_DISPLAY_NAME);
            
            if (strName != "John Doe")
            {
                this.textBoxDisplayName.Text = strName;
            }

            this.textBoxPublicIdentity.Text = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.IMPU, Configuration.DEFAULT_IDENTITY_IMPU);
            this.textBoxPrivateIdentity.Text = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.IMPI, Configuration.DEFAULT_IDENTITY_IMPI);
            
            // 如果需要记住用户名，此处就需要赋值，否则不需要，暂时注释掉
            this.passwordBox.Password = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.PASSWORD, String.Empty);

            this.textBoxRealm.Text = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.REALM, Configuration.DEFAULT_NETWORK_REALM);
            this.checkBoxEarlyIMS.IsChecked = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.EARLY_IMS, Configuration.DEFAULT_NETWORK_EARLY_IMS);

            this.configurationService.onConfigurationEvent += this.configurationService_onConfigurationEvent;
            this.sipService.onRegistrationEvent += this.sipService_onRegistrationEvent;

            buttonSignIn_Click(null, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.progressBar1.Value++;
        }

        private void buttonSignIn_Click(object sender, RoutedEventArgs e)
        {
            //XmlDocument XMLDocFriend = new XmlDocument();
            //string exeFileFullPath = System.Windows.Forms.Application.ExecutablePath;
            //exeFileFullPath = exeFileFullPath.Substring(0, exeFileFullPath.LastIndexOf("\\") + 1);

            //// 读取User.xml文件
            //string XMLFileFullPath = exeFileFullPath + CONFIG_NAME;

            //XMLDocFriend.Load(XMLFileFullPath);
            //XmlNodeList notes = XMLDocFriend.DocumentElement.ChildNodes;
            //XmlElement user = (XmlElement)notes[0];

            //string dispName = user.GetAttribute("name");
            //string passwordBox = user.GetAttribute("password");

            this.textBoxDisplayName.Text = Common.displayName;
            this.passwordBox.Password = Common.Password;

            if (this.sipService.IsRegistered)
            {
                this.sipService.UnRegister();
            }
            else 
            {
                this.UpdatePreferences();
                this.sipService.Register();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.sipService.StopStack();
        }

        private void UpdatePreferences()
        {
            this.configurationService.onConfigurationEvent -= this.configurationService_onConfigurationEvent;

            // 根据界面设置的用户名和密码，把登录必要的信息写到配置文件
            string dispName = this.textBoxDisplayName.Text;

            string publcIdentity = "sip:";
            publcIdentity += dispName;
            publcIdentity += "@";
            publcIdentity += SIP_SERVER_ADDRESS;

            string privateIdentity = dispName;
            privateIdentity += "@";
            privateIdentity += SIP_SERVER_ADDRESS;

            string passwordBox = this.passwordBox.Password;
            string realm = publcIdentity;

            this.configurationService.Set(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.DISPLAY_NAME, dispName);
            this.configurationService.Set(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.IMPU, publcIdentity);
            this.configurationService.Set(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.IMPI, privateIdentity);
            this.configurationService.Set(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.PASSWORD, passwordBox);

            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.REALM, realm);
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_HOST, SIP_SERVER_ADDRESS);
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_PORT, "5060");

            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.EARLY_IMS, this.checkBoxEarlyIMS.IsChecked.HasValue ? this.checkBoxEarlyIMS.IsChecked.Value : Configuration.DEFAULT_NETWORK_EARLY_IMS);

            this.configurationService.onConfigurationEvent += this.configurationService_onConfigurationEvent;
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
                    switch (e.Entry)
                    {
                        case Configuration.ConfEntry.DISPLAY_NAME:
                            this.textBoxDisplayName.Text = e.Value.ToString();
                            break;
                        case Configuration.ConfEntry.IMPI:
                            this.textBoxPrivateIdentity.Text = e.Value.ToString();
                            break;
                        case Configuration.ConfEntry.IMPU:
                            this.textBoxPublicIdentity.Text = e.Value.ToString();
                            break;
                        case Configuration.ConfEntry.PASSWORD:
                            this.passwordBox.Password = e.Value.ToString();
                            break;
                    }
                    break;

                case Configuration.ConfFolder.NETWORK:
                    switch (e.Entry)
                    {
                        case Configuration.ConfEntry.EARLY_IMS:
                            this.checkBoxEarlyIMS.IsChecked = e.Value.Equals(Boolean.TrueString);
                            break;
                        case Configuration.ConfEntry.REALM:
                            this.textBoxRealm.Text = e.Value.ToString();
                            break;
                    }
                    break;
            }
        }

        private void sipService_onRegistrationEvent(object sender, RegistrationEventArgs e)
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
                    this.buttonCancel.IsEnabled = true;
                    this.buttonSignIn.IsEnabled = false;
                    break;

                case RegistrationEventTypes.REGISTRATION_NOK:
                case RegistrationEventTypes.REGISTRATION_OK:
                case RegistrationEventTypes.UNREGISTRATION_NOK:
                case RegistrationEventTypes.UNREGISTRATION_OK:
                    this.buttonCancel.IsEnabled = false;
                    this.buttonSignIn.IsEnabled = true;

                    this.buttonSignIn.Content = this.sipService.IsRegistered ? "Sign Out" : "Sign In";
                    break;
            }
        }
    }
}
