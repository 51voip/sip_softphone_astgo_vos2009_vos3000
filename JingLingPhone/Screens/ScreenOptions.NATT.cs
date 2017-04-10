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
        private void LoadNATT()
        {
            this.checkBoxIceEnabled.IsChecked = this.configurationService.Get(Configuration.ConfFolder.NATT, Configuration.ConfEntry.USE_ICE, Configuration.DEFAULT_NATT_USE_ICE);
            this.checkBoxStunTurnEnable.IsChecked = this.configurationService.Get(Configuration.ConfFolder.NATT, Configuration.ConfEntry.USE_STUN, Configuration.DEFAULT_NATT_USE_STUN);
            this.radioButtonStunDiscover.IsChecked = this.configurationService.Get(Configuration.ConfFolder.NATT, Configuration.ConfEntry.STUN_DISCO, Configuration.DEFAULT_NATT_STUN_DISCO);
            this.radioButtonStunUseThis.IsChecked = !this.radioButtonStunDiscover.IsChecked;
            this.textBoxStunServerAddress.Text = this.configurationService.Get(Configuration.ConfFolder.NATT, Configuration.ConfEntry.STUN_SERVER, Configuration.DEFAULT_NATT_STUN_SERVER);
            this.textBoxStunPort.Text = this.configurationService.Get(Configuration.ConfFolder.NATT, Configuration.ConfEntry.STUN_PORT, Configuration.DEFAULT_NATT_STUN_PORT).ToString();
        }

        private bool UpdateNATT()
        {
            this.configurationService.Set(Configuration.ConfFolder.NATT, Configuration.ConfEntry.USE_ICE, this.checkBoxIceEnabled.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.NATT, Configuration.ConfEntry.USE_STUN, this.checkBoxStunTurnEnable.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.NATT, Configuration.ConfEntry.STUN_DISCO, this.radioButtonStunDiscover.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.NATT, Configuration.ConfEntry.STUN_SERVER, this.textBoxStunServerAddress.Text);
            this.configurationService.Set(Configuration.ConfFolder.NATT, Configuration.ConfEntry.STUN_PORT, this.textBoxStunPort.Text);

            // STUN informaions are checked before each registration which means that we don't need to pass the config to the native part
            // Pass ICE config to the native part
            MediaSessionMgr.defaultsSetIceEnabled(this.checkBoxIceEnabled.IsChecked.Value);

            return true;
        }
    }
}
