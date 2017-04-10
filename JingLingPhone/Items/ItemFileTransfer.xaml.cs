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
using BogheCore.Utils;
using System.Globalization;
using log4net;
using BogheCore.Sip;
using BogheCore.Sip.Events;
using System.IO;
using BogheControls.Utils;
using BogheCore.Services;
using BogheApp.Services.Impl;
using System.Diagnostics;

namespace BogheApp.Items
{
    /// <summary>
    /// Interaction logic for ItemFileTransfer.xaml
    /// </summary>
    public partial class ItemFileTransfer : BaseItem<HistoryFileTransferEvent>
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(ItemFileTransfer));

        private HistoryFileTransferEvent @event;
        private String fileName;
        private bool done;
        private readonly ISipService sipService;

        public ItemFileTransfer()
        {
            InitializeComponent();

            this.sipService = Win32ServiceManager.SharedManager.SipService;

            this.sipService.onInviteEvent += this.sipService_onInviteEvent;
            this.ValueLoaded += this.ItemFileTransfer_ValueLoaded;

            this.progressBar.Value = 0;
            this.textBlockName.Text = String.Empty;
            this.textBlockTransfered.Text = String.Empty;
            this.imageIcon.Cursor = Cursors.Wait;
        }

        private void sipService_onInviteEvent(object sender, InviteEventArgs e)
        {
            if (this.@event == null || this.@event.MsrpSession == null)
            {
                return;
            }

            if (e.SessionId != this.@event.MsrpSession.Id)
            {
                return;
            }

            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<InviteEventArgs>(this.sipService_onInviteEvent), sender, new object[] { e });
                return;
            }

            switch(e.Type)
            {
                case InviteEventTypes.CONNECTED:
                    {
                        if (this.imageIcon.Source == null && File.Exists(this.@event.MsrpSession.FilePath))
                        {
                            using (System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(this.@event.MsrpSession.FilePath))
                            {
                                this.imageIcon.Source = MyImageConverter.FromIcon(icon);
                            }
                        }
                        this.buttonAccept.Visibility = Visibility.Collapsed;
                        this.buttonDecline.Visibility = Visibility.Visible;
                        this.buttonDecline.Content = "abort";
                        break;
                    }

                case InviteEventTypes.DISCONNECTED:
                    {
                        if (!this.done)
                        {
                            this.buttonAccept.Visibility = Visibility.Collapsed;
                            this.buttonDecline.Visibility = Visibility.Collapsed;
                            this.progressBar.Visibility = Visibility.Collapsed;

                            this.labelDescription.Content = String.Format("File transfer Failed ({0})", e.Phrase);
                            this.gradientStop.Color = Colors.Red;

                            this.imageIcon.Cursor = Cursors.No;
                        }
                        else
                        {
                            this.imageIcon.Cursor = Cursors.Hand;
                            this.imageIcon.Tag = this.@event.MsrpSession.FilePath;
                        }

                        this.sipService.onInviteEvent -= this.sipService_onInviteEvent;
                        break;
                    }
                default:
                    break;
            }
        }

        private void ItemFileTransfer_ValueLoaded(object sender, EventArgs e)
        {
            this.@event = this.Value;
            if (this.@event == null)
            {
                LOG.Error("Event is null");
                return;
            }

            if (this.@event.MsrpSession == null)
            {
                LOG.Error("No MSRP session associated to this event");
                return;
            }

            this.@event.MsrpSession.mOnMsrpEvent += this.MsrpSession_onMsrpEvent;

            switch (this.@event.Status)
            {
                case HistoryEvent.StatusType.Incoming:
                    this.labelDescription.Content = String.Format("Receiving from {0}...", UriUtils.GetDisplayName(this.@event.RemoteParty));
                    this.buttonAccept.Content = "accept";
                    this.buttonDecline.Content = "decline";
                    this.gradientStop.Color = Colors.Bisque;
                    break;

                case HistoryEvent.StatusType.Outgoing:
                    this.labelDescription.Content = "Sending...";
                    this.buttonAccept.Visibility = Visibility.Collapsed;
                    this.buttonDecline.Content = "abort";
                    this.gradientStop.Color = Colors.SkyBlue;
                    break;

                case HistoryEvent.StatusType.Failed:
                default:
                    this.gradientStop.Color = Colors.Red;
                    break;
            }

            this.labelDate.Content = BaseItem<HistoryFileTransferEvent>.GetFriendlyDateString(this.@event.Date);
            this.fileName = new FileInfo(this.@event.MsrpSession.FilePath).Name;
            this.textBlockName.Text = this.fileName;
            if (File.Exists(this.@event.MsrpSession.FilePath))
            {
                using (System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(this.@event.MsrpSession.FilePath))
                {
                    this.imageIcon.Source = MyImageConverter.FromIcon(icon);
                }
            }
        }

        private void buttonDecline_Click(object sender, RoutedEventArgs e)
        {
            if (this.@event != null && this.@event.MsrpSession != null)
            {
                this.@event.MsrpSession.HangUp();
            }
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            if (this.@event != null && this.@event.MsrpSession != null)
            {
                this.@event.MsrpSession.Accept();
            }
        }

        private void MsrpSession_onMsrpEvent(object sender, BogheCore.Sip.Events.MsrpEventArgs e)
        {
            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<MsrpEventArgs>(this.MsrpSession_onMsrpEvent), sender, new object[] { e });
                return;
            }

            MyMsrpSession session = (e.GetExtra(MsrpEventArgs.EXTRA_SESSION) as MyMsrpSession);
            if (session == null || this.@event.MsrpSession == null || this.@event.MsrpSession.Id != session.Id)
            {
                LOG.Error("No matching MSRP session could be found");
                return;
            }

            switch (e.Type)
            {
                case MsrpEventTypes.CONNECTED:
                    break;

                case MsrpEventTypes.DISCONNECTED:
                    if(!this.done)
                    {
                        this.buttonAccept.Visibility = Visibility.Collapsed;
                        this.buttonDecline.Visibility = Visibility.Collapsed;
                        this.progressBar.Visibility = Visibility.Collapsed;

                        this.labelDescription.Content = "File transfer Failed";
                        this.borderHdr.Background = Brushes.Red;
                    }

                    if (this.@event != null && this.@event.MsrpSession != null)
                    {
                        this.@event.MsrpSession.mOnMsrpEvent -= this.MsrpSession_onMsrpEvent;
                    }
                    break;

                case MsrpEventTypes.ERROR:
                    break;

                case MsrpEventTypes.SUCCESS_2XX:
                case MsrpEventTypes.DATA:
                case MsrpEventTypes.SUCCESS_REPORT:
                    {
                        long? end = e.GetExtra(MsrpEventArgs.EXTRA_BYTE_RANGE_END) as long?;
                        long? total = e.GetExtra(MsrpEventArgs.EXTRA_BYTE_RANGE_TOTAL) as long?;

                        if (end.HasValue && total.HasValue && end.Value >= 0 && total.Value >= 0)
                        {
                            this.progressBar.Maximum = total.Value;
                            this.progressBar.Value = end.Value;
                            this.textBlockTransfered.Text = String.Format("Transfered: {0}", this.GetFriendlySize(end.Value));
                            this.textBlockName.Text = String.Format("{0} ({1})", this.fileName, this.GetFriendlySize(total.Value));

                            if (end.Value >= total.Value)
                            {
                                this.done = true;
                                this.buttonAccept.Visibility = Visibility.Collapsed;
                                this.buttonDecline.Visibility = Visibility.Collapsed;
                                this.progressBar.Visibility = Visibility.Collapsed;
                                switch (this.@event.Status)
                                {
                                    case HistoryEvent.StatusType.Incoming:
                                        this.labelDescription.Content = "File successfully received";
                                        break;

                                    case HistoryEvent.StatusType.Outgoing:
                                        this.labelDescription.Content = "File successfully sent";
                                        break;

                                    case HistoryEvent.StatusType.Failed:
                                    default:
                                        this.labelDescription.Content = "File transfer Failed";
                                        this.gradientStop.Color = Colors.Red;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            this.progressBar.IsIndeterminate = true;
                        }
                        
                        break;
                    }
            }
        }

        private void imageIcon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.imageIcon.Tag == null)
            {
                return;
            }

            String fullPath = new FileInfo(this.imageIcon.Tag as String).FullName;

            try
            {
                Process.Start(fullPath);
            }
            catch (System.ComponentModel.Win32Exception w32ex)
            {
                if (w32ex.ErrorCode == -2147467259)
                {
                    try
                    {
                        ProcessStartInfo pInfo = new ProcessStartInfo("rundll32.exe");
                        pInfo.Arguments = String.Format("shell32.dll, OpenAs_RunDLL {0}", fullPath);
                        Process.Start(pInfo);
                    }
                    catch(Exception ex)
                    {
                        LOG.Error(ex);
                    }
                }
            }
            catch(Exception ex)
            {
                LOG.Error(ex);
            }
        }

        const long KILO = 1024;
        const long MEGA = KILO * KILO;
        const long GIGA = MEGA * KILO;

        private String GetFriendlySize(float size)
        {
            if (size < ItemFileTransfer.KILO)
            {
                return String.Format(CultureInfo.CurrentUICulture, "{0} o", size);
            }
            else if (size < ItemFileTransfer.MEGA)
            {
                return String.Format(CultureInfo.CurrentUICulture, "{0:0.##} Ko", size / ItemFileTransfer.KILO);
            }
            else if (size < ItemFileTransfer.GIGA)
            {
                return String.Format(CultureInfo.CurrentUICulture, "{0:0.##} Mo", size / ItemFileTransfer.MEGA);
            }
            else
            {
                return String.Format(CultureInfo.CurrentUICulture, "{0:0.##} Go", size / ItemFileTransfer.GIGA);
            }
        }
    }
}
