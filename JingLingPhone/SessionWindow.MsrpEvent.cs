using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BogheCore.Sip.Events;
using System.Threading;
using BogheCore;
using BogheCore.Model;
using BogheCore.Sip;

namespace BogheApp
{
    partial class SessionWindow
    {
        private void FileTransfer_onMsrpEvent(object sender, MsrpEventArgs e)
        {
            if (!this.historyDataSource.Any((x) => x.SipSessionId == e.SessionId))
            {
                return;
            }

            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<MsrpEventArgs>(this.FileTransfer_onMsrpEvent), sender, new object[] { e });
                return;
            }

            MyMsrpSession session = (e.GetExtra(MsrpEventArgs.EXTRA_SESSION) as MyMsrpSession);
            if (session == null)
            {
                LOG.Error("No matching MSRP session could be found");
                return;
            }

            switch (e.Type)
            {
                case MsrpEventTypes.CONNECTED:
                    break;

                case MsrpEventTypes.DISCONNECTED:
                    {
                        lock (this.fileTransferSessions)
                        {
                            this.fileTransferSessions.RemoveAll((x) => x.Id == session.Id);
                        }
                        break;
                    }

                case MsrpEventTypes.ERROR:
                    LOG.Error(String.Format("MSRP session error code={0}", e.GetExtra(MsrpEventArgs.EXTRA_RESPONSE_CODE)));
                    session.HangUp();
                    break;

                case MsrpEventTypes.SUCCESS_2XX:
                    {
                        long? end = e.GetExtra(MsrpEventArgs.EXTRA_BYTE_RANGE_END) as long?;
                        long? total = e.GetExtra(MsrpEventArgs.EXTRA_BYTE_RANGE_TOTAL) as long?;

                        if (end.HasValue && total.HasValue && end.Value >= 0 && total.Value >= 0)
                        {
                            if (end.Value >= total.Value)
                            {
                                session.HangUp();
                            }
                        }
                        else
                        {
                            LOG.Error(String.Format("Invalid MSRP byte-range: {0}-{1}", end, total));
                        }

                        break;
                    }

                case MsrpEventTypes.SUCCESS_REPORT:
                    {
                        break;
                    }
            }
        }

        private void ChatSession_onMsrpEvent(object sender, MsrpEventArgs e)
        {
            if (this.ChatSession == null || this.ChatSession.Id != e.SessionId)
            {
                LOG.Error("Invalid chat session");
                return;
            }

            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<MsrpEventArgs>(this.ChatSession_onMsrpEvent), sender, new object[] { e });
                return;
            }

            switch (e.Type)
            {
                case MsrpEventTypes.CONNECTED:
                    break;

                case MsrpEventTypes.DISCONNECTED:
                    {
                        this.ChatSession = null;
                        break;
                    }

                case MsrpEventTypes.DATA:
                    {
                        byte[] data = (e.GetExtra(MsrpEventArgs.EXTRA_DATA) as byte[]);
                        if (data != null)
                        {
                            String contentType = (e.GetExtra(MsrpEventArgs.EXTRA_CONTENT_TYPE) as String);
                            
                            if (contentType != null)
                            {
                                contentType = contentType.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];
                                if (contentType.Equals(ContentType.CPIM, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Object wrappedContentType = e.GetExtra(MsrpEventArgs.EXTRA_WRAPPED_CONTENT_TYPE);
                                    contentType = wrappedContentType == null ? "UnknowWrappedType" : wrappedContentType as String;
                                }
                            }

                            if (ContentType.IS_COMPOSING.Equals(contentType, StringComparison.InvariantCultureIgnoreCase))
                            {
                                this.imActivityIndicator.OnIndicationReceived(Encoding.UTF8.GetString(data));
                                return;
                            }

                            HistoryShortMessageEvent @event = new HistoryShortMessageEvent(this.remotePartyUri);
                            @event.Status = HistoryEvent.StatusType.Incoming;
                            if (contentType.Equals(ContentType.TEXT_PLAIN, StringComparison.InvariantCultureIgnoreCase))
                            {
                                @event.Content = Encoding.UTF8.GetString(data);
                            }
                            else
                            {
                                @event.Content = String.Format("{0} not supported as content type", contentType);
                                LOG.Warn(@event.Content);
                            }

                            if (this.IsComposingAlertEnabled)
                            {
                                this.imActivityIndicator.OnContentReceived();
                            }

                            this.AddMessagingEvent(@event);
                        }
                        break;
                    }

                case MsrpEventTypes.ERROR:
                    break;

                case MsrpEventTypes.SUCCESS_2XX:
                    break;

                case MsrpEventTypes.SUCCESS_REPORT:
                    break;
            }
        }
    }
}
