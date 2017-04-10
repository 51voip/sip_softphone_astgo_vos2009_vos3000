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
using BogheCore.Services;
using BogheApp.Services.Impl;
using log4net;
using BogheCore.Utils;
using BogheApp.Screens;

namespace BogheApp.Items
{
    /// <summary>
    /// Interaction logic for ItemHistoryFileTransferEvent.xaml
    /// </summary>
    public partial class ItemHistoryFileTransferEvent : BaseItem<HistoryFileTransferEvent>
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(ItemHistoryFileTransferEvent));

        private HistoryFileTransferEvent @event;
        private System.IO.FileInfo fileInfo;

        private readonly IHistoryService historyService;

        public ItemHistoryFileTransferEvent()
        {
            InitializeComponent();

            this.historyService = Win32ServiceManager.SharedManager.HistoryService;

            this.ValueLoaded += this.ItemHistoryFileTransferEvent_ValueLoaded;
        }

        private void ItemHistoryFileTransferEvent_ValueLoaded(object sender, EventArgs e)
        {
            this.@event = this.Value;

            this.fileInfo = String.IsNullOrEmpty(this.@event.FilePath) ? null : new System.IO.FileInfo(this.@event.FilePath);
            this.labelInfo.Content = String.Format("{0} - {1}", this.@event.DisplayName, fileInfo != null ? fileInfo.Name : "-");
            this.labelDate.Content = BaseItem<HistoryFileTransferEvent>.GetFriendlyDateString(this.@event.Date);
            this.ctxMenu_AddToContacts.IsEnabled = (this.@event.Contact == null);

            switch (this.@event.Status)
            {
                case HistoryEvent.StatusType.Failed:
                default:
                    {
                        this.imageStatus.Source = MyImageConverter.FromBitmap(Properties.Resources.document_forbidden_16);
                        break;
                    }

                case HistoryEvent.StatusType.Incoming:
                    {
                        this.imageStatus.Source = MyImageConverter.FromBitmap(Properties.Resources.document_down_16);
                        break;
                    }

                case HistoryEvent.StatusType.Outgoing:
                    {
                        this.imageStatus.Source = MyImageConverter.FromBitmap(Properties.Resources.document_up_16);
                        break;
                    }
            }

            if (this.fileInfo == null || !this.fileInfo.Exists)
            {
                this.imageIcon.Source = MyImageConverter.FromBitmap(Properties.Resources.document_forbidden_32);
                this.ctxMenu_OpenFile.IsEnabled = false;
            }
            else
            {
                using (System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(this.fileInfo.FullName))
                {
                    this.imageIcon.Source = MyImageConverter.FromIcon(icon);
                }
                this.ctxMenu_OpenFile.IsEnabled = true;
            }

            this.Width = Double.NaN;
        }

        private void ctxMenu_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(this.fileInfo.FullName);
            }
            catch (System.ComponentModel.Win32Exception w32ex)
            {
                if (w32ex.ErrorCode == -2147467259)
                {
                    try
                    {
                        System.Diagnostics.ProcessStartInfo pInfo = new System.Diagnostics.ProcessStartInfo("rundll32.exe");
                        pInfo.Arguments = String.Format("shell32.dll, OpenAs_RunDLL {0}", this.fileInfo.FullName);
                        System.Diagnostics.Process.Start(pInfo);
                    }
                    catch (Exception ex)
                    {
                        LOG.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }

        private void ctxMenu_MakeVoiceCall_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.MakeAudioCall(this.@event.RemoteParty);
        }

        private void ctxMenu_MakeVideoCall_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.MakeVideoCall(this.@event.RemoteParty);
        }

        private void ctxMenu_SendFile_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.SendFile(this.@event.RemoteParty, null);
        }

        private void ctxMenu_StartChat_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.StartChat(this.@event.RemoteParty);
        }

        private void ctxMenu_SendSMS_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.SendSMS(this.@event.RemoteParty);
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
            this.historyService.DeleteEvent(this.@event);
        }
    }
}
