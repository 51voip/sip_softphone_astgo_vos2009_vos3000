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
using BogheCore.Model;
using System.Collections.ObjectModel;
using BogheApp.Items;
using BogheCore.Services;
using BogheApp.Services.Impl;
using BogheCore;
using BogheCore.Sip;
using BogheCore.Utils;
using System.ComponentModel;
using System.Collections;
using BogheXdm;
using System.Globalization;
using log4net;
using System.Windows.Forms;

namespace BogheApp.Screens
{
    /// <summary>
    /// Interaction logic for ScreenContacts.xaml
    /// </summary>
    public partial class ScreenContacts : BaseScreen
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(ScreenContacts));

        private ICollectionView contactsView;
        private MyObservableCollection<Contact> dataSource;

        private readonly IContactService contactService;
        private readonly IHistoryService historyService;
        private readonly ISipService sipService;

        public ScreenContacts():base()
        {
            InitializeComponent();

            this.contactService = Win32ServiceManager.SharedManager.ContactService;
            this.sipService = Win32ServiceManager.SharedManager.SipService;
            this.historyService = Win32ServiceManager.SharedManager.HistoryService;

            // important to do it here before setting ItemsSource
            this.contactService.onContactEvent += this.contactService_onContactEvent;
            this.sipService.onSubscriptionEvent += this.sipService_onSubscriptionEvent;

            this.UpdateSource();
        }

        private void buttonAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //ScreenContactEdit screenEditContact = new ScreenContactEdit(null, null);
            //this.screenService.Show(screenEditContact);
        }

        private void BaseScreen_Loaded(object sender, RoutedEventArgs e)
        {
        }

        //private void menuItemStartChat_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!String.IsNullOrEmpty(this.textBoxFreeContact.Text))
        //    {
        //        String remoteUri = UriUtils.GetValidSipUri(this.textBoxFreeContact.Text);
        //        if (!String.IsNullOrEmpty(remoteUri))
        //        {
        //            MediaActionHanler.StartChat(remoteUri);
        //        }
        //    }
        //}

        //private void menuItemSendSMS_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!String.IsNullOrEmpty(this.textBoxFreeContact.Text))
        //    {
        //        String remoteUri = UriUtils.GetValidSipUri(this.textBoxFreeContact.Text);
        //        if (!String.IsNullOrEmpty(remoteUri))
        //        {
        //            MediaActionHanler.SendSMS(remoteUri);
        //        }
        //    }
        //}

        private void textBoxSearchCriteria_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.contactsView != null)
            {
                this.contactsView.Refresh();
            }
        }

        private void imageSearchCriteriaClear_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.textBoxSearchCriteria.Text = String.Empty;
        }

        //private void imageFreeTextClear_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    this.textBoxFreeContact.Text = String.Empty;
        //}

        private void comboBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.contactsView != null)
            {
                this.contactsView.Refresh();
            }
        }

        //private void buttonVoice_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!String.IsNullOrEmpty(this.textBoxFreeContact.Text))
        //    {
        //        // 根据服务器规则，拨叫的号码前，自动添加0
        //        string strTel = "tel:0";
        //        strTel += this.textBoxFreeContact.Text;
        //        String remoteUri = UriUtils.GetValidSipUri(strTel);

        //        //String remoteUri = "sip:4113688615721@1002@115.239.227.121";

        //        if (!String.IsNullOrEmpty(remoteUri))
        //        {
        //            MediaActionHanler.MakeAudioCall(remoteUri);
        //            this.textBoxFreeContact.Text = String.Empty;
        //        }
        //    }
        //    else
        //    {
        //        HistoryEvent @event = this.historyService.Events.FirstOrDefault((x) => { return (x.MediaType & MediaType.Audio) != MediaType.None; });
        //        if (@event != null && !String.IsNullOrEmpty(@event.RemoteParty))
        //        {
        //            this.textBoxFreeContact.Text = UriUtils.GetUserName(@event.RemoteParty);
        //        }
        //    }
        //}

        //private void buttonVisio_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!String.IsNullOrEmpty(this.textBoxFreeContact.Text))
        //    {
        //        String remoteUri = UriUtils.GetValidSipUri(this.textBoxFreeContact.Text);
        //        if (!String.IsNullOrEmpty(remoteUri))
        //        {
        //            MediaActionHanler.MakeVideoCall(remoteUri);
        //            this.textBoxFreeContact.Text = String.Empty;
        //        }
        //    }
        //    else
        //    {
        //        HistoryEvent @event = this.historyService.Events.FirstOrDefault((x) => { return (x.MediaType & MediaType.AudioVideo) != MediaType.None; });
        //        if (@event != null && !String.IsNullOrEmpty(@event.RemoteParty))
        //        {
        //            this.textBoxFreeContact.Text = UriUtils.GetUserName(@event.RemoteParty);
        //        }
        //    }
        //}

        //private void buttonFile_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!String.IsNullOrEmpty(this.textBoxFreeContact.Text))
        //    {
        //        String remoteUri = UriUtils.GetValidSipUri(this.textBoxFreeContact.Text);
        //        if (!String.IsNullOrEmpty(remoteUri))
        //        {
        //            MediaActionHanler.SendFile(remoteUri, null);
        //            this.textBoxFreeContact.Text = String.Empty;
        //        }
        //    }
        //    else
        //    {
        //        HistoryEvent @event = this.historyService.Events.FirstOrDefault((x) => { return (x.MediaType & MediaType.FileTransfer) != MediaType.None; });
        //        if (@event != null && !String.IsNullOrEmpty(@event.RemoteParty))
        //        {
        //            this.textBoxFreeContact.Text = UriUtils.GetUserName(@event.RemoteParty);
        //        }
        //    }
        //}

        //private void buttonMessaging_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!String.IsNullOrEmpty(this.textBoxFreeContact.Text))
        //    {
        //        String remoteUri = UriUtils.GetValidSipUri(this.textBoxFreeContact.Text);
        //        if (!String.IsNullOrEmpty(remoteUri))
        //        {
        //            MediaActionHanler.StartChat(remoteUri);
        //            this.textBoxFreeContact.Text = String.Empty;
        //        }
        //    }
        //    else
        //    {
        //        HistoryEvent @event = this.historyService.Events.FirstOrDefault((x) => { return (x.MediaType & MediaType.Messaging) != MediaType.None; });
        //        if (@event != null && !String.IsNullOrEmpty(@event.RemoteParty))
        //        {
        //            this.textBoxFreeContact.Text = UriUtils.GetUserName(@event.RemoteParty);
        //        }
        //    }
        //}

        #region ContactsSorter

        class ContactsSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                Contact contact1 = x as Contact;
                Contact contact2 = y as Contact;
                if (contact1 != null && contact2 != null)
                {
                    if (contact2.Authorization == contact1.Authorization)
                    {
                        return contact1.CompareTo(contact2);
                    }
                    else
                    {
                        return (contact1.Authorization - contact2.Authorization);
                    }
                }
                return 0;
            }
        }

        #endregion

        #region FilterItem

        class FilterItem
        {
            readonly String name;
            readonly String displayName;
            readonly String imageSource;

            internal FilterItem(String name, String displayName, String imageSource)
            {
                this.name = name;
                this.displayName = displayName;
                this.imageSource = imageSource;
            }

            public String Name
            {
                get { return this.name; }
            }

            public String DisplayName
            {
                get { return this.displayName; }
            }

            public String ImageSource
            {
                get { return this.imageSource; }
            }

            public static String ImageSourceFromAuthorization(Authorization auth)
            {
                switch (auth)
                {
                    case Authorization.Allowed:
                        return "/BogheApp;component/embedded/16/dialog-accept_16.png";
                    case Authorization.Blocked:
                    case Authorization.PoliteBlocked:
                        return "/BogheApp;component/embedded/16/dialog-block_16.png";
                    case Authorization.Revoked:
                        return "/BogheApp;component/embedded/16/dialog-close-2_16.png";
                    case Authorization.All:
                        return "/BogheApp;component/embedded/16/family_16.png";
                    default:
                        return "/BogheApp;component/embedded/16/dialog-question-2_16.png";
                }
            }

        }

        #endregion
    }
}
