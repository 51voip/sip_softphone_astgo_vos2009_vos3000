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
using BogheCore.Model;
using System.ComponentModel;
using System.Windows.Data;

namespace BogheApp.Screens
{
    partial class ScreenOptions
    {
        List<Privacy> privacies;
        ICollectionView privaciesView;

        private void Initializeidentity()
        {
            privacies = new List<Privacy>(new Privacy[]
            {
                new Privacy("none"),
                new Privacy("header"),
                new Privacy("session"),
                new Privacy("user"),
                new Privacy("critical"),
                new Privacy("id"),
            });

            this.listBoxPrivacy.ItemsSource = this.privacies;
            this.privaciesView = CollectionViewSource.GetDefaultView(this.listBoxPrivacy.ItemsSource);
        }

        private void LoadIdentity()
        {
            this.textBoxDisplayName.Text = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.DISPLAY_NAME, Configuration.DEFAULT_IDENTITY_DISPLAY_NAME);
            this.textBoxPublicIdentity.Text = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.IMPU, Configuration.DEFAULT_IDENTITY_IMPU);
            this.textBoxPrivateIdentity.Text = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.IMPI, Configuration.DEFAULT_IDENTITY_IMPI);
            this.passwordBoxSipPassword.Password = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.PASSWORD, String.Empty);
            this.textBoxRealm.Text = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.REALM, Configuration.DEFAULT_NETWORK_REALM);
            this.checkBoxEarlyIMS.IsChecked = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.EARLY_IMS, Configuration.DEFAULT_NETWORK_EARLY_IMS);

            this.textBoxAMF.Text = this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IMSAKA_AMF, Configuration.DEFAULT_IMSAKA_AMF);
            this.textBoxOperatorId.Text = this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IMSAKA_OPID, Configuration.DEFAULT_IMSAKA_OPID);
            
            String privacy = this.configurationService.Get(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.PRIVACY, Configuration.DEFAULT_IDENTITY_PRIVACY);
            String[] privaciesList = privacy.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.privacies.ForEach(x => x.IsEnabled = privaciesList.Contains(x.Name));
        }

        private bool UpdateIdentity()
        {
            this.configurationService.Set(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.DISPLAY_NAME, this.textBoxDisplayName.Text);
            this.configurationService.Set(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.IMPU, this.textBoxPublicIdentity.Text);
            this.configurationService.Set(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.IMPI, this.textBoxPrivateIdentity.Text);
            this.configurationService.Set(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.PASSWORD, this.passwordBoxSipPassword.Password);
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.REALM, this.textBoxRealm.Text);
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.EARLY_IMS, this.checkBoxEarlyIMS.IsChecked.HasValue ? this.checkBoxEarlyIMS.IsChecked.Value : Configuration.DEFAULT_NETWORK_EARLY_IMS);

            this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IMSAKA_AMF, this.textBoxAMF.Text);
            this.configurationService.Get(Configuration.ConfFolder.SECURITY, Configuration.ConfEntry.IMSAKA_OPID, this.textBoxOperatorId.Text);

            String privacy = String.Empty;
            this.privacies.ForEach(x => privacy = x.IsEnabled ? (privacy == String.Empty ? x.Name : String.Format("{0};{1}", privacy, x.Name)) : privacy);
            if (String.IsNullOrEmpty(privacy))
            {
                privacy = Configuration.DEFAULT_IDENTITY_PRIVACY;
            }
            this.configurationService.Set(Configuration.ConfFolder.IDENTITY, Configuration.ConfEntry.PRIVACY, privacy);

            return true;
        }


        private void CheckBoxPrivacy_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkbox = e.OriginalSource as System.Windows.Controls.CheckBox;
            if (checkbox != null)
            {
                if (checkbox.IsChecked.Value)
                {
                    if (checkbox.Tag.ToString().Equals("none"))
                    {
                        this.privacies.ForEach(x => x.IsEnabled = x.Name.Equals("none"));
                    }
                    else
                    {
                        this.privacies.Find(x => x.Name.Equals("none")).IsEnabled = false;
                    }

                    this.privaciesView.Refresh();
                }
            }
        }

        class Privacy
        {
            readonly String name;
            bool enabled;

            internal Privacy(String name)
            {
                this.name = name;
            }

            public String Name
            {
                get { return this.name; }
            }

            public bool IsEnabled
            {
                get { return this.enabled; }
                set { this.enabled = value; }
            }
        }
    }
}
