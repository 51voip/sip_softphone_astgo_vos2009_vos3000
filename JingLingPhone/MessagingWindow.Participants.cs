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
using System.ComponentModel;
using BogheCore.Utils;

namespace BogheApp
{
    partial class MessagingWindow
    {
        class Participant : IComparable<Participant>, INotifyPropertyChanged
        {
            readonly String displayName;
            readonly String sipUri;
            confStatus status;
            bool composing;


            internal enum confStatus
            {
                Online,
                Offline,
                Unknown
            }

            internal Participant(String sipUri)
                :this(sipUri, confStatus.Unknown)
            {
            }

            internal Participant(String sipUri, confStatus status)
            {
                this.sipUri = sipUri;
                this.status = status;
                this.displayName = UriUtils.GetDisplayName(this.sipUri);
            }

            public String DisplayName
            {
                get { return this.displayName; }
            }

            public String PresStatusImageSource
            {
                get { return "/BogheApp;component/embedded/16/user_offline_16.png"; }
            }

            public bool IsComposing
            {
                get 
                { 
                    return this.composing; 
                }
                set
                {
                    if (this.composing != value)
                    {
                        this.composing = value;
                        this.OnPropertyChanged("IsComposing");
                    }
                }
            }

            public String IsComposingVisibility
            {
                get { return this.IsComposing ? "Visible" : "Hidden"; }
            }

            public confStatus Status
            {
                get { return this.status; }
                set
                {
                    if (this.status != value)
                    {
                        this.status = value;
                        this.OnPropertyChanged("Status");
                    }
                }
            }

            public String ConfStatusImageSource
            {
                get
                {
                    switch (this.status)
                    {
                        case confStatus.Online:
                            return "/BogheApp;component/embedded/16/bullet_ball_glass_green_16.png";
                        case confStatus.Offline:
                            return "/BogheApp;component/embedded/16/bullet_ball_glass_red_16.png";
                        default:
                            return "/BogheApp;component/embedded/16/bullet_ball_glass_yellow_16.png";
                    }
                }
            }

            #region IComparable

            public int CompareTo(Participant other)
            {
                if (other == null)
                {
                    throw new ArgumentNullException("other");
                }
                return this.sipUri.CompareTo(other.sipUri);
            }

            #endregion

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(String propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion
        }
    }
}
