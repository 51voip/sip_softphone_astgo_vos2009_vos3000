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
using BogheCore.Sip.Events;
using BogheCore.Sip;
using BogheCore;
using BogheCore.Model;
using System.Threading;

namespace BogheApp
{
    partial class MessagingWindow
    {
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


        private void FileTransfer_onMsrpEvent(object sender, MsrpEventArgs e)
        {
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
                            this.fileTransferSessions.Remove(session);
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
    }
}
