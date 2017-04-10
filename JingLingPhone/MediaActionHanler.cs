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
using BogheCore.Sip;

namespace BogheApp
{
    internal static class MediaActionHanler
    {
        internal static void MakeAudioCall(String remoteUri)
        {
            SessionWindow window = null;
            foreach (var item in SessionWindow.Windows )
            {
                item.Close();
            }
            //lock (SessionWindow.Windows)
            //{
            //    window = SessionWindow.Windows.FirstOrDefault(w => w.CanMakeAudioVideoCall(remoteUri));
            //}
            if (window == null)
            {
                window = new SessionWindow(remoteUri);
                //将会话窗口给公用变量
                Common.CurrSession = window;
            }
            window.MakeAudioCall(remoteUri);
        }

        internal static void MakeVideoCall(String remoteUri)
        {
            SessionWindow window = null;

            lock (SessionWindow.Windows)
            {
                window = SessionWindow.Windows.FirstOrDefault(w => w.CanMakeAudioVideoCall(remoteUri));
            }
            if (window == null)
            {
                window = new SessionWindow(remoteUri);
            }
            window.MakeVideoCall(remoteUri);
        }

        internal static void StartChat(String remoteUri)
        {
            SessionWindow sessionWindow = null;
            MessagingWindow messagingWindow = null;

            lock (SessionWindow.Windows)
            {
                sessionWindow = SessionWindow.Windows.FirstOrDefault(w => w.CanStartChat(remoteUri));
            }

            if (sessionWindow == null)
            {
                lock (MessagingWindow.Windows)
                {
                    messagingWindow = MessagingWindow.Windows.FirstOrDefault(w => w.CanStartChat(remoteUri));
                }
                if (messagingWindow == null)
                {
                    messagingWindow = new MessagingWindow(remoteUri);
                }
                messagingWindow.StartChat(remoteUri);
            }
            else
            {
                sessionWindow.StartChat(remoteUri);
            }
        }

        internal static void ReceiveCall(MyInviteSession session)
        {
            SessionWindow window = null;

            lock (SessionWindow.Windows)
            {
                window = SessionWindow.Windows.FirstOrDefault(w => w.CanReceiveCall(session));
            }
            if (window == null)
            {
                window = new SessionWindow(session.RemotePartyUri);
            }
            window.ReceiveCall(session);
        }

        internal static void SendFile(String remoteUri, String filePath)
        {
            SessionWindow sessionWindow = null;
            MessagingWindow messagingWindow = null;

            lock (SessionWindow.Windows)
            {
                sessionWindow = SessionWindow.Windows.FirstOrDefault(w => w.CanSendFile(remoteUri));
            }

            if (sessionWindow == null)
            {
                lock (MessagingWindow.Windows)
                {
                    messagingWindow = MessagingWindow.Windows.FirstOrDefault(w => w.CanSendFile(remoteUri));
                }
                if (messagingWindow == null)
                {
                    messagingWindow = new MessagingWindow(remoteUri);
                }
                messagingWindow.SendFile(remoteUri, filePath);
            }
            else
            {
                sessionWindow.SendFile(remoteUri, filePath);
            }
        }

        internal static void SendSMS(String remoteUri)
        {
            MessagingWindow messagingWindow = null;
            
            lock (MessagingWindow.Windows)
            {
                messagingWindow = MessagingWindow.Windows.FirstOrDefault(w => w.CanSendSMS(remoteUri));
            }
            if (messagingWindow == null)
            {
                messagingWindow = new MessagingWindow(remoteUri);
            }
            messagingWindow.SendSMS(remoteUri);
        }

        internal static void ReceiveShortMessage(String remoteUri, byte[] payload, String contentType)
        {
            MessagingWindow messagingWindow = null;

            lock (MessagingWindow.Windows)
            {
                messagingWindow = MessagingWindow.Windows.FirstOrDefault(w => w.CanReceiveShortMessage(remoteUri));
            }
            if (messagingWindow == null)
            {
                messagingWindow = new MessagingWindow(remoteUri);
            }
            messagingWindow.ReceiveShortMessage(remoteUri, payload, contentType);
        }
    }
}
