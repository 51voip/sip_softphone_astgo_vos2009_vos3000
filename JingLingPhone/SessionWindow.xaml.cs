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
using BogheCore.Sip;
using BogheCore.Services;
using BogheApp.Services.Impl;
using BogheCore;
using System.Timers;
using System.Globalization;
using log4net;
using System.Reflection;
using BogheCore.Model;
using org.doubango.tinyWRAP;
using BogheControls;
using BogheApp.Items;
using System.Collections.Specialized;
using BogheControls.Utils;
using BogheApp.embedded;
using BogheCore.Utils;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.IO;

namespace BogheApp
{
    /// <summary>
    /// Interaction logic for SessionWindows.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        string phoneNum = "";//存储完整手机号码
        string tel = "";//存储手机号码的前缀

        private static ILog LOG = LogManager.GetLogger(typeof(SessionWindow));
        private static List<SessionWindow> windows = new List<SessionWindow>();

        private bool isTransfering = false;
        private bool isHeld = false;
        private MyMsrpSession chatSession = null;
        private MyAVSession avSession = null;
        private String transferUri = null;
        private MyAVSession avTransfSession = null;
        private int volume = 0;
        private readonly List<MyMsrpSession> fileTransferSessions;
        private readonly String remotePartyUri = null;
        private readonly IMActivityIndicator imActivityIndicator;

        private readonly IContactService contactService;
        private readonly ISipService sipService;
        private readonly IHistoryService historyService;
        public readonly ISoundService soundService;
        private readonly IConfigurationService configurationService;

        public readonly Timer timerCall;

        private HistoryChatEvent chatHistoryEvent;
        private HistoryAVCallEvent avHistoryEvent;

        private readonly VideoDisplay videoDisplayLocal;
        private readonly VideoDisplay videoDisplayRemote;

        private readonly MyObservableCollection<HistoryEvent> historyDataSource;

        public SessionWindow(String remotePartyUri)
            : base()
        {
            InitializeComponent();

            this.remotePartyUri = remotePartyUri;
            this.Title = String.Empty;
            this.buttonCallOrAnswer.Tag = Strings.Text_Call;

            this.fileTransferSessions = new List<MyMsrpSession>();
            this.imActivityIndicator = new IMActivityIndicator(this.remotePartyUri);

            this.videoDisplayLocal = new VideoDisplay();
            this.videoDisplayLocal.Visibility = Visibility.Hidden;
            this.videoDisplayLocal.VerticalAlignment = VerticalAlignment.Stretch;
            this.videoDisplayLocal.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.videoDisplayRemote = new VideoDisplay();
            this.videoDisplayRemote.Visibility = Visibility.Hidden;
            this.videoDisplayRemote.ToolTip = this.borderVideoDispalyRemote.ToolTip;

            this.borderVideoDispalyRemote.Child = this.videoDisplayRemote;
            this.borderVideoDispalyLocal.Child = this.videoDisplayLocal;

            this.labelInfo.Content = String.Empty;
            this.timerCall = new Timer(1000);
            this.timerCall.AutoReset = true;
            this.timerCall.Elapsed += this.timerCall_Elapsed;

            // Services
            this.contactService = Win32ServiceManager.SharedManager.ContactService;
            this.sipService = Win32ServiceManager.SharedManager.SipService;
            this.historyService = Win32ServiceManager.SharedManager.HistoryService;
            this.soundService = Win32ServiceManager.SharedManager.SoundService;
            this.configurationService = Win32ServiceManager.SharedManager.ConfigurationService;

            // Messaging
            this.historyDataSource = new MyObservableCollection<HistoryEvent>();
            this.historyCtrl.ItemTemplateSelector = new DataTemplateSelectorMessaging();
            this.historyCtrl.ItemsSource = this.historyDataSource;

            // Register events
            this.sipService.onInviteEvent += this.sipService_onInviteEvent;
            this.imActivityIndicator.RemoteStateChangedEvent += this.imActivityIndicator_RemoteStateChangedEvent;
            this.imActivityIndicator.SendMessageEvent += this.imActivityIndicator_SendMessageEvent;

            this.volume = this.configurationService.Get(Configuration.ConfFolder.GENERAL, Configuration.ConfEntry.AUDIO_VOLUME, Configuration.DEFAULT_GENERAL_AUDIO_VOLUME);
            this.sliderVolume.Value = (double)this.volume;

            lock (SessionWindow.windows)
            {
                SessionWindow.windows.Add(this);
            }
        }

        // 手机归属地查询
        public string GetNumAttribution()
        {
            string numAttribution = "未知";

            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\data"))//检查数据库是否存在
            {
                tel = phoneNum.Substring(0, 3);//取手机号码前面三位判断
                if (checkTel())
                {
                    phoneNum = phoneNum.Substring(0, 7);//取手机号码前面七位进行查询
                    string sql = "select * from '" + tel + "' where numberrange='" + phoneNum + "' limit 0,1";
                    SQLiteConnection con = null;
                    SQLiteCommand cmd = null;
                    SQLiteDataReader dr = null;
                    try
                    {
                        con = new SQLiteConnection("Data Source=" + System.Windows.Forms.Application.StartupPath + "\\data");
                        cmd = new SQLiteCommand(sql, con);
                        con.Open();
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)//如果存在此记录
                        {
                            numAttribution = dr.GetValue(1).ToString() + dr.GetValue(2).ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        return numAttribution;
                    }
                    finally
                    {
                        dr.Close();
                        dr.Dispose();
                        con.Close();
                        con.Dispose();
                    }
                }
            }

            return numAttribution;
        }

        //检验输入手机号码的合法性和正确性
        private bool checkTel()
        {
            if (phoneNum == null)//如果号码为空
            {
                return false;
            }
            Match num = Regex.Match(phoneNum, "[0-9]+");
            if (!num.Success)//如果号码不为数字
            {
                return false;
            }
            if (phoneNum.Length < 7)//如果号码长度小于七位
            {
                return false;
            }
            if (phoneNum.Length > 11)//如果号码长度大于十一位
            {
                return false;
            }
            if (tel != "130" && tel != "131" && tel != "132" && tel != "133" && tel != "134" &&
                tel != "135" && tel != "136" && tel != "137" && tel != "138" && tel != "139" &&
                tel != "150" && tel != "151" && tel != "152" && tel != "153" && tel != "156" &&
                tel != "158" && tel != "159")//如果不是手机号码
            {
                return false;
            }
            return true;
        }

        public static List<SessionWindow> Windows
        {
            get { return SessionWindow.windows; }
        }

        private bool IsHeld
        {
            get { return this.isHeld; }
            set
            {
                if (this.isHeld != value)
                {
                    this.isHeld = value;
                    if (this.isHeld)
                    {
                        this.MenuItemCall_HoldResume.Header = Strings.Text_ResumeCall;
                        this.MenuItemCall_HoldResumeImage.Source = MyImageConverter.FromBitmap(Properties.Resources.call_resume_16);
                    }
                    else
                    {
                        this.MenuItemCall_HoldResume.Header = Strings.Text_HoldCall;
                        this.MenuItemCall_HoldResumeImage.Source = MyImageConverter.FromBitmap(Properties.Resources.call_hold_16);
                    }
                }
            }
        }

        public MyAVSession AVSession
        {
            get { return this.avSession; }
            set { 
                this.avSession = value;
                this.IsHeld = false;
            }
        }

        public MyAVSession AVTransfSession
        {
            get { return this.avTransfSession; }
            set{
                this.avTransfSession = value;
            }
        }

        private int Volume
        {
            get
            {
                return this.volume;
            }
            set
            {
                if (this.AVSession != null)
                {
                    this.AVSession.SetVolume(value);
                }
                this.configurationService.Set(Configuration.ConfFolder.GENERAL, Configuration.ConfEntry.AUDIO_VOLUME, value);
                MediaSessionMgr.defaultsSetVolume(value);
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
                else
                {
                    this.imageIsComposing.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        public bool CanMakeAudioVideoCall(String remoteUri)
        {
            if (String.Equals(this.remotePartyUri, remoteUri))
            {
                return (this.AVSession == null);
            }
            return false;
        }

        public bool CanStartChat(String remoteUri)
        {
            if (String.Equals(this.remotePartyUri, remoteUri))
            {
                return (this.ChatSession == null);
            }
            return false;
        }

        public bool CanReceiveCall(MyInviteSession session)
        {
            if (String.Equals(this.remotePartyUri, session.RemotePartyUri))
            {
                if (session is MyMsrpSession)
                {
                    if (session.MediaType == MediaType.Chat)
                    {
                        return (this.ChatSession == null);
                    }
                    else
                    {
                        // Can always receive file transfer session as long as it's from the same remote uri
                        return true;
                    }
                }
                else if (session is MyAVSession)
                {
                    return (this.AVSession == null);
                }
            }
            return false;
        }

        public bool CanReceiveFile(String remoteUri)
        {
            return String.Equals(this.remotePartyUri, remoteUri);
        }

        public bool CanSendFile(String remoteUri)
        {
            return String.Equals(this.remotePartyUri, remoteUri);
        }

        public void MakeCall(String remoteUri, MediaType mediaType)
        {
            System.Diagnostics.Debug.Assert(this.AVSession == null);

            // Add T140 to the mediaType is the corresponding codec is enabled
            if (((tdav_codec_id_t)this.sipService.Codecs & tdav_codec_id_t.tdav_codec_id_t140) == tdav_codec_id_t.tdav_codec_id_t140)
            {
                mediaType |= MediaType.T140;
            }
           this.InitializeView();
            this.AVSession = MyAVSession.CreateOutgoingSession(Win32ServiceManager.SharedManager.SipService.SipStack, mediaType);
            //this.Show();

            this.AVSession.MakeCall(remoteUri);

           
        }

        public void MakeAudioCall(String remoteUri)
        {
            MakeCall(remoteUri, MediaType.Audio);
        }

        public void MakeVideoCall(String remoteUri)
        {
            MakeCall(remoteUri, MediaType.AudioVideo);
        }

        public void StartChat(String remoteUri)
        {
            System.Diagnostics.Debug.Assert(this.ChatSession == null);

            this.Show();
            this.InitializeView();
        }

        public void ReceiveCall(MyInviteSession session)
        {
            bool isAV = (session is MyAVSession);

            if(isAV)
            {
                System.Diagnostics.Debug.Assert(this.AVSession == null);

                this.AVSession = session as MyAVSession;
                this.avHistoryEvent = new HistoryAVCallEvent(((session.MediaType & MediaType.Video) == MediaType.Video), session.RemotePartyUri);
                this.avHistoryEvent.SipSessionId = session.Id;
                this.avHistoryEvent.Status = HistoryEvent.StatusType.Missed;
                
            }
            else if (session is MyMsrpSession)
            {
                MyMsrpSession msrpSession = session as MyMsrpSession;
                msrpSession.SuccessReport = this.MsrpSuccessReport;
                msrpSession.FailureReport = this.MsrpFailureReport;
                msrpSession.OmaFinalDeliveryReport = this.MsrpOmaFinalDeliveryReport;

                if (session.MediaType == MediaType.Chat)
                {
                    System.Diagnostics.Debug.Assert(this.ChatSession == null);

                     this.ChatSession = msrpSession;
                }
                else if (session.MediaType == MediaType.FileTransfer)
                {
                    HistoryFileTransferEvent @event = new HistoryFileTransferEvent(this.remotePartyUri, msrpSession.FilePath);
                    @event.Status = HistoryEvent.StatusType.Incoming;
                    @event.SipSessionId = session.Id;
                    @event.MsrpSession = msrpSession;
                    this.AddMessagingEvent(@event);
                }
                else
                {
                    throw new Exception("Unsupported session Type");
                }
            }
            else
            {
                throw new Exception("Unsupported session Type");
            }
            
            this.InitializeView();
            this.Show();


            if (isAV)
            {
                if (((session.MediaType & MediaType.Video) == MediaType.Video))
                {
                    this.AttachDisplays();
                }
          
            }
            else if (session.MediaType == MediaType.Chat && this.ChatSession != null)
            {
                this.ChatSession.Accept();
            }
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
                this.Show();

                this.InitializeView();
                this.SendFile(filePath);
            }
        }

        private void timerCall_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                {
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                            new EventHandler<ElapsedEventArgs>(this.timerCall_Elapsed), sender, new object[] { e });
                    return;
                }
                 
                if (this.avHistoryEvent != null)
                {
                    TimeSpan duration = (DateTime.Now - this.avHistoryEvent.StartTime);
                    Common.CallMain.H2.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", duration.Hours, duration.Minutes, duration.Seconds);
                    this.labelDuration.Content = string.Format("{0:D2}:{1:D2}:{2:D2}", duration.Hours, duration.Minutes, duration.Seconds);
                    
                
                }
            }
            catch (TargetInvocationException ex)
            {
                LOG.Error(ex);
            }
        }

        public void buttonCallOrAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (Strings.Text_Call.Equals(this.buttonCallOrAnswer.Tag) && this.AVSession == null)
            {
                this.AVSession = MyAVSession.CreateOutgoingSession(this.sipService.SipStack, MediaType.Audio);
                this.AVSession.MakeCall(this.remotePartyUri);
            }
            else if (this.AVSession != null)
            {
                this.AVSession.AcceptCall();
            }
        }

        private void buttonSound_Click(object sender, RoutedEventArgs e)
        {
            if (this.AVSession != null)
            {
                this.AVSession.Mute(!this.AVSession.IsMute, twrap_media_type_t.twrap_media_audio);
                this.imageSound.Source = MyImageConverter.FromBitmap(this.AVSession.IsMute ? Properties.Resources.sound_off_16 : Properties.Resources.sound_on_16);
            }
        }

        private void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Volume = (int)e.NewValue;
            // if ((e.OldValue == 0.0 && e.NewValue != 0.0) || (e.OldValue != 0.0 && e.NewValue == 0.0))
            //{
            //    this.imageSound.Source = MyImageConverter.FromBitmap(e.NewValue == 0.0 ? Properties.Resources.sound_off_16 : Properties.Resources.sound_on_16);
            //}
        }

        private void buttonHangUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.AVSession != null)
            {
                this.AVSession.HangUpCall();
            }
        }

        private void textBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*if (AVSession != null && (AVSession.MediaType & MediaType.T140) == MediaType.T140)
            {
                // Do not send on composing when T.140 is activated
                if (AVSession.IsConnected)
                {
                    if (!String.IsNullOrEmpty(this.textBoxInput.Text))
                    {
                        AVSession.SendT140Data(Encoding.UTF8.GetBytes(this.textBoxInput.Text));
                    }
                    this.textBoxInput.Text = String.Empty;
                }
                return;
            }*/

            if (this.IsComposingAlertEnabled && !String.IsNullOrEmpty(this.textBoxInput.Text))
            {
                this.imActivityIndicator.OnComposing();
            }
        }

        private void buttonSendText_Click(object sender, RoutedEventArgs e)
        {
            if (this.ChatSession == null)
            {
                this.ChatSession = this.CreateOutgoingSession(MediaType.Chat);
            }
            HistoryShortMessageEvent @event = new HistoryShortMessageEvent(this.remotePartyUri);
            @event.Status = HistoryEvent.StatusType.Outgoing;
            @event.SipSessionId = this.ChatSession.Id;
            @event.Content = this.textBoxInput.Text;
            this.AddMessagingEvent(@event);

            this.ChatSession.SendMessage(this.textBoxInput.Text);
            this.textBoxInput.Text = String.Empty;

            if (this.IsComposingAlertEnabled)
            {
                this.imActivityIndicator.OnContentSent();
            }
        }

        private void imageFullScreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.AVSession != null)
            {
                this.AVSession.SetFullscreen(true);
            }
        }

        private void SessionWindowName_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
 if (this.AVSession != null && this.AVSession.IsActive)
            {
                if (this.AVSession.State == MyInviteSession.InviteState.INCOMING)
                {
                    this.soundService.StopRingTone();
                }
                else if (this.AVSession.State == MyInviteSession.InviteState.INPROGRESS)
                {
                    this.soundService.StopRingBackTone();
                }
                this.AVSession.HangUpCall();
            }
            if (this.ChatSession != null && this.ChatSession.IsActive)
            {
                this.ChatSession.HangUp();
            }

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
            }
            catch (Exception)
            {
                
           
            }
           
        }

        private void SessionWindowName_Closed(object sender, EventArgs e)
        {
            this.AVSession = null;
            this.ChatSession = null;

            lock (SessionWindow.windows)
            {
                SessionWindow.windows.Remove(this);
            }
        }

        private void labelVideoDisplayRemote_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (this.AVSession != null && this.AVSession.IsConnected && this.AVSession.MediaSessionMgr != null)
            //{
            //    this.VideoDisplayRemote.Width = this.labelVideoDisplayRemote.Width;
            //}
        }

        private void SessionWindowName_Loaded(object sender, RoutedEventArgs e)
        {
            //


            if (this.AVSession != null)
            {
                
            }

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

            phoneNum = this.remotePartyUri.Substring(6);
            string str = GetNumAttribution();

            this.Title = "正在呼叫 " + this.remotePartyUri.Substring(6) + "(" + str + ")";
        }
    }
}