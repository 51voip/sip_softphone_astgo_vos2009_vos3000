using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BogheCore.Sip.Events;
using System.Threading;
using BogheApp.Screens;
using BogheCore.Sip;
using BogheApp.Services.Impl;
using BogheControls.Utils;
using BogheCore.Generated.regingo;
using System.Xml.Serialization;
using BogheCore.Model;
using System.IO;
using BogheXdm.Generated.watcherinfo;

namespace BogheApp
{
    partial class MainWindow
    {
        private void sipService_onStackEvent(object sender, StackEventArgs e)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<StackEventArgs>(this.sipService_onStackEvent), sender, new object[] { e });
                return;
            }

            switch (e.Type)
            {
                // for test, 正常情况下，需要全部放开注释

                //case StackEventTypes.START_OK:
                //case StackEventTypes.START_NOK:
                //case StackEventTypes.STOP_NOK:
                //case StackEventTypes.STOP_OK:
                //    this.screenService.SetProgressInfo("登录失败：请输入正确的用户名和密码");
                //    if (this.sipService.SipStack.State == MySipStack.STACK_STATE.STOPPED)
                //    {
                //        this.screenService.HideAllExcept(ScreenType.Options | ScreenType.Authorizations);
                //        this.screenService.Show(ScreenType.Authentication, 0);
                //    }
                //    break;
            }
        }

        private void sipService_onRegistrationEvent(object sender, RegistrationEventArgs e)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<RegistrationEventArgs>(this.sipService_onRegistrationEvent), sender, new object[] { e });
                return;
            }

            switch (e.Type)
            {
                //case RegistrationEventTypes.REGISTRATION_INPROGRESS:
                //    this.screenService.SetProgressInfo("正在登陆服务器，请稍后...");

                //    // indicators
                //    this.imageIndicatorConn.Source = MyImageConverter.FromBitmap(Properties.Resources.bullet_ball_glass_grey_24);
                //    break;

                case RegistrationEventTypes.REGISTRATION_NOK:
                    this.screenService.SetProgressInfo(e.Phrase);
                    break;

                // for test
                case RegistrationEventTypes.REGISTRATION_INPROGRESS:
                case RegistrationEventTypes.UNREGISTRATION_OK:
                case RegistrationEventTypes.REGISTRATION_OK:
                    this.screenService.SetProgressInfo("Signed In");

                    // Screens
                    this.screenService.Hide(ScreenType.Authentication);
                    this.screenService.Show(ScreenType.Contacts, 1);
                    this.screenService.Show(ScreenType.History, 2);
                    this.screenService.Show(ScreenType.PersonalCenter, 3);
                    this.screenService.Show(ScreenType.Call, 0);

                    // Menus
                    this.MenuItemFile_SignIn.IsEnabled = false;
                    this.MenuItemFile_SignOut.IsEnabled = true;
                    //this.MenuItemFile_Registrations.IsEnabled = true;
                    this.MenuItemEAB.IsEnabled = true;
                    this.MenuItemHistory.IsEnabled = true;

                    // indicators
                    this.imageIndicatorConn.Source = MyImageConverter.FromBitmap(Properties.Resources.bullet_ball_glass_green_24);

                    // Sound alert
                    // this.soundService.PlayConnectionChanged(true);

                    break;

                case RegistrationEventTypes.UNREGISTRATION_INPROGRESS:
                    this.screenService.SetProgressInfo("正在退出服务器，请稍后....");

                    // indicators
                    this.imageIndicatorConn.Source = MyImageConverter.FromBitmap(Properties.Resources.bullet_ball_glass_grey_24);
                    break;

                case RegistrationEventTypes.UNREGISTRATION_NOK:
                    this.screenService.SetProgressInfo(e.Phrase);
                    break;

                //case RegistrationEventTypes.UNREGISTRATION_OK:
                //    this.screenService.SetProgressInfo("Signed Out");
                //    this.screenService.SetProgressInfo("登录失败：请输入正确的用户名和密码");

                //    // Screens
                //    if (this.sipService.SipStack.State == MySipStack.STACK_STATE.STOPPED)
                //    {
                //        this.screenService.HideAllExcept(ScreenType.Options | ScreenType.Authorizations);
                //        this.screenService.Show(ScreenType.Authentication, 0);
                //    }

                //    // Menus
                //    this.MenuItemFile_SignIn.IsEnabled = true;
                //    this.MenuItemFile_SignOut.IsEnabled = false;
                //   // this.MenuItemFile_Registrations.IsEnabled = false;
                //    this.MenuItemEAB.IsEnabled = false;
                //    this.MenuItemHistory.IsEnabled = false;

                //    //...
                //    this.registrations.Clear();
                //    this.watchers.Clear();

                //    // indicators
                //    this.imageIndicatorConn.Source = MyImageConverter.FromBitmap(Properties.Resources.bullet_ball_glass_red_24);

                //    // Sound alert
                //    // this.soundService.PlayConnectionChanged(false);
                //    break;
            }
        }


        private void sipService_onInviteEvent(object sender, InviteEventArgs e)
        {
            switch (e.Type)
            {
                case InviteEventTypes.INCOMING:
                    Win32ServiceManager.SharedManager.Dispatcher.Invoke((System.Threading.ThreadStart)delegate
                    {
                        MediaActionHanler.ReceiveCall(e.GetExtra(InviteEventArgs.EXTRA_SESSION) as MyInviteSession);
                    }, null);
                    
                    break;

                default:
                    break;
            }
        }

        private void sipService_onMessagingEvent(object sender, MessagingEventArgs e)
        {
            switch (e.Type)
            {
                case MessagingEventTypes.INCOMING:
                    Win32ServiceManager.SharedManager.Dispatcher.Invoke((System.Threading.ThreadStart)delegate
                    {
                        MediaActionHanler.ReceiveShortMessage(e.GetExtra(MessagingEventArgs.EXTRA_REMOTE_PARTY) as String,
                            e.Payload, e.GetExtra(MessagingEventArgs.EXTRA_CONTENT_TYPE) as String);
                    }, null);

                    break;

                default:
                    break;
            }
        }

        private void sipService_onHyperAvailabilityTimedout(object sender, EventArgs e)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<EventArgs>(this.sipService_onHyperAvailabilityTimedout), sender, new object[] { e });
                return;
            }

           // this.comboBoxStatus.SelectedIndex = 0; // Online
        }

        private void sipService_onSubscriptionEvent(object sender, SubscriptionEventArgs e)
        {
            if (e.Type != SubscriptionEventTypes.INCOMING_NOTIFY 
                || e.Content == null
                || (e.Package != MySubscriptionSession.EVENT_PACKAGE_TYPE.REG && e.Package != MySubscriptionSession.EVENT_PACKAGE_TYPE.WINFO && e.Package != MySubscriptionSession.EVENT_PACKAGE_TYPE.MESSAGE_SUMMARY)
                )
            {
                return;
            }

            switch (e.Type)
            {
                case SubscriptionEventTypes.INCOMING_NOTIFY:
                    {
                        new Thread(delegate()
                        {
                            this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate
                                {
                                    if (e.Package == MySubscriptionSession.EVENT_PACKAGE_TYPE.REG)
                                    {
                                        this.ParseRegInfo(e.Content);
                                    }
                                    else if (e.Package == MySubscriptionSession.EVENT_PACKAGE_TYPE.WINFO)
                                    {
                                        this.ParseWatcherInfo(e.Content);
                                    }
                                    else if (e.Package == MySubscriptionSession.EVENT_PACKAGE_TYPE.MESSAGE_SUMMARY)
                                    {
                                        MessageSummary ms = MessageSummary.Parse(e.Content);
                                        if (ms != null)
                                        {
                                            //this.imageMailbox.Visibility = ms.HaveWaitingMessages ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                                            String tooltip = String.Format("Message-Account: {0}\n", ms.Account);
                                            new String[] { "Voice-Message", "Fax-Message", "Pager-Message", "Multimedia-Message", "Text-Message" }.ToList().ForEach(x =>
                                                {
                                                    tooltip += String.Format("{0}: {1}/{2} ({3}/{4})\n", x, ms.GetNewMessages(x), ms.GetOldMessages(x), ms.GetNewUrgentMessages(x), ms.GetOldUrgentMessages(x));
                                                });
                                            //this.imageMailbox.ToolTip = tooltip;
                                        }
                                    }
                                });
                        })
                        .Start();
                        break;
                    }
            }
        }

        private void ParseRegInfo(byte[] Content)
        {
            try
            {
                reginfo info = null;
                using (Stream stream = new MemoryStream(Content))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(reginfo));
                    info = serializer.Deserialize(stream) as reginfo;
                    if (info.registration != null)
                    {
                        if (String.Equals("full", info.state.ToString(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            this.registrations.Clear();
                        }
                        foreach (registration reg in info.registration)
                        {
                            if (reg.contact == null || reg.contact.Length == 0)
                            {
                                continue;
                            }
                            foreach (contact c in reg.contact)
                            {
                                RegistrationInfo registrationInfo = this.registrations.FirstOrDefault(x =>
                                    String.Equals(x.Id, reg.id) && String.Equals(x.ContactId, c.id));
                                bool isNew = (registrationInfo == null);
                                if (isNew)
                                {
                                    registrationInfo = new RegistrationInfo();
                                    registrationInfo.Id = reg.id;
                                    registrationInfo.AoR = reg.aor;
                                    registrationInfo.ContactId = c.id;
                                    registrationInfo.ContactUriString = c.uri;
                                }

                                registrationInfo.RegistrationState = reg.state;
                                registrationInfo.ContactDisplayName = (c.displayname == null) ? String.Empty : c.displayname.Value;
                                registrationInfo.ContactEvent = c.@event;
                                registrationInfo.ContactExpires = (Int32)((c.expiresSpecified) ? c.expires : 0);
                                registrationInfo.ContactState = c.state;

                                if (isNew)
                                {
                                    this.registrations.Add(registrationInfo);
                                }
                            }
                        }
                    }
                }

                // Remove terminated registration
                this.registrations.RemoveAll(x =>
                    x.ContactState == contactState.terminated);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }

        private void ParseWatcherInfo(byte[] Content)
        {
            try
            {
                watcherinfo winfo = null;
                using (Stream stream = new MemoryStream(Content))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(watcherinfo));
                    winfo = serializer.Deserialize(stream) as watcherinfo;
                    if (winfo.watcherlist != null)
                    {
                        if (String.Equals("full", winfo.state.ToString(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            this.watchers.Clear();
                        }

                        foreach (watcherlist wlist in winfo.watcherlist)
                        {
                            if (wlist.watcher == null || wlist.watcher.Length == 0)
                            {
                                continue;
                            }
                            foreach (watcher w in wlist.watcher)
                            {
                                WatcherInfo watcherInfo = this.watchers.FirstOrDefault(x =>
                                    String.Equals(x.Resource, wlist.resource) && String.Equals(x.WatcherId, w.id));
                                bool isNew = (watcherInfo == null);
                                if (isNew)
                                {
                                    watcherInfo = new WatcherInfo();
                                    watcherInfo.Resource = wlist.resource;
                                    watcherInfo.Package = wlist.package;
                                    watcherInfo.WatcherId = w.id;
                                    watcherInfo.WatcherUriString = w.Value;
                                }

                                watcherInfo.WatcherDisplayName = w.displayname;
                                watcherInfo.WatcherDurationSubscribed = (int)((w.durationsubscribedSpecified) ? w.durationsubscribed : 0);
                                watcherInfo.WatcherEvent = w.@event;
                                watcherInfo.WatcherExpiration = (int)((w.expirationSpecified) ? w.expiration : 0);
                                watcherInfo.WatcherStatus = w.status;

                                if (isNew)
                                {
                                    this.watchers.Add(watcherInfo);
                                }
                            }
                        }
                    }
                }

                // Remove terminated registration
                this.watchers.RemoveAll(x =>
                    x.WatcherStatus == watcherStatus.terminated);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }
    }
}
