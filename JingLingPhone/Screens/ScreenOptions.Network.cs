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
        private void LoadNetwork()
        {
            this.textBoxProxyHost.Text = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_HOST, Configuration.DEFAULT_NETWORK_PCSCF_HOST);
            this.textBoxProxyPort.Text = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_PORT, Configuration.DEFAULT_NETWORK_PCSCF_PORT.ToString());
            String transport = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.TRANSPORT, Configuration.DEFAULT_NETWORK_TRANSPORT);
            this.comboBoxTransport.SelectedIndex = transport.Equals("UDP") ? 0 : (transport.Equals("TCP") ? 1 : 2);
            this.checkBoxDiscoDNS.IsChecked = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_DISCOVERY_DNS, Configuration.DEFAULT_NETWORK_PCSCF_DISCOVERY_DNS);
            this.checkBoxDiscoDHCP.IsChecked = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_DISCOVERY_DHCP, Configuration.DEFAULT_NETWORK_PCSCF_DISCOVERY_DHCP);
            this.checkBoxSigComp.IsChecked = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.SIGCOMP, Configuration.DEFAULT_NETWORK_SIGCOMP);
            String IPversion = this.configurationService.Get(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.IP_VERSION, Configuration.DEFAULT_NETWORK_IP_VERSION);
            this.radioButtonIPv4.IsChecked = IPversion.Equals("IPv4", StringComparison.InvariantCultureIgnoreCase);
            this.radioButtonIPv6.IsChecked = !this.radioButtonIPv4.IsChecked;
        }

        private bool UpdateNetwork()
        {
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_HOST, this.textBoxProxyHost.Text);
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_PORT, this.textBoxProxyPort.Text);
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.TRANSPORT, this.comboBoxTransport.SelectionBoxItem.ToString());

            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_DISCOVERY_DNS, this.checkBoxDiscoDNS.IsChecked.HasValue ? this.checkBoxDiscoDNS.IsChecked.Value : Configuration.DEFAULT_NETWORK_PCSCF_DISCOVERY_DNS);
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.PCSCF_DISCOVERY_DHCP, this.checkBoxDiscoDHCP.IsChecked.HasValue ? this.checkBoxDiscoDHCP.IsChecked.Value : Configuration.DEFAULT_NETWORK_PCSCF_DISCOVERY_DHCP);
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.SIGCOMP, this.checkBoxSigComp.IsChecked.HasValue ? this.checkBoxSigComp.IsChecked.Value : Configuration.DEFAULT_NETWORK_SIGCOMP);
            this.configurationService.Set(Configuration.ConfFolder.NETWORK, Configuration.ConfEntry.IP_VERSION, (this.radioButtonIPv6.IsChecked.HasValue && this.radioButtonIPv6.IsChecked.Value) ? "IPv6" : "IPv4");
            

            return true;
        }
    }
}
