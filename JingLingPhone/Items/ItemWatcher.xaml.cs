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
using BogheCore.Model;
using BogheControls;
using System.Globalization;
using BogheCore.Utils;
using BogheXdm.Generated.watcherinfo;
using BogheControls.Utils;
using BogheCore.Services;
using BogheApp.Services.Impl;

namespace BogheApp.Items
{
    /// <summary>
    /// Interaction logic for ItemWatcher.xaml
    /// </summary>
    public partial class ItemWatcher : BaseItem<WatcherInfo>
    {
        private readonly IXcapService xcapService;
        private readonly IContactService contactService;

        private WatcherInfo @event;

        public ItemWatcher()
        {
            InitializeComponent();

            this.xcapService = Win32ServiceManager.SharedManager.XcapService;
            this.contactService = Win32ServiceManager.SharedManager.ContactService;

            this.ValueLoaded += this.ItemWatcher_ValueLoaded;
        }

        private void ItemWatcher_ValueLoaded(object sender, EventArgs e)
        {
            this.@event = this.Value;

            this.labelUriString.Content = String.Format(CultureInfo.CurrentUICulture, "{0} ({1})", UriUtils.GetDisplayName(this.@event.WatcherUriString), this.@event.WatcherUriString);
            this.labelPackage.Content = String.Format(CultureInfo.CurrentUICulture, "Package ({0})", this.@event.Package);
            this.labelStatus.Content = String.Format(CultureInfo.CurrentUICulture, "{0} ({1})", ItemWatcher.GetAsString(this.@event.WatcherStatus), ItemWatcher.GetAsString(this.@event.WatcherEvent));

            switch (this.@event.WatcherStatus)
            {
                case watcherStatus.active:
                    this.imageStatus.Source = MyImageConverter.FromBitmap(Properties.Resources.bullet_ball_glass_green_16);
                    this.ctxMenu_Allow.IsEnabled = false;
                    break;
                case watcherStatus.pending:
                    this.imageStatus.Source = MyImageConverter.FromBitmap(Properties.Resources.bullet_ball_glass_yellow_16);
                    break;
                case watcherStatus.waiting:
                    this.imageStatus.Source = MyImageConverter.FromBitmap(Properties.Resources.bullet_ball_glass_grey_16);
                    break;
                case watcherStatus.terminated:
                    this.imageStatus.Source = MyImageConverter.FromBitmap(Properties.Resources.bullet_ball_glass_red_16);
                    this.ctxMenu_Allow.IsEnabled = false;
                    this.ctxMenu_Block.IsEnabled = false;
                    this.ctxMenu_Revoke.IsEnabled = false;
                    break;
            }
        }

        private void ctxMenu_Allow_Click(object sender, RoutedEventArgs e)
        {
            if (this.@event == null || String.IsNullOrEmpty(this.@event.WatcherUriString))
            {
                return;
            }
            
            this.contactService.ContactAuthorize(this.GetContact(), BogheXdm.Authorization.Allowed);
        }

        private void ctxMenu_Block_Click(object sender, RoutedEventArgs e)
        {
            if (this.@event == null || String.IsNullOrEmpty(this.@event.WatcherUriString))
            {
                return;
            }

            this.contactService.ContactAuthorize(this.GetContact(), BogheXdm.Authorization.Blocked);
        }

        private void ctxMenu_Revoke_Click(object sender, RoutedEventArgs e)
        {
            if (this.@event == null || String.IsNullOrEmpty(this.@event.WatcherUriString))
            {
                return;
            }
            
            this.contactService.ContactAuthorize(this.GetContact(), BogheXdm.Authorization.Revoked);
        }

        private Contact GetContact()
        {
            String uri = this.@event.WatcherUriString; // e.g. sip:test@doubango.org;pres-list=rcs
            //if (uri.Contains(";"))
            //{
            //    uri = uri.Substring(0, uri.IndexOf(";"));
           //}

            Contact contact = this.contactService.ContactFind(uri);
            if (contact == null)
            {
                contact = new Contact();
                contact.UriString = uri;
            }
            return contact;
        }

        private static String GetAsString(watcherStatus status)
        {
            switch (status)
            {
                case watcherStatus.pending:
                    return "Pending";
                case watcherStatus.active:
                    return "Active";
                case watcherStatus.waiting:
                    return "Waiting";
                case watcherStatus.terminated:
                    return "Terminated";
                default:
                    return "Unknown";
            }
        }

        private static String GetAsString(watcherEvent @event)
        {
            switch (@event)
            {
                case watcherEvent.subscribe:
                    return "Subscribe";
                case watcherEvent.approved:
                    return "Approved";
                case watcherEvent.deactivated:
                    return "Deactivated";
                case watcherEvent.probation:
                    return "Probation";
                case watcherEvent.rejected:
                    return "Rejected";
                case watcherEvent.timeout:
                    return "Time Out";
                case watcherEvent.giveup:
                    return "Give Up";
                case watcherEvent.noresource:
                    return "No Resource";
                default:
                    return "Unknown";
            }
        }
    }
}
