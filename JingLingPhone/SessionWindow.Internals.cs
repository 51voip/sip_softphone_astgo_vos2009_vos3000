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
using org.doubango.tinyWRAP;
using BogheCore.Utils;
using BogheCore.Sip;
using System.Drawing;
using BogheControls.Utils;
using BogheCore.Model;
using BogheCore;
using BogheApp.embedded;
using System.Threading;

namespace BogheApp
{
    partial class SessionWindow
    {
        private bool MsrpFailureReport
        {
            get
            {
                return this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.MSRP_FAILURE, Configuration.DEFAULT_RCS_MSRP_FAILURE);
            }
        }

        private bool MsrpSuccessReport
        {
            get
            {
                return this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.MSRP_SUCCESS, Configuration.DEFAULT_RCS_MSRP_SUCCESS);
            }
        }

        private bool MsrpOmaFinalDeliveryReport
        {
            get
            {
                return this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.OMAFDR, Configuration.DEFAULT_RCS_OMAFDR);
            }
        }

        private bool IsComposingAlertEnabled
        {
            get
            {
                return this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.ISCOMOPING, Configuration.DEFAULT_RCS_ISCOMOPING);
            }
        }

        private void imActivityIndicator_SendMessageEvent(object sender, EventArgs e)
        {
            if (this.IsComposingAlertEnabled && this.ChatSession != null)
            {
                if (this.Dispatcher.Thread != Thread.CurrentThread)
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new EventHandler<EventArgs>(this.imActivityIndicator_SendMessageEvent), sender, new object[] { e });
                    return;
                }

                if (this.ChatSession != null)
                {
                    this.ChatSession.SendMessage(this.imActivityIndicator.GetMessageIndicator(), ContentType.IS_COMPOSING, null);
                }
            }
        }

        private void imActivityIndicator_RemoteStateChangedEvent(object sender, EventArgs e)
        {
            if (this.IsComposingAlertEnabled)
            {
                if (this.Dispatcher.Thread != Thread.CurrentThread)
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new EventHandler<EventArgs>(this.imActivityIndicator_RemoteStateChangedEvent), sender, new object[] { e });
                    return;
                }
                this.imageIsComposing.Visibility = this.imActivityIndicator.IsRemoteActive ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
        }

        private MyMsrpSession CreateOutgoingSession(MediaType mediaType)
        {
            MyMsrpSession msrpSession = MyMsrpSession.CreateOutgoingSession(this.sipService.SipStack, mediaType, this.remotePartyUri);
            msrpSession.SuccessReport = this.MsrpSuccessReport;
            msrpSession.FailureReport = this.MsrpFailureReport;
            msrpSession.OmaFinalDeliveryReport = this.MsrpOmaFinalDeliveryReport;

            return msrpSession;
        }

        private void SendFile(String filePath)
        {
            MyMsrpSession msrpSession = this.CreateOutgoingSession(MediaType.FileTransfer);
            lock (this.fileTransferSessions)
            {
                this.fileTransferSessions.Add(msrpSession);
            }
            msrpSession.mOnMsrpEvent += this.FileTransfer_onMsrpEvent;

            HistoryFileTransferEvent @event = new HistoryFileTransferEvent(this.remotePartyUri, filePath);
            @event.Status = HistoryEvent.StatusType.Outgoing;
            @event.MsrpSession = msrpSession;
            @event.SipSessionId = msrpSession.Id;
            this.AddMessagingEvent(@event);

            msrpSession.SendFile(filePath);
        }        

        private void AttachDisplays()
        {
            if (this.AVSession == null || this.AVSession.MediaSessionMgr == null)
            {
                return;
            }

            lock (this.AVSession)
            {
                // Remote
                this.AVSession.MediaSessionMgr.consumerSetInt64(twrap_media_type_t.twrap_media_video, "remote-hwnd", this.videoDisplayRemote.Handle.ToInt64());
                this.AVSession.MediaSessionMgr.producerSetInt64(twrap_media_type_t.twrap_media_video, "local-hwnd", this.videoDisplayLocal.Handle.ToInt64());

#if FALSE // Early Media => "IsConnected" must be "MediaStarted"
                if (this.AVSession.IsConnected)
#endif
                {
                    this.videoDisplayLocal.Visibility = System.Windows.Visibility.Visible;
                    this.videoDisplayRemote.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void AddMessagingEvent(HistoryEvent @event)
        {
            this.historyDataSource.Add(@event);

            if (this.chatHistoryEvent != null)
            {
                this.chatHistoryEvent.Messages.Add(@event as HistoryShortMessageEvent);
            }
            else
            {
                switch (@event.MediaType)
                {
                    case BogheCore.MediaType.ShortMessage:
                    case BogheCore.MediaType.SMS:
                        this.historyService.AddEvent(@event);
                        break;
                }
            }

            if (@event.Status == HistoryEvent.StatusType.Incoming)
            {
                this.soundService.PlayNewEvent();
            }
        }

        private void InitializeView()
        {
            if (this.AVSession != null)
            {
                lock (this.AVSession)
                {
                    this.Title = String.Format("{0} {1}", Strings.Text_TalkingWith, this.AVSession.RemotePartyDisplayName);
                    this.UpdateControls();
                }
            }
            else
            {
                this.Title = String.Format("{0} {1}", Strings.Text_TalkingWith, UriUtils.GetDisplayName(this.remotePartyUri));
            }
        }

        private void UpdateControls()
        {
            if (this.AVSession != null)
            {
                lock (this.AVSession)
                {
                    switch (this.AVSession.State)
                    {
                        case MyInviteSession.InviteState.INCOMING:
                            this.labelInfo.Content = String.Format("{0} {1}", Strings.Text_IncomingCall, this.AVSession.RemotePartyDisplayName);
                            this.buttonHangUp.IsEnabled = true;
                            this.UpdateButtonCallOrAnswer(true, Strings.Text_Answer, Properties.Resources.phone_pick_up_32);
                            this.UpdateMenuItemAddRemoveVideo(false);
                            
                            this.MenuItemCall_MakeAudioCall.IsEnabled = false;
                            this.MenuItemCall_MakeVideoCall.IsEnabled = false;
                            this.MenuItemCall_HoldResume.IsEnabled = false;
                            this.MenuItemCall_MakeTransfer.IsEnabled = false;
                            this.MenuItemCall_HangUp.IsEnabled = true;
                            this.sliderVolume.IsEnabled = true;
                            this.buttonSound.IsEnabled = true;
                            this.imageIndicatorSecurity.Visibility = System.Windows.Visibility.Hidden;
                            this.imageFullScreen.Visibility = System.Windows.Visibility.Hidden;
                            break;

                        case MyInviteSession.InviteState.INPROGRESS:
                            this.buttonHangUp.IsEnabled = true;
                            this.UpdateButtonCallOrAnswer(false, Strings.Text_Call, Properties.Resources.phone_pick_up_32);
                            this.UpdateMenuItemAddRemoveVideo(false);

                            this.MenuItemCall_MakeAudioCall.IsEnabled = false;
                            this.MenuItemCall_MakeVideoCall.IsEnabled = false;
                            this.MenuItemCall_HoldResume.IsEnabled = false;
                            this.MenuItemCall_MakeTransfer.IsEnabled = false;
                            this.MenuItemCall_HangUp.IsEnabled = true;
                            this.sliderVolume.IsEnabled = true;
                            this.buttonSound.IsEnabled = true;
                            this.imageIndicatorSecurity.Visibility = System.Windows.Visibility.Hidden;
                            this.imageFullScreen.Visibility = System.Windows.Visibility.Hidden;
                            break;

                        case MyInviteSession.InviteState.INCALL:
                            this.buttonHangUp.IsEnabled = true;
                            this.UpdateButtonCallOrAnswer(false, Strings.Text_Call, Properties.Resources.phone_pick_up_32);
                            this.UpdateMenuItemAddRemoveVideo(true);

                            this.MenuItemCall_MakeAudioCall.IsEnabled = false;
                            this.MenuItemCall_MakeVideoCall.IsEnabled = false;
                            this.MenuItemCall_HoldResume.IsEnabled = true;
                            this.MenuItemCall_MakeTransfer.IsEnabled = true;
                            this.MenuItemCall_HangUp.IsEnabled = true;
                            this.sliderVolume.IsEnabled = true;
                            this.buttonSound.IsEnabled = true;
                            this.imageIndicatorSecurity.Visibility = this.AVSession.IsSecure() ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                            this.imageFullScreen.Visibility = ((this.AVSession.MediaType & MediaType.Video) == MediaType.Video) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                            break;

                        case MyInviteSession.InviteState.TERMINATED:
                        case MyInviteSession.InviteState.TERMINATING:
                            this.buttonHangUp.IsEnabled = false;
                            this.UpdateButtonCallOrAnswer(true, Strings.Text_Call, Properties.Resources.phone_pick_up_32);
                            this.UpdateMenuItemAddRemoveVideo(false);

                            this.MenuItemCall_MakeAudioCall.IsEnabled = true;
                            this.MenuItemCall_MakeVideoCall.IsEnabled = true;
                            this.MenuItemCall_HoldResume.IsEnabled = false;
                            this.MenuItemCall_MakeTransfer.IsEnabled = false;
                            this.MenuItemCall_HangUp.IsEnabled = false;
                            this.sliderVolume.IsEnabled = false;
                            this.buttonSound.IsEnabled = false;
                            this.imageIndicatorSecurity.Visibility = System.Windows.Visibility.Hidden;
                            this.imageFullScreen.Visibility = System.Windows.Visibility.Hidden;
                            break;
                    }
                }
            }
        }

        private void UpdateButtonCallOrAnswer(bool enabled, String text, Bitmap image)
        {
            this.buttonCallOrAnswer.Tag = text;
            this.buttonCallOrAnswer.IsEnabled = enabled;
            this.buttonCallOrAnswerLabel.Content = text;
            this.buttonCallOrAnswerImage.Source = MyImageConverter.FromBitmap(image);
        }

        private void UpdateMenuItemAddRemoveVideo(bool enabled)
        {
            this.MenuItemCall_AddRemoveVideo.IsEnabled = enabled;
            if (this.AVSession != null)
            {
                if ((this.AVSession.MediaType & MediaType.Video) == MediaType.Video)
                {
                    this.MenuItemCall_AddRemoveVideo.Header = Strings.AV_MenuRemoveVideo;
                    this.MenuItemCall_AddRemoveVideoImage.Source = MyImageConverter.FromBitmap(Properties.Resources.video_pause_16);
                }
                else
                {
                    this.MenuItemCall_AddRemoveVideo.Header = Strings.AV_MenuAddVideo;

                    this.MenuItemCall_AddRemoveVideoImage.Source = MyImageConverter.FromBitmap(Properties.Resources.video_play_16);
                }
            }
        }
    }
}
