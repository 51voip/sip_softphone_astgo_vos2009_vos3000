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
using BogheCore.Services;
using BogheApp.Services.Impl;
using BogheApp.Services;
using BogheApp.embedded;

namespace BogheApp.Screens
{
    /// <summary>
    /// Interaction logic for ScreenContactEdit.xaml
    /// </summary>
    public partial class ScreenContactEdit : BaseScreen
    {
        private Contact contact;
        private bool editMode;

        private readonly IContactService contactService;
        private readonly IHistoryService historyService;
        private readonly IWin32ScreenService screenService;

        public ScreenContactEdit(Contact contact, Group group) : base()
        {
            InitializeComponent();

            if ((this.contact = contact) != null)
            {
                this.textBoxSipUri.Text = this.contact.UriString ?? this.contact.UriString;
                this.textBoxDisplayName.Text = this.contact.DisplayName ?? this.contact.DisplayName;
                this.textBoxSipUri.IsEnabled = false;
                this.editMode = true;
                this.labelTitle.Content = Strings.Text_EditContact;
            }
            else
            {
                String realm = Win32ServiceManager.SharedManager.ConfigurationService.Get(Configuration.ConfFolder.NETWORK, 
                    Configuration.ConfEntry.REALM, Configuration.DEFAULT_NETWORK_REALM);
                this.contact = new Contact();
                if (group != null)
                {
                    this.contact.GroupName = group.Name;
                }
                //this.textBoxSipUri.Text = this.contact.UriString = String.Format("sip:johndoe@{0}", realm.Replace("sip:", String.Empty));
                //this.textBoxDisplayName.Text = this.contact.DisplayName = " ";
                this.textBoxSipUri.IsEnabled = true;
                this.editMode = false;
                this.labelTitle.Content = Strings.Text_AddContact;
            }

            this.contactService = Win32ServiceManager.SharedManager.ContactService;
            this.historyService = Win32ServiceManager.SharedManager.HistoryService;
            this.screenService = Win32ServiceManager.SharedManager.Win32ScreenService;
        }

        public bool EditMode
        {
            get { return this.editMode; }
            set
            {
                this.editMode = value;
                this.labelTitle.Content = this.editMode ? Strings.Text_EditContact : Strings.Text_AddContact;
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.comboBoxGroup.SelectedIndex == -1)
            {
                MessageBox.Show("你必须选择一个分组", "无效分组");
                return;
            }

            if (this.contact == null)
            {
                this.contact = new Contact();
            }

            Group group = this.comboBoxGroup.SelectedItem as Group;
            

            this.contact.DisplayName = this.textBoxDisplayName.Text;

            // 根据服务器特性，增加联系电话时候，自动添加前缀  tel:0
            if (this.textBoxSipUri.Text.IndexOf("tel:41") < 0)
            {
                this.textBoxSipUri.Text = "tel:41" + this.textBoxSipUri.Text;
            }

            this.contact.UriString = this.textBoxSipUri.Text;

            this.contact.GroupName = group.Name;

            if (this.editMode)
            {
                this.contactService.ContactUpdate(this.contact, group.Name);
            }
            else
            {
                if (this.Tag == null)
                {
                    this.Tag = this.contact;
                }
                this.contactService.ContactAdd(this.contact);
            }
            this.screenService.Show(ScreenType.Contacts);
            this.screenService.Hide(this);
            if (this.Tag != null)
            {
                List<HistoryEvent> events = null;
                if (this.Tag is HistoryEvent)
                {
                    HistoryEvent @event = (this.Tag as HistoryEvent);
                    events = this.historyService.Events.FindAll((x) => { return x.RemoteParty.Equals(@event.RemoteParty); });
                }
                else if (this.Tag is Contact)
                {
                    Contact contact = (this.Tag as Contact);
                    events = this.historyService.Events.FindAll((x) => { return x.RemoteParty.Equals(contact.UriString); });
                }
                if (events != null && events.Count > 0)
                {
                    events.ForEach((x) => x.DisplayName = null);
                    this.screenService.ScreenHistory.Refresh();
                }
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.screenService.Show(ScreenType.Contacts);
            this.screenService.Hide(this);
        }
    }
}
