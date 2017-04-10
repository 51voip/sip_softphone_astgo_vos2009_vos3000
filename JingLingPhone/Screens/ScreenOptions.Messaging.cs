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
        private void LoadMessaging()
        {
            this.radioButtonMessagingSMSBinary.IsChecked = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.BINARY_SMS, Configuration.DEFAULT_RCS_BINARY_SMS);
            this.radioButtonMessagingSMSText.IsChecked = !this.radioButtonMessagingSMSBinary.IsChecked;
            this.textBoxMessagingPSI.IsEnabled = this.radioButtonMessagingSMSBinary.IsChecked.Value;
            this.textBoxMessagingPSI.Text = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.SMSC, Configuration.DEFAULT_RCS_SMSC);
            this.checkBoxMsrpSuccessReport.IsChecked = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.MSRP_SUCCESS, Configuration.DEFAULT_RCS_MSRP_SUCCESS);
            this.checkBoxMsrpFailureReports.IsChecked = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.MSRP_FAILURE, Configuration.DEFAULT_RCS_MSRP_FAILURE);
            this.checkBoxOFDR.IsChecked = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.OMAFDR, Configuration.DEFAULT_RCS_OMAFDR);
            this.checkBoxIMDN.IsChecked = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.IMDN, Configuration.DEFAULT_RCS_IMDN);
            this.checkBoxIsComposing.IsChecked = this.configurationService.Get(Configuration.ConfFolder.RCS, Configuration.ConfEntry.ISCOMOPING, Configuration.DEFAULT_RCS_ISCOMOPING);
        }

        private bool UpdateMessaging()
        {
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.BINARY_SMS, this.radioButtonMessagingSMSBinary.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.SMSC, this.textBoxMessagingPSI.Text);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.MSRP_SUCCESS, this.checkBoxMsrpSuccessReport.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.MSRP_FAILURE, this.checkBoxMsrpFailureReports.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.OMAFDR, this.checkBoxOFDR.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.IMDN, this.checkBoxIMDN.IsChecked.Value);
            this.configurationService.Set(Configuration.ConfFolder.RCS, Configuration.ConfEntry.ISCOMOPING, this.checkBoxIsComposing.IsChecked.Value);

            return true;
        }
    }
}
