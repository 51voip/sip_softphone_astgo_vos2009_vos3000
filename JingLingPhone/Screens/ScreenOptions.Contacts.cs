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

namespace BogheApp.Screens
{
    partial class ScreenOptions
    {
        private void LoadContacts()
        {
            this.radioButtonContactsRemote.IsChecked = this.configurationService.Get(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.ENABLED, Configuration.DEFAULT_XCAP_ENABLED);
            this.textBoxXcapRoot.Text = this.configurationService.Get(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.XCAP_ROOT, Configuration.DEFAULT_XCAP_ROOT);
            this.textBoxXUI.Text = this.configurationService.Get(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.USERNAME, Configuration.DEFAULT_XUI);
            this.passwordBoxXUI.Password = this.configurationService.Get(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.PASSWORD, String.Empty);
            this.textBoxXcapTimeout.Text = this.configurationService.Get(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.TIMEOUT, Configuration.DEFAULT_XCAP_TIMEOUT).ToString();

            this.groupBoxXCAP.IsEnabled = this.radioButtonContactsRemote.IsChecked.HasValue ? this.radioButtonContactsRemote.IsChecked.Value : Configuration.DEFAULT_XCAP_ENABLED;
        }

        private bool UpdateContacts()
        {
            this.configurationService.Set(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.ENABLED, this.radioButtonContactsRemote.IsChecked.HasValue ? this.radioButtonContactsRemote.IsChecked.Value : Configuration.DEFAULT_XCAP_ENABLED);
            this.configurationService.Set(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.XCAP_ROOT, this.textBoxXcapRoot.Text);
            this.configurationService.Set(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.USERNAME, this.textBoxXUI.Text);
            this.configurationService.Set(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.PASSWORD, this.passwordBoxXUI.Password);
            this.configurationService.Set(Configuration.ConfFolder.XCAP, Configuration.ConfEntry.TIMEOUT, this.textBoxXcapTimeout.Text);

            return true;
        }
    }
}
