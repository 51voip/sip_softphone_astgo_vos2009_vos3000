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
using BogheApp.Services.Impl;
using BogheCore.Services;
using BogheCore.Xcap.Events;
using log4net;
using BogheControls.Utils;
using BogheCore.Events;

namespace BogheApp.Screens
{
    /// <summary>
    /// Interaction logic for ScreenOptions.xaml
    /// </summary>
    public partial class ScreenOptions : BaseScreen
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(ScreenOptions));

        public event EventHandler<StringEventArgs> onAvatarChanded;

        private readonly IConfigurationService configurationService;
        private readonly IXcapService xcapService;
        private readonly ISipService sipService;

        private bool presenceChanged;

        public ScreenOptions()
        {
            InitializeComponent();

            this.configurationService = Win32ServiceManager.SharedManager.ConfigurationService;
            this.xcapService = Win32ServiceManager.SharedManager.XcapService;
            this.sipService = Win32ServiceManager.SharedManager.SipService;

            this.xcapService.onXcapEvent += this.xcapService_onXcapEvent;

            //
            //  Contacts
            //
            this.radioButtonContactsRemote.Checked += (sender, e) => this.groupBoxXCAP.IsEnabled = true;
            this.radioButtonContactsRemote.Unchecked += (sender, e) => this.groupBoxXCAP.IsEnabled = false;

            //
            //  Messaging
            //
            this.radioButtonMessagingSMSBinary.Checked += (sender, e) => this.textBoxMessagingPSI.IsEnabled = true;
            this.radioButtonMessagingSMSBinary.Unchecked += (sender, e) => this.textBoxMessagingPSI.IsEnabled = false;


            //
            //  Presence
            //
            this.checkBoxPresenceSubscribe.Checked += (sender, e) =>
                {
                    this.checkBoxPresenceRLS.IsEnabled = true;
                };
            this.checkBoxPresenceSubscribe.Unchecked += (sender, e) =>
            {
                this.checkBoxPresenceRLS.IsEnabled = false;
            };
            
            this.checkBoxPresencePublish.Checked += (sender, e) =>
            {
                this.textBoxPresenceFreeText.IsEnabled = true;
                this.textBoxPresenceHomePage.IsEnabled = true;
                this.textBoxHyperAvailabilityTimeout.IsEnabled = true;
                this.borderAvatar.IsEnabled = true;
            };
            this.checkBoxPresencePublish.Unchecked += (sender, e) =>
            {
                this.textBoxPresenceFreeText.IsEnabled = false;
                this.textBoxPresenceHomePage.IsEnabled = false;
                this.textBoxHyperAvailabilityTimeout.IsEnabled = false;
                this.borderAvatar.IsEnabled = false;
            };

            this.textBoxPresenceFreeText.TextChanged += (sender, e) => this.presenceChanged = true;
            this.textBoxPresenceHomePage.TextChanged += (sender, e) => this.presenceChanged = true;

            //
            //  NATT
            //
            this.checkBoxStunTurnEnable.Checked += (sender, e) =>
            {
                this.radioButtonStunDiscover.IsEnabled = true;
                this.radioButtonStunUseThis.IsEnabled = true;
                this.textBoxStunServerAddress.IsEnabled = true & this.radioButtonStunUseThis.IsChecked.Value;
                this.textBoxStunPort.IsEnabled = true & this.radioButtonStunUseThis.IsChecked.Value;
            };
            this.checkBoxStunTurnEnable.Unchecked += (sender, e) =>
            {
                this.radioButtonStunDiscover.IsEnabled = false;
                this.radioButtonStunUseThis.IsEnabled = false;
                this.textBoxStunServerAddress.IsEnabled = false;
                this.textBoxStunPort.IsEnabled = false;
            };
            this.radioButtonStunUseThis.Checked += (sender, e) =>
            {
                this.textBoxStunServerAddress.IsEnabled = true & this.checkBoxStunTurnEnable.IsChecked.Value;
                this.textBoxStunPort.IsEnabled = true & this.checkBoxStunTurnEnable.IsChecked.Value;
            };
            this.radioButtonStunUseThis.Unchecked += (sender, e) =>
            {
                this.textBoxStunServerAddress.IsEnabled = false;
                this.textBoxStunPort.IsEnabled = false;
            };


            //
            //  Security
            //
            this.checkBoxIPSecSecAgreeEnabled.Unchecked += (sender, e) =>
            {
                this.comboBoxIPSecAlgorithm.IsEnabled = false;
                this.comboBoxIPSecEAlgorithm.IsEnabled = false;
                this.comboBoxIPSecMode.IsEnabled = false;
                this.comboBoxIPSecProtocol.IsEnabled = false;
            };
            this.checkBoxIPSecSecAgreeEnabled.Checked += (sender, e) =>
            {
                this.comboBoxIPSecAlgorithm.IsEnabled = true;
                this.comboBoxIPSecEAlgorithm.IsEnabled = true;
                this.comboBoxIPSecMode.IsEnabled = true;
                this.comboBoxIPSecProtocol.IsEnabled = true;
            };

            //
            //  QoS
            //
            this.checkBoxSessionTimersEnable.Checked += (sender, e) =>
            {
                this.comboBoxSessionTimerRefreser.IsEnabled = true;
                this.textBoxSessionTimersTimeout.IsEnabled = true;
            };
            this.checkBoxSessionTimersEnable.Unchecked += (sender, e) =>
            {
                this.comboBoxSessionTimerRefreser.IsEnabled = false;
                this.textBoxSessionTimersTimeout.IsEnabled = false;
            };

            this.InitializeGeneral();
            this.Initializeidentity();
            this.InitializeCodecs();
            this.InitializeQoS();
            this.InitializeSecurity();

            this.LoadConfiguration();
        }

        public void SelectPresenceTab()
        {
            this.tabControl.SelectedItem = this.tabItemPresence;
        }

        private void SaveConfiguration()
        {
            try
            {
                if (!this.UpdateGeneral())
                    return;

                if (!this.UpdateIdentity())
                    return;

                if (!this.UpdateNetwork())
                    return;

                if (!this.UpdateContacts())
                    return;

                if (!this.UpdatePresence())
                    return;

                if (!this.UpdateCodecs())
                    return;

                if (!this.UpdateMessaging())
                    return;

                if (!this.UpdateNATT())
                    return;

                if (!this.UpdateSecurity())
                    return;

                if (!this.UpdateQoS())
                    return;

                if (this.presenceChanged && this.sipService.IsPublicationEnabled)
                {
                    this.sipService.PresencePublish();
                }


                this.presenceChanged = false;
            }
            catch (Exception e)
            {
                LOG.Error(e);
            }
        }

        private void LoadConfiguration()
        {
            try
            {
                this.LoadGeneral();
                this.LoadIdentity();
                this.LoadNetwork();
                this.LoadContacts();
                this.LoadPresence();
                this.LoadCodecs();
                this.LoadMessaging();
                this.LoadNATT();
                this.LoadSecurity();
                this.LoadQoS();
            }
            catch (Exception e)
            {
                LOG.Error(e);
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            this.SaveConfiguration();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.LoadConfiguration();
        }

        private void xcapService_onXcapEvent(object sender, BogheCore.Xcap.Events.XcapEventArgs e)
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
                        object content = e.GetExtra(XcapEventArgs.EXTRA_CONTENT);
                        if (content != null && content is String)
                        {
                            this.SetAvatarFromBase64String(content as String);
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        private void avatar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Multiselect = false;
            Nullable<bool> result = fileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                if (this.sipService.IsRegistered && this.sipService.IsXcapEnabled)
                {
                    LOG.Debug("Saving avatar(remote)...");
                    String mimeType = null;
                    String avatarB64 = this.GetAvatarBase64FromFilePath(fileDialog.FileName, out mimeType);
                    if (!String.IsNullOrEmpty(avatarB64))
                    {
                        // ImageSource will be updated when xcapcallback is received
                        this.xcapService.AvatarPublish(avatarB64, mimeType);
                    }
                }
                else
                {
                    LOG.Debug("Saving avatar(local)...");
                    System.Drawing.Imaging.ImageFormat rawFormat;
                    System.Drawing.Image avatar = this.GetAvatarFromFilePath(fileDialog.FileName, out rawFormat);
                    if (avatar != null)
                    {
                        try
                        {
                            avatar.Save(MainWindow.AVATAR_PATH);
                            this.imageAvatar.Source = MyImageConverter.FromImage(avatar);
                            EventHandlerTrigger.TriggerEvent(this.onAvatarChanded, this, new StringEventArgs(MainWindow.AVATAR_PATH));
                        }
                        catch (Exception ex)
                        {
                            LOG.Error(ex);
                        }
                    }
                }
            }
        }

        
    }
}
