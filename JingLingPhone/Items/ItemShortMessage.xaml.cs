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
using BogheControls.Utils;
using System.Globalization;
using BogheCore.Utils;
using BogheCore.Services;
using BogheApp.Services.Impl;

namespace BogheApp.Items
{
    /// <summary>
    /// Interaction logic for ItemShortMessage.xaml
    /// </summary>
    public partial class ItemShortMessage : BaseItem<HistoryShortMessageEvent>
    {
        private readonly IHistoryService historyService;

        public ItemShortMessage()
        {
            InitializeComponent();

            this.historyService = Win32ServiceManager.SharedManager.HistoryService;

            this.ValueLoaded += this.ItemShortMessage_ValueLoaded;
        }

        private void ItemShortMessage_ValueLoaded(object sender, EventArgs e)
        {
            HistoryShortMessageEvent messageEventValue = this.Value;
            if (messageEventValue == null)
            {
                return;
            }

            switch (messageEventValue.Status)
            {
                case HistoryEvent.StatusType.Incoming:
                    this.labelDescription.Content = String.Format("{0} says", UriUtils.GetDisplayName(messageEventValue.RemoteParty));
                    this.gradientStop.Color = Colors.Bisque;
                    break;

                case HistoryEvent.StatusType.Outgoing:
                    this.labelDescription.Content = "I say";
                    this.gradientStop.Color = Colors.SkyBlue;
                    break;

                case HistoryEvent.StatusType.Failed:
                default:
                    this.gradientStop.Color = Colors.Red;
                    break;
            }

            this.labelDate.Content = BaseItem<HistoryShortMessageEvent>.GetFriendlyDateString(messageEventValue.Date);
            this.labelContent.Content = messageEventValue.Content;
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

        }

        private void ctxMenu_StartChat_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.StartChat(this.Value.RemoteParty);
        }

        private void ctxMenu_SendSMS_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ctxMenu_AddToContacts_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ctxMenu_DeleteHistoryEvent_Click(object sender, RoutedEventArgs e)
        {
            this.historyService.DeleteEvent(this.Value);
        }
    }
}
