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
using org.doubango.tinyWRAP;

namespace BogheApp.Screens
{
    partial class ScreenOptions
    {
        Profile[] Profiles = new Profile[]
            {
                new Profile("Default (User Defined)", tmedia_profile_t.tmedia_profile_default),
                new Profile("RTCWeb (Override)", tmedia_profile_t.tmedia_profile_rtcweb)
            };

        void InitializeGeneral()
        {
            this.comboBoxProfile.ItemsSource = Profiles;
        }

        private void LoadGeneral()
        {
            tmedia_profile_t profile = (tmedia_profile_t)Enum.Parse(typeof(tmedia_profile_t),
                this.configurationService.Get(Configuration.ConfFolder.MEDIA, Configuration.ConfEntry.PROFILE, Configuration.DEFAULT_MEDIA_PROFILE));
            int profileIndex = Profiles.ToList().FindIndex(x => x.Value == profile);

            this.comboBoxProfile.SelectedIndex = Math.Max(0, profileIndex);
            this.checkBoxLaunchWhenStart.IsChecked = this.configurationService.Get(Configuration.ConfFolder.GENERAL, Configuration.ConfEntry.AUTO_START, Configuration.DEFAULT_GENERAL_AUTOSTART);
            this.textBoxENUM.Text = this.configurationService.Get(Configuration.ConfFolder.GENERAL, Configuration.ConfEntry.ENUM_DOMAIN, Configuration.DEFAULT_GENERAL_ENUM_DOMAIN);
        }

        private bool UpdateGeneral()
        {
            tmedia_profile_t profile = (this.comboBoxProfile.SelectedValue as Profile).Value;
            this.configurationService.Set(Configuration.ConfFolder.MEDIA, Configuration.ConfEntry.PROFILE, profile.ToString());
            this.configurationService.Set(Configuration.ConfFolder.GENERAL, Configuration.ConfEntry.AUTO_START, this.checkBoxLaunchWhenStart.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.GENERAL, Configuration.ConfEntry.ENUM_DOMAIN, this.textBoxENUM.Text);

            // Transmit values to the native part (global)
            MediaSessionMgr.defaultsSetProfile(profile);

            return true;
        }

        public class Profile
        {
            readonly String text;
            readonly tmedia_profile_t value;

            public Profile(String text, tmedia_profile_t value)
            {
                this.text = text;
                this.value = value;
            }

            public String Text
            {
                get { return this.text; }
            }

            public tmedia_profile_t Value
            {
                get { return this.value; }
            }
        }
    }
}
