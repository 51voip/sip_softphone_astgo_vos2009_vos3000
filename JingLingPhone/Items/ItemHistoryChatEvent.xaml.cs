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
using System.Globalization;
using BogheCore.Utils;
using BogheApp.Screens;

namespace BogheApp.Items
{
    /// <summary>
    /// Interaction logic for ItemHistoryChatEvent.xaml
    /// </summary>
    public partial class ItemHistoryChatEvent : BaseItem<HistoryChatEvent>
    {
        private readonly IHistoryService historyService;
        private HistoryChatEvent @event;

        public ItemHistoryChatEvent()
        {
            InitializeComponent();

            this.historyService = Win32ServiceManager.SharedManager.HistoryService;

            this.ValueLoaded += this.ItemHistoryChatEvent_ValueLoaded;
        }

        private void ItemHistoryChatEvent_ValueLoaded(object sender, EventArgs e)
        {
            this.@event = this.Value;

            this.labelDisplayName.Content = this.@event.DisplayName;
            this.labelDate.Content = BaseItem<HistoryChatEvent>.GetFriendlyDateString(this.@event.Date);
            this.ctxMenu_AddToContacts.IsEnabled = (this.@event.Contact == null);

            this.textBockMessage.Text = String.Empty;
            if (@event.Messages.Count > 0)
            {
                HistoryShortMessageEvent shortMessage = @event.Messages[0];
                this.textBockMessage.Text = shortMessage.Content ?? shortMessage.Content;
            }

            this.Width = Double.NaN;
        }

        private void ctxMenu_MakeVoiceCall_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.MakeAudioCall(this.Value.RemoteParty);
        }

        private void ctxMenu_MakeVideoCall_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.MakeVideoCall(this.Value.RemoteParty);
        }

        private void ctxMenu_SendFile_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.SendFile(this.Value.RemoteParty, null);
        }

        private void ctxMenu_StartChat_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.StartChat(this.Value.RemoteParty);
        }

        private void ctxMenu_SendSMS_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.SendSMS(this.Value.RemoteParty);
        }

        private void ctxMenu_AddToContacts_Click(object sender, RoutedEventArgs e)
        {
            Contact contact = new Contact();
            contact.UriString = this.@event.RemoteParty;
            contact.DisplayName = UriUtils.GetUserName(contact.UriString);
            ScreenContactEdit screenEditContact = new ScreenContactEdit(contact, null);
            screenEditContact.EditMode = false;
            screenEditContact.Tag = this.@event;
            Win32ServiceManager.SharedManager.Win32ScreenService.Show(screenEditContact);
        }

        private void ctxMenu_DeleteHistoryEvent_Click(object sender, RoutedEventArgs e)
        {
            this.historyService.DeleteEvent(this.Value);
        }
    }
}
