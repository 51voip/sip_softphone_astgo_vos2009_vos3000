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
using BogheCore.Events;
using BogheCore.Model;
using System.ComponentModel;
using System.Threading;
using BogheCore;
using System.Windows.Data;
using System.Windows.Media;
using BogheCore.Sip.Events;
using BogheCore.Sip;
using BogheCore.Generated.pidf;
using System.IO;
using System.Xml.Serialization;
using BogheCore.Generated.data_model;
using BogheCore.Utils;
using BogheCore.Generated.oma.pidf_pres;

namespace BogheApp.Screens
{
    partial class ScreenContacts
    {
        private void sipService_onSubscriptionEvent(object sender, SubscriptionEventArgs e)
        {
            if (e.Type != SubscriptionEventTypes.INCOMING_NOTIFY || 
                (e.Package != MySubscriptionSession.EVENT_PACKAGE_TYPE.PRESENCE && e.Package != MySubscriptionSession.EVENT_PACKAGE_TYPE.PRESENCE_LIST)
                || e.Content == null)
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
                                this.ParsePresenceContent(e);
                            });
                        })
                        .Start();
                        break;
                    }
            }
        }

        private void ParsePidf(byte[] pidfContent)
        {
            presence presence;

            using (MemoryStream stream = new MemoryStream(pidfContent))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(presence));
                presence = serializer.Deserialize(stream) as presence;
            }

            PresenceStatus status = PresenceStatus.Offline;

            if (presence != null)
            {
                person[] persons = presence.Persons;
                person person = null;
                if (persons != null)
                {
                    if (persons.Length > 0)
                    {
                        person = persons[0];
                    }

                    DateTime lastTimeStamp = DateTime.MinValue;
                    foreach (person p in persons)
                    {
                        String timeStamp = p.GetTimeStamp();
                        if (!String.IsNullOrEmpty(timeStamp))
                        {
                            DateTime timestamp = Rfc3339DateTime.Parse(timeStamp);
                            if (timestamp.CompareTo(lastTimeStamp) > 0)
                            {
                                lastTimeStamp = timestamp;
                                person = p;
                            }
                        }
                    }
                }
                String statusicon = (person != null && person.statusicon != null) ? person.statusicon.Value : null;

                Contact contact = this.contactService.ContactFind(presence.entity);
                if (contact != null)
                {
                    if (person != null)
                    {
                        //
                        // Basic
                        //
                        if (person.overridingWillingness != null)
                        {
                            status = (person.overridingWillingness.basic == basicType.closed) ? PresenceStatus.Offline : PresenceStatus.Online;
                            if (!String.IsNullOrEmpty(person.overridingWillingness.Until))
                            {
                                contact.HyperAvaiability = Rfc3339DateTime.Parse(person.overridingWillingness.Until).ToLocalTime();
                            }
                        }

                        //
                        //  Activities
                        //
                        if (person.activities != null && person.activities.ItemsElementName != null)
                        {
                            if (person.activities.ItemsElementName.Length > 0)
                            {
                                switch (person.activities.ItemsElementName[0])
                                {
                                    case BogheCore.Generated.rpid.ItemsChoiceType.away:
                                    case BogheCore.Generated.rpid.ItemsChoiceType.shopping:
                                    case BogheCore.Generated.rpid.ItemsChoiceType.sleeping:
                                    case BogheCore.Generated.rpid.ItemsChoiceType.working:
                                    case BogheCore.Generated.rpid.ItemsChoiceType.appointment:
                                        status = PresenceStatus.Away;
                                        break;

                                    case BogheCore.Generated.rpid.ItemsChoiceType.busy:
                                        status = PresenceStatus.Busy;
                                        break;

                                    case BogheCore.Generated.rpid.ItemsChoiceType.vacation:
                                        status = PresenceStatus.BeRightBack;
                                        break;

                                    case BogheCore.Generated.rpid.ItemsChoiceType.onthephone:
                                    case BogheCore.Generated.rpid.ItemsChoiceType.playing:
                                        status = PresenceStatus.OnThePhone;
                                        break;

                                    case BogheCore.Generated.rpid.ItemsChoiceType.dinner:
                                    case BogheCore.Generated.rpid.ItemsChoiceType.breakfast:
                                    case BogheCore.Generated.rpid.ItemsChoiceType.meal:
                                        status = PresenceStatus.OutToLunch;
                                        break;
                                }
                            }
                        }

                        // Assign status
                        contact.PresenceStatus = status;

                        // Free Text
                        String note = person.GetNote();
                        if (!String.IsNullOrEmpty(note))
                        {
                            contact.FreeText = note;
                        }

                        // Avatar
                        /*if (!String.IsNullOrEmpty(statusicon))
                        {
                            contact.Avatar = this.GetContactStatusIcon(statusicon);
                        }*/

                        // Home Page
                        String hp = person.homepage;
                        if (!String.IsNullOrEmpty(hp))
                        {
                            contact.HomePage = hp;
                        }

                        // Service willingness (open/closed)
                        // IMPORTANT: ignore availability[service.status]
                        /*if (presence.tuple != null && presence.tuple.Length > 0)
                        {
                            foreach (tuple service in presence.tuple)
                            {
                                if (service != null && service.willingness != null && service.serviceDescription != null)
                                {
                                    if (service.willingness.basic == basicType.closed)
                                    {
                                        contact.AddClosedServices(service.serviceDescription.serviceid);
                                    }
                                    else if (contact.ClosedServices.Contains(service.serviceDescription.serviceid))
                                    {
                                        contact.RemoveClosedServices(service.serviceDescription.serviceid);
                                    }
                                }
                            }
                        }*/
                    }
                    else
                    {
                        // Get the first tuple
                        tuple tuple = (presence.tuple != null && presence.tuple.Length > 0) ? presence.tuple[0] : null;
                        contact.PresenceStatus = (tuple != null && tuple.status != null && tuple.status.basic == basic.open) ? PresenceStatus.Online : PresenceStatus.Offline;
                    }
                }
            }
        }

        private void ParseRLMI(byte[] rmliContent)
        {
        }

        private void ParsePresenceContent(SubscriptionEventArgs e)
        {
            try
            {
                if (ContentType.MULTIPART_RELATED.Equals(e.ContentType) && (e.GetExtra(SubscriptionEventArgs.EXTRA_CONTENTYPE_TYPE) as String).Contains(ContentType.RLMI))
                {
                    String boundary = e.GetExtra(SubscriptionEventArgs.EXTRA_CONTENTYPE_BOUNDARY) as String;
                    // to support both \r\n and \n\n
                   String[] contents = Encoding.UTF8.GetString(e.Content).Split(new String[] { ("\n--" + boundary), ("\r--" + boundary) }, StringSplitOptions.RemoveEmptyEntries);
                   int indexStart, indexEnd, contentStart;
                   String contentType;
                   foreach(String content in contents)
                   {
                       String _content = content.Trim();
                       if (_content == String.Empty)
                       {
                           continue;
                       }
                       indexStart = _content.IndexOf("Content-Type:", StringComparison.InvariantCultureIgnoreCase);
                       if (indexStart == -1)
                       {
                           continue;
                       }
                       indexEnd = _content.IndexOf("\n", indexStart);
                       if (indexEnd == -1)
                       {
                           continue;
                       }
                       contentType = _content.Substring(indexStart + 13, (indexEnd - indexStart - 13)).Split(";".ToCharArray(),  StringSplitOptions.RemoveEmptyEntries)[0].Trim();

                       if ((contentStart = _content.IndexOf("\r\n\r\n")) == -1 && (contentStart = _content.IndexOf("\n\n\r\n")) == -1)
                       {
                           continue;
                       }
                       _content = _content.Substring(contentStart).Trim();

                       if (ContentType.RLMI.Equals(contentType, StringComparison.InvariantCultureIgnoreCase))
                       {
                           this.ParseRLMI(Encoding.UTF8.GetBytes(_content));
                       }
                       else if (ContentType.PIDF.Equals(contentType, StringComparison.InvariantCultureIgnoreCase))
                       {
                           this.ParsePidf(Encoding.UTF8.GetBytes(_content));
                       }
                   }
                }
                else if (ContentType.RLMI.Equals(e.ContentType))
                {
                    this.ParseRLMI(e.Content);
                }
                else if (ContentType.PIDF.Equals(e.ContentType))
                {
                    this.ParsePidf(e.Content);
                }
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
        }

        private void contactService_onContactEvent(object sender, ContactEventArgs e)
        {
            if (e.Type != ContactEventTypes.RESET && e.Type != ContactEventTypes.GROUP_UPDATED && e.Type != ContactEventTypes.GROUP_REMOVED)
            {
                return;
            }

            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        new EventHandler<ContactEventArgs>(this.contactService_onContactEvent), sender, new object[] { e });
                return;
            }

            this.UpdateSource();
        }

        private void UpdateSource()
        {
            bool firstTime = (this.contactsView == null);
            if (this.dataSource != null)
            {
                this.dataSource.onItemPropChanged -= this.dataSource_onItemPropChanged;
            }
            this.dataSource = this.contactService.Contacts;
            this.dataSource.onItemPropChanged += this.dataSource_onItemPropChanged;

            this.listBox.ItemsSource = this.dataSource;
            this.contactsView = CollectionViewSource.GetDefaultView(this.listBox.ItemsSource);
            (this.contactsView as ListCollectionView).CustomSort = new ContactsSorter();
            this.contactsView.GroupDescriptions.Clear();
            this.contactsView.GroupDescriptions.Add(new PropertyGroupDescription("GroupName"));
            

            IList<FilterItem> filterItems = new List<FilterItem>();
            filterItems.Add(new FilterItem(null, "所有联系人", FilterItem.ImageSourceFromAuthorization(BogheXdm.Authorization.All)));
            foreach(Group g in this.contactService.Groups)
            {
                if (BogheXdm.SpecialNames.IsSpecial(g.Name))
                {
                    switch (g.Name)
                    {
                        case BogheXdm.SpecialNames.SHARED_RCS:
                        case BogheXdm.SpecialNames.SHARED_RCS_BLOCKEDCONTACTS:
                        case BogheXdm.SpecialNames.SHARED_RCS_REVOKEDCONTACTS:
                            break;
                        default:
                            continue;
                    }
                }

                string value = g.DisplayName;

                if (value == "My Contacts")
                {
                    value = "我的好友";
                }

                filterItems.Add(new FilterItem(g.Name, value, FilterItem.ImageSourceFromAuthorization(g.Authorization)));
            }

            this.comboBoxGroups.ItemsSource = filterItems;
            this.comboBoxGroups.SelectedIndex = 0;

            this.contactsView.Filter = delegate(object c)
            {
                if (this.comboBoxGroups.SelectedIndex < 0)
                {
                    return true;
                }

                Contact contact = c as Contact;
                FilterItem fItem = this.comboBoxGroups.SelectedItem as FilterItem;
                if (fItem == null || contact == null || String.IsNullOrEmpty(contact.GroupName))
                {
                    return false;
                }
                return (contact.DisplayName == null || contact.DisplayName.StartsWith(this.textBoxSearchCriteria.Text, StringComparison.InvariantCultureIgnoreCase)) && (fItem.Name == null || contact.GroupName.Equals(fItem.Name));
            };
        }

        private void dataSource_onItemPropChanged(object sender, StringEventArgs e)
        {
            if (this.contactsView != null)
            {
                this.contactsView.Refresh();
            }
        }
    }
}
