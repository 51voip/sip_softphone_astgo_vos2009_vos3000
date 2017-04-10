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
using BogheCore.Services;
using BogheApp.Services.Impl;
using BogheCore;
using BogheCore.Sip;
using BogheControls.Utils;
using BogheCore.Model;
using BogheApp.Items;
using log4net;
using System.Collections.Specialized;
using BogheApp.embedded;
using BogheCore.Utils;
using System.ComponentModel;

namespace BogheApp
{
    /// <summary>
    /// Interaction logic for MessagingWindow.xaml
    /// </summary>
    public partial class MessagingWindow : Window
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MessagingWindow));
        private static List<MessagingWindow> windows = new List<MessagingWindow>();

        private readonly String remotePartyUri = null;

        private readonly IConfigurationService configurationService;
        private readonly IContactService contactService;
        private readonly ISipService sipService;
        private readonly IHistoryService historyService;
        private readonly ISoundService soundService;

        private MediaType messagingType;
        private MyMsrpSession chatSession = null;
        private HistoryChatEvent chatHistoryEvent;
        private readonly List<MyMsrpSession> fileTransferSessions;
        private readonly IMActivityIndicator imActivityIndicator;

        private readonly MyObservableCollection<HistoryEvent> historyDataSource;
        private readonly MyObservableCollection<Participant> participants;
        private ICollectionView participantsView;

        public MessagingWindow(String remotePartyUri)
        {
            InitializeComponent();

            this.remotePartyUri = remotePartyUri;
            this.Title = String.Empty;
            this.messagingType = MediaType.None;
            this.fileTransferSessions = new List<MyMsrpSession>();
            this.imActivityIndicator = new IMActivityIndicator(this.remotePartyUri);

            // Services
            this.configurationService = Win32ServiceManager.SharedManager.ConfigurationService;
            this.contactService = Win32ServiceManager.SharedManager.ContactService;
            this.sipService = Win32ServiceManager.SharedManager.SipService;
            this.historyService = Win32ServiceManager.SharedManager.HistoryService;
            this.soundService = Win32ServiceManager.SharedManager.SoundService;

            // Messaging
            this.historyDataSource = new MyObservableCollection<HistoryEvent>();
            this.historyCtrl.ItemTemplateSelector = new DataTemplateSelectorMessaging();
            this.historyCtrl.ItemsSource = this.historyDataSource;

            // Participants
            this.participants = new MyObservableCollection<Participant>();
            this.participants.Add(new Participant(this.remotePartyUri));
            this.listBoxParticipants.ItemsSource = this.participants;
            this.participantsView = CollectionViewSource.GetDefaultView(this.listBoxParticipants.ItemsSource);

            // Events
            this.sipService.onInviteEvent += this.sipService_onInviteEvent;
            this.imActivityIndicator.RemoteStateChangedEvent += this.imActivityIndicator_RemoteStateChangedEvent;
            this.imActivityIndicator.SendMessageEvent += this.imActivityIndicator_SendMessageEvent;

            lock (MessagingWindow.windows)
            {
                MessagingWindow.windows.Add(this);
            }
        }

        public static List<MessagingWindow> Windows
        {
            get { return MessagingWindow.windows; }

        }

        private bool UseBinarySMS
        {
            get
            {
                return this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.BINARY_SMS, Configuration.DEFAULT_RCS_BINARY_SMS);
            }
        }

        private String SMSCAddress
        {
            get
            {
                return this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.SMSC, Configuration.DEFAULT_RCS_SMSC);
            }
        }

        private MediaType MessagingType
        {
            get { return this.messagingType; }
            set
            {
                this.messagingType = value;
                switch (this.messagingType)
                {
                    case MediaType.SMS:
                    case MediaType.ShortMessage:
                        this.buttonSendTextImage.Source = MyImageConverter.FromBitmap(Properties.Resources.message_24);
                        this.buttonSendTextLabel.Content = Strings.Text_SendSMS;
                        break;

                    case MediaType.Chat:
                        this.buttonSendTextImage.Source = MyImageConverter.FromBitmap(Properties.Resources.messages_24);
                        this.buttonSendTextLabel.Content = Strings.Text_SendText;
                        break;

                    default:
                        break;
                }
            }
        }

        private MyMsrpSession ChatSession
        {
            get { return this.chatSession; }
            set
            {
                if (this.chatSession != null)
                {
                    this.chatSession.mOnMsrpEvent -= this.ChatSession_onMsrpEvent;
                    if (this.chatHistoryEvent != null)
                    {
                        this.historyService.AddEvent(this.chatHistoryEvent);
                    }
                }
                if ((this.chatSession = value) != null)
                {
                    this.chatSession.mOnMsrpEvent += this.ChatSession_onMsrpEvent;
                    this.chatHistoryEvent = new HistoryChatEvent(this.remotePartyUri);
                    this.chatHistoryEvent.SipSessionId = value.Id;
                }
            }
        }

        public bool CanSendSMS(String remoteUri)
        {
            if(String.Equals(this.remotePartyUri, remoteUri))
            {
                return this.MessagingType == MediaType.SMS;
            }
            return false;
        }

        public bool CanSendShortMessage(String remoteUri)
        {
            if (String.Equals(this.remotePartyUri, remoteUri))
            {
                return this.MessagingType == MediaType.ShortMessage;
            }
            return false;
        }

        public bool CanSendFile(String remoteUri)
        {
            return String.Equals(this.remotePartyUri, remoteUri);
        }

        public bool CanReceiveShortMessage(String remoteUri)
        {
            if (String.Equals(this.remotePartyUri, remoteUri))
            {
                return (this.MessagingType == MediaType.ShortMessage || this.MessagingType == MediaType.SMS);
            }
            return false;
        }

        public bool CanStartChat(String remoteUri)
        {
            if (String.Equals(this.remotePartyUri, remoteUri))
            {
                if (this.MessagingType == MediaType.Chat)
                {
                    return (this.ChatSession == null);
                }
            }
            return false;
        }

        public void SendSMS(String remoteUri)
        {
            System.Diagnostics.Debug.Assert(this.messagingType == MediaType.None 
            || this.MessagingType == MediaType.SMS || this.MessagingType == MediaType.ShortMessage);

            if (this.messagingType == MediaType.None)
            {
                this.MessagingType = MediaType.SMS;
            }
            this.Show();

            this.InitializeView();
        }

        public void ReceiveShortMessage(String remoteUri, byte[] payload, String contentType)
        {
            if (ContentType.IS_COMPOSING.Equals(contentType, StringComparison.InvariantCultureIgnoreCase))
            {
                if (this.IsComposingAlertEnabled)
                {
                    this.imActivityIndicator.OnIndicationReceived(Encoding.UTF8.GetString(payload));
                }
                return;
            }
            System.Diagnostics.Debug.Assert(this.messagingType == MediaType.None
            || this.MessagingType == MediaType.SMS || this.MessagingType == MediaType.ShortMessage);

            if (this.MessagingType == MediaType.None)
            {
                this.MessagingType = MediaType.SMS;
            }
                        
            this.Show();

            HistoryShortMessageEvent @event = new HistoryShortMessageEvent(remoteUri);
            @event.Status = HistoryEvent.StatusType.Incoming;
            @event.Content = Encoding.UTF8.GetString(payload);

            if (this.IsComposingAlertEnabled)
            {
                this.imActivityIndicator.OnContentReceived();
            }

            this.AddMessagingEvent(@event);
        }

        public void StartChat(String remoteUri)
        {
            System.Diagnostics.Debug.Assert(this.MessagingType == MediaType.None
            || this.MessagingType == MediaType.Chat);


            if (this.MessagingType == MediaType.None)
            {
                this.MessagingType = MediaType.Chat;
            }
            
            this.Show();
            this.InitializeView();
        }

        public void SendFile(String remoteUri, String filePath)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
                fileDialog.Multiselect = false;
                Nullable<bool> result = fileDialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    filePath = fileDialog.FileName;
                }
            }

            if (!String.IsNullOrEmpty(filePath))
            {
                if (this.MessagingType == MediaType.None)
                {
                    this.MessagingType = MediaType.Chat;
                }
                this.Show();

                this.InitializeView();
                this.SendFile(filePath);
            }
        }

        private void buttonSendText_Click(object sender, RoutedEventArgs e)
        {
            if (this.textBoxInput.Text == String.Empty)
            {
                return;
            }

            HistoryShortMessageEvent @event = new HistoryShortMessageEvent(this.remotePartyUri);
            @event.Status = HistoryEvent.StatusType.Outgoing;
            @event.Content = this.textBoxInput.Text;

            switch (this.messagingType)
            {
                case MediaType.Chat:
                    {
                        if (this.ChatSession == null)
                        {
                            this.ChatSession = this.CreateOutgoingSession(MediaType.Chat);
                        }
                        @event.SipSessionId = this.ChatSession.Id;
                        this.ChatSession.SendMessage(this.textBoxInput.Text);
                        this.textBoxInput.Text = String.Empty;
                        break;
                    }

                case MediaType.ShortMessage:
                case MediaType.SMS:
                default:
                    {
                        MyMessagingSession shortMessageSession = new MyMessagingSession(this.sipService.SipStack, this.remotePartyUri);
                        if (this.UseBinarySMS)
                        {
                            shortMessageSession.SendBinaryMessage(this.textBoxInput.Text, this.SMSCAddress);
                        }
                        else
                        {
                            shortMessageSession.SendTextMessage(this.textBoxInput.Text);
                        }
                        this.textBoxInput.Text = String.Empty;
                        shortMessageSession.Dispose();
                        break;
                    }
            }

            if (this.IsComposingAlertEnabled)
            {
                this.imActivityIndicator.OnContentSent();
            }

            this.AddMessagingEvent(@event);
        }

        private void MenuItemCall_MakeAudioCall_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.MakeAudioCall(this.remotePartyUri);
        }

        private void MenuItemCall_MakeVideoCall_Click(object sender, RoutedEventArgs e)
        {
            MediaActionHanler.MakeVideoCall(this.remotePartyUri);
        }

        private void MenuItemCall_ShareImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG" // From IR.79
            };
            if (fileDialog.ShowDialog() == true)
            {
                this.SendFile(fileDialog.FileName);
            }
        }

        private void MenuItemCall_ShareVideo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonSendFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Multiselect = false
            };
            if (fileDialog.ShowDialog() == true)
            {
                this.SendFile(fileDialog.FileName);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (this.fileTransferSessions)
            {
                this.fileTransferSessions.ForEach((x) =>
                    {
                        if (x != null && x.IsActive)
                        {
                            x.HangUp();
                        }
                    });
            }

            if (this.ChatSession != null && this.ChatSession.IsActive)
            {
                this.ChatSession.HangUp();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            lock (this.fileTransferSessions)
            {
                foreach (MyMsrpSession session in this.fileTransferSessions)
                {
                    this.fileTransferSessions.ForEach((x) =>
                    {
                        if (x != null)
                        {
                            x.mOnMsrpEvent -= this.FileTransfer_onMsrpEvent;
                        }
                    });
                }
            }

            lock (MessagingWindow.windows)
            {
                MessagingWindow.windows.Remove(this);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.historyDataSource.CollectionChanged += (_sender, _e) =>
            {
                switch (_e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Reset:
                        this.historyCtrlScrollViewer.ScrollToEnd();
                        break;
                }
            };
        }

        private void textBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsComposingAlertEnabled && !String.IsNullOrEmpty(this.textBoxInput.Text))
            {
                this.imActivityIndicator.OnComposing();
            }
        }
    }
}
