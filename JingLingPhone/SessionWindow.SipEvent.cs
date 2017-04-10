using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BogheCore.Sip.Events;
using BogheCore.Model;
using BogheCore;
using BogheApp.embedded;
using System.Windows.Forms;
using BogheCore.Utils;
using BogheCore.Sip;

namespace BogheApp
{
    partial class SessionWindow
    {
        public void sipService_onInviteEvent(object sender, InviteEventArgs e)
        {
            if (this.AVSession == null || (this.AVSession.Id != e.SessionId && e.Type != InviteEventTypes.REMOTE_TRANSFER_INPROGESS))
            {
                /* Messaging */
                if (e.Type == InviteEventTypes.DISCONNECTED)
                {
                    if (this.historyDataSource.Any(x => x.SipSessionId == e.SessionId))
                    {
                        this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate
                        {
                            HistoryEvent @event = this.historyDataSource.FirstOrDefault(x => x.SipSessionId == e.SessionId);
                            if (@event != null)
                            {
                                this.historyService.AddEvent(@event);
                            }
                        }, null);
                    }
                    else if (this.ChatSession != null && this.ChatSession.Id == e.SessionId)
                    {
                        this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate
                        {
                            this.ChatSession = null;
                        });
                    }
                }
                return;
            }

            /* Audio/Video */

            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<InviteEventArgs>(this.sipService_onInviteEvent), sender, new object[] { e });
                return;
            }

            this.UpdateControls();

            switch (e.Type)
            {

                case InviteEventTypes.INCOMING:
                    {
                        this.labelInfo.Content = String.Format("{0} {1}", Strings.Text_IncomingCall, this.AVSession.RemotePartyDisplayName);
                        this.avHistoryEvent = new HistoryAVCallEvent(this.AVSession.MediaType != BogheCore.MediaType.Audio, this.AVSession.RemotePartyUri);
                        this.avHistoryEvent.Status = HistoryEvent.StatusType.Missed;
                        break;
                    }

                case InviteEventTypes.INPROGRESS:
                    {
                       
                        // History Event
                        this.labelInfo.Content = String.Format("{0}...", Strings.Text_CallInProgress);
                        bool isVideo = ((this.AVSession.MediaType & MediaType.Video) == MediaType.Video);
                        this.avHistoryEvent = new HistoryAVCallEvent(isVideo, this.AVSession.RemotePartyUri);
                        this.avHistoryEvent.Status = HistoryEvent.StatusType.Outgoing;
                        // Video Displays
                        if (isVideo)
                        {
                            this.AttachDisplays();
                        }
                        
                        break;
                    }

                case InviteEventTypes.RINGING:
                    {
                        this.soundService.StopRingBackTone();
                        this.soundService.StopRingTone();
                        this.labelInfo.Content = Strings.Text_Ringing;
                        Common.MP3Play.StopT();
                        Common.CallMain.timer.IsEnabled = false;
                        Common.CallMain.timer.Stop();
                        Common.CallMain.IsCall = false;
                        Common.CallMain.H2.Text = "电话已接通等待接听";
                        break;
                    }

                case InviteEventTypes.EARLY_MEDIA:
                    {
                        this.labelInfo.Content = Strings.Text_EarlyMediaStarted;
                        this.soundService.StopRingBackTone();
                        this.soundService.StopRingTone();
                        break;
                    }

                case InviteEventTypes.MEDIA_UPDATING:
                    {
                        this.labelInfo.Content = "Trying to update media...";
                        break;
                    }

                case InviteEventTypes.MEDIA_UPDATED:
                    {
                        bool isVideo = ((this.AVSession.MediaType & MediaType.Video) == MediaType.Video);
                        this.labelInfo.Content = String.Format("Media Updated - {0}", isVideo ? "Video" : "Audio");
                        if (isVideo)
                        {
                            this.AttachDisplays();
                        }
                        break;
                    }

                case InviteEventTypes.CONNECTED://连接成功触发
                    {
                        this.labelInfo.Content = Strings.Text_InCall;
                        this.soundService.StopRingBackTone();
                        this.soundService.StopRingTone();

                        this.videoDisplayLocal.Visibility = System.Windows.Visibility.Visible;
                        this.videoDisplayRemote.Visibility = System.Windows.Visibility.Visible;

                        this.timerCall.Start();
                        if (this.avHistoryEvent != null)
                        {
                            if (this.avHistoryEvent.Status == HistoryEvent.StatusType.Missed)
                            {
                                this.avHistoryEvent.Status = HistoryEvent.StatusType.Incoming;
                            }
                            this.avHistoryEvent.StartTime = DateTime.Now;
                            //this.avHistoryEvent.EndTime = this.avHistoryEvent.StartTime;
                        }

                        break;
                    }

                case InviteEventTypes.DISCONNECTED:

                    {
                        this.labelInfo.Content = e.Type == InviteEventTypes.TERMWAIT ? Strings.Text_CallTerminated : e.Phrase;
                        this.timerCall.Stop();
                        this.soundService.StopRingBackTone();
                        this.soundService.StopRingTone();
                        this.AVSession.PreDispose();
                        this.AVSession = null;

                        break;
                    
                    }

                case InviteEventTypes.TERMWAIT://挂断
                    {
                        this.labelInfo.Content = e.Type == InviteEventTypes.TERMWAIT ? Strings.Text_CallTerminated : e.Phrase;
                        this.timerCall.Stop();
                        this.soundService.StopRingBackTone();
                        this.soundService.StopRingTone();

                        if (this.avHistoryEvent != null&&e.Phrase.Contains("Call"))
                        {
                            lock (this.avHistoryEvent)
                            {
                                this.avHistoryEvent.EndTime = DateTime.Now;
                                this.historyService.AddEvent(this.avHistoryEvent);
                                //Common.CallMain.ShowView(1);//挂断后显示费率信息
                                this.avHistoryEvent = null;
                            }
                        }

                        //--this.videoDisplayLocal.Visibility = System.Windows.Visibility.Hidden;
                        //--this.videoDisplayRemote.Visibility = System.Windows.Visibility.Hidden;
                        this.AVSession.PreDispose();
                        this.AVSession = null;
                        break;
                    }

                case InviteEventTypes.LOCAL_HOLD_OK:
                    {
                        if (this.isTransfering)
                        {
                            this.isTransfering = false;
                            this.AVSession.TransferCall(this.transferUri);
                        }
                        this.labelInfo.Content = Strings.Text_CallPlacedOnHold;
                        this.IsHeld = true;
                        break;
                    }

                case InviteEventTypes.LOCAL_HOLD_NOK:
                    {
                        this.isTransfering = false;
                        this.labelInfo.Content = Strings.Text_FailedToPlaceRemotePartyOnHold;
                        break;
                    }

                case InviteEventTypes.LOCAL_RESUME_OK:
                    {
                        this.isTransfering = false;
                        this.labelInfo.Content = Strings.Text_CallTakenOffHold;
                        this.IsHeld = false;
                        break;
                    }

                case InviteEventTypes.LOCAL_RESUME_NOK:
                    {
                        this.isTransfering = false;
                        this.labelInfo.Content = Strings.Text_FailedToUnholdCall;
                        break;
                    }

                case InviteEventTypes.REMOTE_HOLD:
                    {
                        this.labelInfo.Content = Strings.Text_PlacedOnHoldByRemoteParty;
                        break;
                    }

                case InviteEventTypes.REMOTE_RESUME:
                    {
                        this.labelInfo.Content = Strings.Text_TakenOffHoldByRemoteParty;
                        break;
                    }


                case InviteEventTypes.LOCAL_TRANSFER_TRYING:
                    {
                        this.labelInfo.Content = String.Format("{0}: {1}", Strings.Text_CallTransfer, Strings.Text_Initiated);
                        break;
                    }
                case InviteEventTypes.LOCAL_TRANSFER_FAILED:
                    {
                        this.labelInfo.Content = String.Format("{0}: {1}", Strings.Text_CallTransfer, Strings.Text_Failed);
                        break;
                    }
                case InviteEventTypes.LOCAL_TRANSFER_ACCEPTED:
                    {
                        this.labelInfo.Content = String.Format("{0}: {1}", Strings.Text_CallTransfer, Strings.Text_Accepted);
                        break;
                    }
                case InviteEventTypes.LOCAL_TRANSFER_COMPLETED:
                    {
                        this.labelInfo.Content = String.Format("{0}: {1}", Strings.Text_CallTransfer, Strings.Text_Completed);
                        break;
                    }
                case InviteEventTypes.LOCAL_TRANSFER_NOTIFY:
                case InviteEventTypes.REMOTE_TRANSFER_NOTIFY:
                    {
                        short? code = e.GetExtra(InviteEventArgs.EXTRA_SIP_CODE) as short?;
                        this.labelInfo.Content = String.Format("{0}: {1} {2}", Strings.Text_CallTransfer, code.HasValue ? code.Value : -1, e.Phrase);
                        if (code.HasValue)
                        {
                            if (code.Value >= 300 && this.IsHeld)
                            {
                                this.AVSession.ResumeCall();
                            }
                        }
                        break;
                    }

                case InviteEventTypes.REMOTE_TRANSFER_REQUESTED:
                    {
                        new Thread((System.Threading.ParameterizedThreadStart)delegate(object _e)
                        {
                            this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate
                            {
                                InviteEventArgs args = _e as InviteEventArgs;
                                if (args != null)
                                {
                                    String referToUri = args.GetExtra(InviteEventArgs.EXTRA_REFERTO_URI) as String;
                                    String referToName = UriUtils.GetDisplayName(referToUri);
                                    DialogResult ret = MessageBox.Show(String.Format("Call Transfer to {0} requested. Do you accept?", referToName), "Call Transfer Request", MessageBoxButtons.YesNo);
                                    if (this.AVSession != null)
                                    {
                                        if (ret == System.Windows.Forms.DialogResult.Yes)
                                        {
                                            this.AVSession.AcceptCallTransfer();
                                        }
                                        else
                                        {
                                            this.AVSession.RejectCallTransfer();
                                        }
                                    }
                                }
                            });
                        })
                        .Start(e);
                        break;
                    }

                case InviteEventTypes.REMOTE_TRANSFER_INPROGESS:
                    {
                        this.AVTransfSession = e.GetExtra(InviteEventArgs.EXTRA_SESSION) as MyAVSession;
                        break;
                    }
                case InviteEventTypes.REMOTE_TRANSFER_FAILED:
                    {
                        this.AVTransfSession = null;
                        break;
                    }
                case InviteEventTypes.REMOTE_TRANSFER_COMPLETED:
                    {
                        if (this.AVTransfSession != null)
                        {
                            this.AVSession = this.AVTransfSession;
                            this.AVTransfSession = null;
                            this.InitializeView();
                            this.UpdateControls();
                        }
                        break;
                    }
            }
        }
    }
}
